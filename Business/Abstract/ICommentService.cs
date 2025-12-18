using Business.DTOs;

namespace Business.Abstract
{
    public interface ICommentService
    {
        void AddComment(AddCommentDto commentDto, Guid userId);

        // Bir Kayak Merkezine ait yorumları getirme
        List<CommentDetailDto> GetCommentsBySkiArea(Guid skiAreaId);
    }
}
