using FamilyExperienceApp.Entities;

namespace FamilyExperienceApp.ViewModels
{
    public class ProfileVM
    {
        public MemberUpdateVM Member { get; set; }
        public List<Order> Orders { get; set; }
    }
}
