using System.ComponentModel.DataAnnotations;

namespace Entities.Models.Roles
{
    public class Role 
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Description { get; set; }

        public bool IsDelete { get; set; }
        #region Relations
        public virtual List<UserRoles> UserRoles { get; set; }

        #endregion
    }
}
