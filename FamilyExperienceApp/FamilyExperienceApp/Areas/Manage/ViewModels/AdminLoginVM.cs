using System.ComponentModel.DataAnnotations;

namespace FamilyExperienceApp.Areas.Manage.ViewModels
{
    public class AdminLoginVM
    {
        [Required]
        [MaxLength(20)]
        public string UserName { get; set; }
        [Required]
        [MaxLength(25)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
