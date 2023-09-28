using FamilyExperienceApp.Entities;

namespace FamilyExperienceApp.Areas.Manage.ViewModels
{
    public class ChartVM
    {
        public decimal TotalAmount { get; set; }
        public int TotalOrders { get; set; }
        public int NewOrderCount { get; set; }
        public int OldOrderCount { get; set; }
        public int AprilCount { get; set; }
        public int MayCount { get; set; }
        public int JuneCount { get; set; }
        public int JulyCount { get; set; }
        public int AugustCount { get; set; }
        public int SeptemberCount { get; set; }
        public List<Order> Orders { get; set; }
    }
}
