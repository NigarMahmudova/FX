using System.ComponentModel.DataAnnotations;

namespace FamilyExperienceApp.ViewModels
{
    public class ContactVM
    {
        [Required]
        [MaxLength(35)]
        public string FullName { get; set; }
        [Required]
        [MaxLength(100)]
        public string Email { get; set; }
        [Required]
        [MaxLength(15)]
        public string Phone { get; set; }
        [MaxLength(100)]
        public string Subject { get; set; }
        [MaxLength(250)]
        public string Message { get; set; }
    }
}
