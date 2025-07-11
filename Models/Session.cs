using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatWebApp.Models
{
    public class Session
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SessionId { get; set; }
        public string JWTtooken { get; set; }
        public DateTime Generatedtime {  get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public User User { get; set; }
    }
}
