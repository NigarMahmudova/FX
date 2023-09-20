using System.ComponentModel.DataAnnotations;

namespace FamilyExperienceApp.Entities
{
    public class EmailAddress
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Email { get; set; }
    }
}
