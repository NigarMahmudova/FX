using System.ComponentModel.DataAnnotations;

namespace FamilyExperienceApp.ViewModels
{
    public class EmailVM
    {
        [Required]
        [MaxLength(100)]
        public string Email { get; set; }
    }
}
