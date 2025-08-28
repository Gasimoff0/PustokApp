namespace PustokApp.Models.Common
{
    public class AuditEntity: BaseEntity
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
