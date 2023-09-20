namespace FamilyExperienceApp.ViewModels
{
    public class CheckoutVM
    {
        public List<CheckoutItemVM> Items { get; set; } = new List<CheckoutItemVM>();
        public decimal TotalAmount { get; set; }
        public OrderCreateVM Order { get; set; }
    }
}
