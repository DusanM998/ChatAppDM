using System.ComponentModel.DataAnnotations;

namespace ChatAppDusan.Dtos
{
    public class UserDto
    {
        [Required]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Ime mora biti duzine barem {2}, a najvise {1} karaktera!")]
        public string Name { get; set; }
        public int MyProperty { get; set; }
    }
}
