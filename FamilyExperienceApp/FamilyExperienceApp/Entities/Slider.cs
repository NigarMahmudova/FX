using FamilyExperienceApp.Attributes.CustomValidationAttributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyExperienceApp.Entities
{
    public class Slider
    {
        public int Id { get; set; }
        public int Order { get; set; }
        [MaxLength(50)]
        public string Title { get; set; }
        [MaxLength(150)]
        public string Desc { get; set; }
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
