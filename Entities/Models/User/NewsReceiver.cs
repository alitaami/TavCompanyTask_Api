using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models.User
{
    public class NewsReceiver
    {
        [Key]
        public int Id { get; set; }
        public string Email { get; set; }
        public int UserId { get; set; }

        // Relations
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
    }
}
