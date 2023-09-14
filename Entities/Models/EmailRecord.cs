using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class EmailRecord
    {
        [Key]
        public int Id { get; set; }  
        public string RecipientEmail { get; set; }  
        public string Subject { get; set; }  
        public string Body { get; set; }  
        public DateTime SentTime { get; set; } // Time when the email was sent.
        public DateTime? SeenTime { get; set; } // Time when the recipient viewed the email (nullable).

    }
}
