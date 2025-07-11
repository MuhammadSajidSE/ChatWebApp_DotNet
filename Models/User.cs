using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatWebApp.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId {  get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string phoneNo { get; set; }
    }
}
