using System.ComponentModel.DataAnnotations;

namespace FamilyExperienceApp.ViewModels
{
    public class OrderCreateVM
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
        [Required]
        [MaxLength(150)]
        public string Address { get; set; }
        [Required]
        [MaxLength(50)]
        public string City { get; set; }
        public string PostCode { get; set; }
        [MaxLength(250)]
        public string Note { get; set; }
    }
}
