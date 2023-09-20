using FamilyExperienceApp.Attributes.CustomValidationAttributes;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FamilyExperienceApp.Entities
{
    public class ImageCarousel
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string ImageName { get; set; }

        [NotMapped]
        [MaxFileLength(1073741824)]
        [AllowedContentTypes("image/jpeg", "image/jpg", "image/png")]
        public IFormFile ImageFile { get; set; }
    }
}
