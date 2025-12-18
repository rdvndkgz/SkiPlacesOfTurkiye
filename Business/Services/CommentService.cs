using Business.Abstract;
using Business.DTOs;
using DataAccess.Abstract;
using Entities.Entity;

namespace Business.Services
{
    public class CommentService : ICommentService
    {
        private readonly IRepository<Comment> _commentRepository;
        private readonly IRepository<User> _userRepository; 
        private readonly IUnitOfWork _uow;

        public CommentService(IRepository<Comment> commentRepository, IRepository<User> userRepository, IUnitOfWork uow)
        {
            _commentRepository = commentRepository;
            _userRepository = userRepository;
            _uow = uow;
        }

        public void AddComment(AddCommentDto commentDto, Guid userId)
        {
            var comment = new Comment
            {
                SkiAreaId = commentDto.SkiAreaId,
                UserId = userId, // Token'dan gelen GUID
                Content = commentDto.Content,
                CreateDate = DateTime.UtcNow
            };

            _commentRepository.Add(comment);
            _uow.Save();
        }

        public List<CommentDetailDto> GetCommentsBySkiArea(Guid skiAreaId)
        {
            var comments = _commentRepository.GetAll(x => x.SkiAreaId == skiAreaId);

            var result = comments.Select(c => new CommentDetailDto
            {
                Id = c.Id,
                Content = c.Content,
                CreatedDate = c.CreateDate,
                // Kullanıcı adını çekmek için User tablosuna gidiyoruz (Lazy Loading veya Include yoksa)
                // Performans için buraya User Include eklenmesi önerilir.
                Username = _userRepository.Get(u => u.Id == c.UserId)?.Username ?? "Anonim"
            }).OrderByDescending(x => x.CreatedDate).ToList();

            return result;
        }
    }
}
