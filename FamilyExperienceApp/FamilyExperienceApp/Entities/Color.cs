using System.ComponentModel.DataAnnotations;

namespace FamilyExperienceApp.Entities
{
    public class Color
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(25)]
        public string Name { get; set; }

        public List<Product> Products { get; set; }

    }
}
