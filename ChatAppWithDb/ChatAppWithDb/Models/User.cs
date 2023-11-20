using System.ComponentModel.DataAnnotations;

namespace ChatAppWithDb.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Ime mora biti duzine barem {2}, a najvise {1} karaktera!")]
        public string Name { get; set; }
    }
}
