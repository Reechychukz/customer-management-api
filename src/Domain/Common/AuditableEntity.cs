using System;
namespace Domain.Common
{
    public class AuditableEntity : IAuditableEntity
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}

