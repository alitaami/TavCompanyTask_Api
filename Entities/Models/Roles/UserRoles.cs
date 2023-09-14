using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models.Roles
{
    public class UserRoles  
    {
        public UserRoles()
        {

        }
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }

        #region relations

        [ForeignKey(nameof(UserId))]
        public virtual User.User User { get; set; }

        [ForeignKey(nameof(RoleId))]
        public virtual Role Role { get; set; }
        #endregion
    }
}
