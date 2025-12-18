using Business.Abstract;
using Business.DTOs;
using DataAccess.Abstract;
using DataAccess.Repositories;
using Entities.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Ocsp;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class AuthService : IAuthService
{
    private readonly IRepository<User> _userRepository;
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expiryMinutes;
    private readonly IUnitOfWork _uow;

    public AuthService(IRepository<User> userRepository, IConfiguration configuration, IUnitOfWork uow)
    {
        _userRepository = userRepository;
        _secretKey = configuration["JwtSettings:SecretKey"] ?? throw new Exception("JWT SecretKey eksik");
        _issuer = configuration["JwtSettings:Issuer"];
        _audience = configuration["JwtSettings:Audience"];
        _expiryMinutes = int.Parse(configuration["JwtSettings:ExpiryMinutes"] ?? "60");
        _uow = uow;
    }

    // token üretimi
    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        }),
            Expires = DateTime.UtcNow.AddMinutes(_expiryMinutes),
            Issuer = _issuer,
            Audience = _audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    //token kontrol ediyoruz
    public ClaimsPrincipal ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);

        try
        {
            var validationParams = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParams, out SecurityToken validatedToken);
            return principal;
        }
        catch
        {
            return null;
        }
    }

    //kayıt süreci
    public LoginResponseDto Register(RegisterRequest request)
    {
        // 1. Kullanıcı Kontrolü
        var existingUser = _userRepository.Get(x => x.Email == request.Email);
        if (existingUser != null)
            throw new Exception("Email zaten kayıtlı.");

        var user = new User
        {
            Username = request.Username,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Role = 0,
            Password = Encoding.UTF8.GetBytes(request.Password)
        };

        _userRepository.Add(user);
        _uow.Save();

        var token = GenerateJwtToken(user);

        return new LoginResponseDto
        {
            Token = token,
            Username = user.Username,
            Role = user.Role.ToString()
        };
    }

    // kayıt sürecine bağlı olarak token kontrolü entegreli login süreci
    public LoginResponseDto Login(LoginDto dto)
    {
        var user = _userRepository.Get(x => x.Email == dto.Email);
        if (user == null)
            throw new Exception("Email veya şifre hatalı.");

        var dtoPass = Encoding.UTF8.GetBytes(dto.Password);
        if (!dtoPass.SequenceEqual(user.Password))
            throw new Exception("Email veya şifre hatalı.");

        var newToken = GenerateJwtToken(user);

        return new LoginResponseDto
        {
            Token = newToken,
            Username = user.Username,
            Role = user.Role.ToString()
        };
    }
}
