using System.ComponentModel.DataAnnotations.Schema;

namespace ChatWebApp.Models
{
    public class Contacts
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [ForeignKey("User")]
        public int UserId {  get; set; }
        [ForeignKey("User")]
        public int ContactId { get; set; }
        ICollection<User> Users { get; set; }=new List<User>();
    }
}
