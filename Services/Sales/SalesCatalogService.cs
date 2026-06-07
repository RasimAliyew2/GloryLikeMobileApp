using System.Collections.ObjectModel;
using MetanetA_MobileApp.Model;

namespace MetanetA_MobileApp.Services.Sales
{
    public class SalesCatalogService
    {
        public ObservableCollection<SalesItem> Products { get; } = new();

        public SalesCatalogService()
        {

            // Demo məhsullar (özündə dəyiş)


               
                    Products.Add(new SalesItem
                    {
                        Name = $"Keramika və Mozoik yapıştırıcısı",
                        SubCategory = "Plitə yapışdırıcıları",
                        Image = "plite1.jpg",
                        Price = 25
                    });
                    Products.Add(new SalesItem
                    {
                        Name = $"Keramika və dekorativ daş yapıştırıcısı",
                        SubCategory = "Plitə yapışdırıcıları",
                        Image = "plite2.jpg",
                        Price = 16
                    });
                    Products.Add(new SalesItem
                    {
                        Name = $"Elastik və yüksək performanslı plitə yapıştırıcısı",
                        SubCategory = "Plitə yapışdırıcıları",
                        Image = "plite3.jpg",
                        Price = 50
                    });
                    Products.Add(new SalesItem
                    {
                        Name = $"Keramika yapıştırıcısı",
                        SubCategory = "Plitə yapışdırıcıları",
                        Description = "",
                        Image = "plite4.jpg",
                        Price = 30
                    });
               


            //    Products.Add(new SalesItem
            //{
            //    Name = "\"MATANAT A\" HYBRID keramika yapışdırıcısı (boz)",
            //    SubCategory = "Plitə yapışdırıcıları",
            //    Description = "\"MATANAT A\" HYBRID keramika yapışdırıcısı (boz)",
            //    Price = 20m,
            //    Image = "pic5.png"
            //});

            //Products.Add(new SalesItem
            //{
            //    Name = "Rokol silikon boya 3.5 Kg",
            //    SubCategory  = "Plitə yapışdırıcıları",
            //    Description = "Fasad boyaları",
            //    Price = 15.17m,
            //    Image = "pic1.png"
            //});

            //Products.Add(new SalesItem
            //{
            //    Name = "Rokol silikon boya 10 Kg",
            //    SubCategory = "Plitə yapışdırıcıları",
            //    Description = "Fasad boyaları",
            //    Price = 41.36m,
            //   Image = "pic2.png"
            //});

            //Products.Add(new SalesItem
            //{
            //    Name = "Rokol silikon boya 25 Kg",
            //    SubCategory = "Plitə yapışdırıcıları",
            //    Description = "Fasad boyaları",
            //    Price = 97.47m,
            //    Image = "pic3.png"
            //});

            //Products.Add(new SalesItem
            //{
            //    Name = "Rokol akrilik fasad boya 3.5 Kg",
            //    SubCategory = "Plitə yapışdırıcıları",
            //    Description = "Fasad boyaları",
            //    Price = 9.50m,
            //    Image = "pic4.png"
            //});
            //    Products.Add(new SalesItem
            //{
            //    Name = "\"MATANAT A\" HYBRID keramika yapışdırıcısı (boz)",
            //    SubCategory = "Plitə yapışdırıcıları",
            //    Description = "\"MATANAT A\" HYBRID keramika yapışdırıcısı (boz)",
            //    Price = 20m,
            //    Image = "pic5.png"
            //});

            //Products.Add(new SalesItem
            //{
            //    Name = "Rokol silikon boya 3.5 Kg",
            //    SubCategory  = "Plitə yapışdırıcıları",
            //    Description = "Fasad boyaları",
            //    Price = 15.17m,
            //    Image = "pic1.png"
            //});

            //Products.Add(new SalesItem
            //{
            //    Name = "Rokol silikon boya 10 Kg",
            //    SubCategory = "Plitə yapışdırıcıları",
            //    Description = "Fasad boyaları",
            //    Price = 41.36m,
            //   Image = "pic2.png"
            //});

            //Products.Add(new SalesItem
            //{
            //    Name = "Rokol silikon boya 25 Kg",
            //    SubCategory = "Plitə yapışdırıcıları",
            //    Description = "Fasad boyaları",
            //    Price = 97.47m,
            //    Image = "pic3.png"
            //});

            //Products.Add(new SalesItem
            //{
            //    Name = "Rokol akrilik fasad boya 3.5 Kg",
            //    SubCategory = "Plitə yapışdırıcıları",
            //    Description = "Fasad boyaları",
            //    Price = 9.50m,
            //    Image = "pic4.png"
            //});
            //    Products.Add(new SalesItem
            //{
            //    Name = "\"MATANAT A\" HYBRID keramika yapışdırıcısı (boz)",
            //    SubCategory = "Plitə yapışdırıcıları",
            //    Description = "\"MATANAT A\" HYBRID keramika yapışdırıcısı (boz)",
            //    Price = 20m,
            //    Image = "pic5.png"
            //});

            //Products.Add(new SalesItem
            //{
            //    Name = "Rokol silikon boya 3.5 Kg",
            //    SubCategory  = "Plitə yapışdırıcıları",
            //    Description = "Fasad boyaları",
            //    Price = 15.17m,
            //    Image = "pic1.png"
            //});

            //Products.Add(new SalesItem
            //{
            //    Name = "Rokol silikon boya 10 Kg",
            //    SubCategory = "Plitə yapışdırıcıları",
            //    Description = "Fasad boyaları",
            //    Price = 41.36m,
            //   Image = "pic2.png"
            //});

            //Products.Add(new SalesItem
            //{
            //    Name = "Rokol silikon boya 25 Kg",
            //    SubCategory = "Plitə yapışdırıcıları",
            //    Description = "Fasad boyaları",
            //    Price = 97.47m,
            //    Image = "pic3.png"
            //});

            //Products.Add(new SalesItem
            //{
            //    Name = "Rokol akrilik fasad boya 3.5 Kg",
            //    SubCategory = "Plitə yapışdırıcıları",
            //    Description = "Fasad boyaları",
            //    Price = 9.50m,
            //    Image = "pic4.png"
            //});
        }
    }
}
