using System.ComponentModel.DataAnnotations;

namespace ChatAppWithDb.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string From { get; set; }
        public string To { get; set; }
        [Required]
        public string Content { get; set; }
    }
}
