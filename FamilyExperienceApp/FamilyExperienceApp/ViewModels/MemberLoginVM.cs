using System.ComponentModel.DataAnnotations;

namespace FamilyExperienceApp.ViewModels
{
    public class MemberLoginVM
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
