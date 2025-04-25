using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Entities
{
    public partial class User
    {
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = null!;

        [Required]
        [Phone]
        public string Phone { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!; 

        public DateTime? CreatedAt { get; set; }

        public int? RoleId { get; set; }

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        public virtual Role? Role { get; set; }
        public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
    }
}