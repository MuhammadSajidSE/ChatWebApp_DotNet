using System.ComponentModel.DataAnnotations.Schema;

namespace ChatWebApp.Models
{
    public class Messages
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime DateTime { get; set; }
        [ForeignKey("User")]
        public int SenderId { get; set; }
        [ForeignKey("User")]
        public int RecieverId { get; set; }
        ICollection<User> Users { get; set; } = new List<User>();
    }
}
