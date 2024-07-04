using Microsoft.AspNetCore.Identity;
using Domain.Common;

namespace Domain.Entities
{
	public class User: IdentityUser<Guid>, IAuditableEntity
	{
        public string PhoneNumber { get; set; }
        public int StateId { get; set; }
        public State State { get; set; }
        public int LGAId { get; set; }
        public LGA LGA { get; set; }
        public bool IsVerified { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ICollection<UserActivity> UserActivities { get; set; }
    }
}

