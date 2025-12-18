using Business.Abstract;
using Business.DTOs.SkiAreaDtos;
using DataAccess;
using DataAccess.Abstract;
using Entities.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Minio;
using Minio.DataModel.Args;

namespace Business.Services
{
    public class SkiAreaService : ISkiAreaService
    {
        private IUnitOfWork unitOfWork;
        private IRepository<SkiArea> skiAreaRepository;
        private HttpContext httpContext;
        private MasterContext context;
        private readonly IMinioClient minioClient;

        public SkiAreaService(IRepository<SkiArea> skiRepo, IUnitOfWork uow, IHttpContextAccessor httpContextAccessor, MasterContext context, IMinioClient minio)
        {
            this.unitOfWork = uow;
            this.skiAreaRepository = skiRepo;
            this.httpContext = httpContextAccessor.HttpContext;
            this.context = context;
            this.minioClient = minio;
        }

        public void Create(CreateSkiAreaDto input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (string.IsNullOrWhiteSpace(input.Name) ||
               string.IsNullOrWhiteSpace(input.Description))
                throw new ArgumentException("İsim ve açıklama alanları gereklidir.");

            var skiArea = new SkiArea()
            {
                Id = Guid.NewGuid(),
                Name = input.Name,
                Description = input.Description,
                Rating = input.Rating,
                PhotoUrl = input.PhotoUrl,
            };

            skiAreaRepository.Add(skiArea);
            unitOfWork.Save();
        }

        public void Delete(string id)
        {
            if (!Guid.TryParse(id, out Guid areaId))
                throw new InvalidDataException(id);

            var deletedArea = skiAreaRepository.Get(area => area.Id.Equals(areaId));

            skiAreaRepository.Delete(deletedArea);
            unitOfWork.Save();
        }


        public void Update(SkiAreaDto input)
        {
            throw new NotImplementedException();
        }

        public void UploadPhoto(SkiAreaPhotoUploadDto dto)
        {
            if (dto.File == null || dto.File.Length == 0)
                throw new Exception("Lütfen bir fotoğraf seçin.");

            var skiArea = context.SkiAreas
                .FirstOrDefault(x => x.Name.ToLower() == dto.SkiAreaName.ToLower());

            if (skiArea == null)
                throw new Exception("Bu isimde bir kayak merkezi bulunamadı.");

            // dosyaların tutulacağı klasör
            string bucketName = "ski-area-photos";
            string objectName = $"{Guid.NewGuid()}_{dto.File.FileName}";

            
            var bucketExists = minioClient.BucketExistsAsync(
                    new BucketExistsArgs().WithBucket(bucketName)
                )
                .GetAwaiter().GetResult();

            if (!bucketExists)
            {
                minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName))
                    .GetAwaiter().GetResult();
            }

            using var stream = dto.File.OpenReadStream();

            minioClient.PutObjectAsync(
                    new PutObjectArgs()
                        .WithBucket(bucketName)
                        .WithObject(objectName)
                        .WithStreamData(stream)
                        .WithObjectSize(dto.File.Length)
                        .WithContentType(dto.File.ContentType)
                )
                .GetAwaiter().GetResult();

            string url = $"http://localhost:9000/{bucketName}/{objectName}";

            skiArea.PhotoUrl = url;
            context.SaveChanges();
        }

        public SkiAreaDto GetBySkiAreaName(string name)
        {
            var skiArea = skiAreaRepository.Get(x => x.Name.ToLower() == name.ToLower());

            if (skiArea == null)
            {
                // Bulamazsa null dönebiliriz, Controller 404 basar.
                return null;
                // Veya istersen hata fırlatabilirsin: 
                // throw new Exception("Kayak merkezi bulunamadı.");
            }

            // Entity'i DTO'ya çevirip döndürüyoruz
            return new SkiAreaDto
            {
                Id = skiArea.Id,
                Name = skiArea.Name,
                Description = skiArea.Description,
                PhotoUrl = skiArea.PhotoUrl,
                Rating = skiArea.Rating
            };  
        }

        public List<SkiAreaDto> GetAll()
        {
            var entities = skiAreaRepository.GetAll();

            // 2. Entity'yi DTO'ya çevir (Select ile)
            var dtos = entities.Select(x => new SkiAreaDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                PhotoUrl = x.PhotoUrl,
                Rating = x.Rating
            }).ToList();

            return dtos;
        }
    }
}
