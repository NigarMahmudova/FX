using System.ComponentModel.DataAnnotations;

namespace FamilyExperienceApp.Entities
{
    public class Size
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(10)]
        public string Name { get; set; }

    }
}
