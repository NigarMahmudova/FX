using System.ComponentModel.DataAnnotations;

namespace FamilyExperienceApp.Entities
{
    public class Contact
    {
        public int Id { get; set; }
        public string? AppUserId { get; set; }
        [MaxLength(35)]
        public string FullName { get; set; }
        [MaxLength(100)]
        public string Email { get; set; }
        [MaxLength(15)]
        public string Phone { get; set; }
        [MaxLength(100)]
        public string Subject { get; set; }
        [MaxLength(250)]
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }

        public AppUser AppUser { get; set; }
    }
}
