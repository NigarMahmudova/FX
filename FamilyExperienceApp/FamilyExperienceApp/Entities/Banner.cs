using FamilyExperienceApp.Attributes.CustomValidationAttributes;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FamilyExperienceApp.Entities
{
    public class Banner
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string Title { get; set; }
        [MaxLength(50)]
        public string SubTitle1 { get; set; }
        [MaxLength(50)]
        public string SubTitle2 { get; set; }
        [MaxLength(50)]
        public string BtnText { get; set; }
        [MaxLength(150)]
        public string BtnUrl { get; set; }
        [MaxLength(100)]
        public string ImageName { get; set; }

        [NotMapped]
        [MaxFileLength(1073741824)]
        [AllowedContentTypes("image/jpeg", "image/jpg", "image/png")]
        public IFormFile ImageFile { get; set; }
    }
}
