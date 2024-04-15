namespace CloudMining.Domain.Models.Base
{
    public abstract class Entity
    {
        public Guid Id { get; set; }
        public string? Caption { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime DeletedDate { get; set; }
    }
}
