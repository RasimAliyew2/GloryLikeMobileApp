using System;

namespace MetanetA_MobileApp.Model
{
    public class SalesItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Name { get; set; }

        // Yeni struktur
        public string MainCategory { get; set; }   // Ana başlıq
        public string SubCategory { get; set; }    // Sub kateqoriya

        // Köhnə kod qırılmasın deyə saxlanılır
        public string Category
        {
            get => SubCategory;
            set => SubCategory = value;
        }

        public string Weight { get; set; }
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
    }
}