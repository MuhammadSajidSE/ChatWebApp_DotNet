using System.ComponentModel.DataAnnotations.Schema;
namespace ChatWebApp.Models
{
    public class UsavedContact
    {
        public int Id { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        [ForeignKey("User")]
        public int ContactId { get; set; }
        public string ContactNumber { get; set; }
        public DateTime LastMessage { get; set; }
        ICollection<User> Users { get; set; } = new List<User>();
    }
}
