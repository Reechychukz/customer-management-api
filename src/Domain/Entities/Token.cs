using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Entities
{
    public class Token
    {
        public Guid Id { get; set; }
        public TokenTypeEnum TokenType { get; set; }
        public string OTPToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public bool IsValid { get; set; }
    }
}

