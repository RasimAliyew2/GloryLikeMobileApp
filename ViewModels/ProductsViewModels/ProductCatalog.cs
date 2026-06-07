// ViewModels/ProductsViewModels/ProductCatalog.cs
using System.Collections.Generic;

namespace MetanetA_MobileApp.ViewModels.ProductsViewModels;

public static class ProductCatalog
{
    // Root category key -> (Title, SubCategories[])
    public static readonly IReadOnlyDictionary<string, (string Title, string[] SubCategories)> Data =
        new Dictionary<string, (string, string[])>
        {
            ["INSAAT"] = ("İNŞAAT SİSTEMLƏRİ", new[]
            {
                "Plitə yapışdırıcıları",
                "Plitə aralıq doldurucuları",
                "Təmir, Grout və Ankraj qarışıqları",
                "Hidroizolyasiya qarışıqları",
                "Gips əsaslı məhsullar",
                "Gipskarton lövhələri və gipskarton yardımçı məhsulları",
                "Gipskarton profil və aksesuarları",
                "Əhəng məhsulları",
                "Boya və lak məhsulları: İnşaat Boyaları",
                "Boya və lak məhsulları: Sənaye Boyaları",
                "Dekorativ boya və macunlar",
                "Maye Yapışdırıcı Qrupu",
                "Astarlar",
                "Səth qoruma və səth təmizləmə maddələri"
            }),

            ["FASAD"] = ("FASAD SİSTEMLƏRİ", new[]
            {
                "Mineral Daş yunu",
                "Termoizolyasiya məhsulları və aksesuarları",
                "İnovex Quru divar və fasad üzləmə sistemləri",
                "Dekorativ fasad suvaqları",
                "Fasad suvaq və hörgü məhsulları",
                "FASNATURAL - Təbii Ağlay əsaslı fasad lövhələri və arxitektura elementləri"
            }),

            ["YER"] = ("YER-DÖŞƏMƏ SİSTEMLƏRİ", new[]
            {
                "Özüyayılan Şap məhsulları, Self-levelling",
                "Sement əsaslı Səth Sərtləşdiriciləri",
                "Epoksid və poliuretan əsaslı döşəmə örtükləri"
            }),

            ["QATQI"] = ("QATQI SİSTEMLƏRİ", new[]
            {
                "Beton Qatqıları",
                "Beton yardımçı məhsulları",
                "Üyütməyə yardımçı məhsullar",
                "Yeraltı inşaat məhsulları"
            })
        };
}
