namespace FamilyExperienceApp.Entities
{
    public class BasketItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int SizeId { get; set; }
        public int Count { get; set; }
        public string AppUserId { get; set; }

        public Product Product { get; set; }
        public Size Size { get; set; }
        public AppUser AppUser { get; set; }
    }
}
