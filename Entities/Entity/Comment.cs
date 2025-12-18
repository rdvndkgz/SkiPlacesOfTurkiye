namespace Entities.Entity
{
    public class Comment : BaseEntity
    {
        public string Content { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;

        public Guid SkiAreaId { get; set; }
        public Guid UserId { get; set; }

        public virtual User User { get; set; }
        public virtual SkiArea SkiArea { get; set; }
    }
}
