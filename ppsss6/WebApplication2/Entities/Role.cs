using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Entities
{
    public partial class Role
    {
        public int RoleId { get; set; }

        [Required]
        [MaxLength(50)]
        public string RoleName { get; set; } = null!;

        public string? Description { get; set; }

        public virtual ICollection<User> Users { get; set; } = new List<User>();

        public Role(
            int roleId,
            string roleName,
            string? description)
        {
            RoleId = roleId;
            RoleName = roleName;
            Description = description;
        }
    }
}