using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MetanetA_MobileApp.Model;
using MetanetA_MobileApp.Services.UIState;
using MetanetA_MobileApp.View.Products;

namespace MetanetA_MobileApp.ViewModels.ProductsViewModels;

[QueryProperty(nameof(CategoryKey), "CategoryKey")]
public partial class ProductViewModel : BaseViewModel
{
    public ObservableCollection<ProductRootCategorySection> RootCategories { get; } = new();
    public ObservableCollection<ProductItem> SearchResults { get; } = new();

    [ObservableProperty]
    private string categoryKey;

    [ObservableProperty]
    private string searchText;

    public bool IsSearchActive => !string.IsNullOrWhiteSpace(SearchText);
    public bool IsCategoryViewVisible => !IsSearchActive;

    public ProductViewModel(BottomMenuState menuState) : base(menuState)
    {
        LoadAllCategories();
    }

    partial void OnCategoryKeyChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;

        ExpandRootCategory(value);
    }

    partial void OnSearchTextChanged(string value)
    {
        ApplySearch(value);
        OnPropertyChanged(nameof(IsSearchActive));
        OnPropertyChanged(nameof(IsCategoryViewVisible));
    }

    private void ApplySearch(string value)
    {
        SearchResults.Clear();

        if (string.IsNullOrWhiteSpace(value))
            return;

        var text = value.Trim();

        var results = RootCategories
            .SelectMany(root => root.SubCategories)
            .SelectMany(sub => sub.Products)
            .Where(product =>
                !string.IsNullOrWhiteSpace(product.Name) &&
                product.Name.Contains(text, StringComparison.OrdinalIgnoreCase))
            .OrderBy(product => product.Name.StartsWith(text, StringComparison.OrdinalIgnoreCase) ? 0 : 1)
            .ThenBy(product => product.Name)
            .ToList();

        foreach (var item in results)
            SearchResults.Add(item);
    }

    private void LoadAllCategories()
    {
        RootCategories.Clear();

        foreach (var key in GetRootCategoryOrder())
        {
            if (!ProductCatalog.Data.TryGetValue(key, out var data))
                continue;

            var root = new ProductRootCategorySection(
                key,
                data.Title,
                GetCategoryImage(key));

            foreach (var subName in data.SubCategories)
            {
                var sub = new ProductSubCategorySection(subName);

                if (sub.Name == "Plitə yapışdırıcıları")
                {
                    #region FirstProduct
                    sub.Products.Add(new ProductItem
                    {

                      Name = "Keramika və Mozoik yapıştırıcısı",
     
                      Description = data.Title,
                      ImageUrl = "plite1.jpg",
                      Price = 25,
                      AboutTheProduct = @"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <style>
        body {
            font-family: Arial, Helvetica, sans-serif;
            font-size: 14px;
            color: #243B53;
            line-height: 1.6;
            margin: 0;
            padding: 0;
            background-color: #ffffff;
        }

        .section-title {
            background: #EAEAEA;
            color: #243B53;
            font-weight: bold;
            font-size: 18px;
            text-transform: uppercase;
            padding: 10px 14px;
            margin: 18px 0 10px 0;
        }

        .sub-title {
            background: #EFEFEF;
            color: #243B53;
            font-weight: bold;
            font-size: 16px;
            padding: 8px 12px;
            margin: 16px 0 10px 0;
        }

        p {
            margin: 0 0 12px 0;
        }

        ul {
            margin: 0 0 12px 20px;
            padding: 0;
        }

        li {
            margin-bottom: 6px;
        }

        table {
            width: 100%;
            border-collapse: collapse;
            margin: 12px 0 18px 0;
            font-size: 14px;
        }

        th, td {
            border: 1px solid #A7B3C2;
            padding: 10px;
            vertical-align: top;
            text-align: left;
        }

        th {
            background-color: #F1F1F1;
            font-weight: bold;
        }

        .note {
            margin-top: 10px;
        }
    </style>
</head>
<body>

    <div class=""section-title"">Məhsul haqqında</div>
    <p>
        Sement əsaslı, məhsulun istifadə imkanlarını artıran qatqılarla zəngin, yüksək keyfiyyətli,
        hibrid və nano texnologiyalı, kombinasiyalı polimerləşdirilmiş, birkomponentli,
        boz rəngli keramika yapışdırıcısıdır.
    </p>
    <p>
        EN 12004-1 standartının C1TE sinfinin tələblərinə uyğundur.
    </p>

    <div class=""section-title"">İstifadə sahələri</div>
    <p>Bu keramika yapışdırıcısı:</p>
    <ul>
        <li>daxili və xarici məkanlarda;</li>
        <li>kiçik və orta ölçülü üzlük materialların beton, suvaq, döşəmə suvağı kimi səthlərin üzərinə yapışdırılmasında istifadə olunur.</li>
    </ul>

    <div class=""section-title"">Üstünlükləri</div>
    <ul>
        <li>Rahat hazırlanır və tətbiq olunur;</li>
        <li>Şaquli və üfüqi səthlərdə sürüşmə ehtimalı yoxdur;</li>
        <li>Yuxarıdan aşağıya doğru keramika, şüşə mozaika, şüşə bloklar və digər üzlük plitələri yapışdırmaq mümkündür;</li>
        <li>Şaxtaya və istiliyə davamlıdır.</li>
    </ul>

    <div class=""section-title"">İstifadə qaydaları</div>
    <p>
        <b>Səthin hazırlanması.</b> İstifadə olunacaq səthlərin sağlam, quru, təmiz və ölçülərinin düzgün
        olmasına diqqət etməli, səth yapışma müqavimətini zəiflədən hər növ yağ, parafin və digər
        qalıqlardan tamamilə təmizlənməlidir. Səthdə 5–6 mm dərinliyində çatlar və ya boşluqlar
        olarsa, onlar bir gün əvvəl Fikret təmir qarışığı və ya digər oxşar məhsulla doldurulmalıdır.
        Tətbiq olunacaq səth eyni zamanda əvvəldən nəmləndirilməli, ROKOL Binder Astar və ya digər
        oxşar məhsulla astarlanmalıdır.
    </p>

    <div class=""sub-title"">Qarışığın hazırlanması</div>
    <p>
        25 kq-lıq quru qarışığı ehmalca 6,0 – 6,5 litr suyun üzərinə boşaldıb kürəciklər yox olana qədər
        qarışdırın. Qarışığın özünü tutması üçün 3–5 dəqiqə gözləyin, istifadəyə başlamazdan əvvəl
        yenidən qarışdırın.
    </p>

    <div class=""sub-title"">Tətbiqi</div>
    <p>
        Malanın düz tərəfi ilə qarışığı nazik qatla səthə yayın. Sonra plitələrin ölçüsünə uyğun olaraq
        səthə vurulmuş qarışığı malanın dişli-daraqlı tərəfi ilə daralayın. İri ölçülü plitələrin
        yapışdırılmasında kombinə edilmiş yapışdırma üsulundan (bu üsulla yapışdırıcı həm səthə çəkilir
        və daraqlanır, həm də üzlük plitənin arxasına yaxılaraq daraqlanmış səthə yapışdırılır)
        istifadə olunması tövsiyə olunur.
    </p>
    <p>
        Plitənin yapışdırılma müddətini səthin üzərinə çəkilmiş qarışığa barmaqla azacıq toxunmaqla
        (basmaqla) müəyyən edin. Qarışıq barmağa yapışarsa, o zaman plitələri səthə yapışdırmaq olar.
        Əgər səthin üzərinə çəkilmiş qarışıq üz bağlayarsa, o zaman onu sıyırıb atmalı və həmin səthin
        üzərinə yenidən qarışıq vuraraq daraqlı mala ilə daraqladıqdan sonra plitələr yapışdırılmalıdır.
    </p>
    <p>
        Hazırlanan qarışıq 2 saat ərzində işlədilməlidir. İstifadə müddəti keçmiş və ya qabıq bağlamış
        qarışığı istifadə etmək məsləhət görülmür. Aralıq doldurma işi divarda 24 saatdan, döşəmədə isə
        48 saatdan sonra icra edilməlidir.
    </p>

    <div class=""sub-title"">Tövsiyələr</div>
    <p>
        “Matanat A” HYBRID keramika yapışdırıcısı istifadə olunarkən mühitin temperaturu +5°C-dən aşağı
        və +30°C-dən yuxarıdırsa, lazımi temperatur təmin olunmalıdır. İsti, yağışlı və küləkli
        havalarda istifadə olunmamalıdır. Su hopması yüksək olan keramika istifadə edəcəksinizsə,
        plitələri tətbiqdən öncə su ilə doyurulmalıdır. İstifadə müddəti keçmiş qarışığa qətiyyən su
        və ya yapışdırıcı əlavə edilməməlidir. İstifadədən sonra bütün alət və ləvazimatlar su ilə
        təmizlənməlidir.
    </p>

    <div class=""section-title"">Texniki göstəricilər</div>
    <table>
        <tr>
            <th>Göstərici</th>
            <th>Məlumat</th>
        </tr>
        <tr>
            <td>Rəngi</td>
            <td>boz</td>
        </tr>
        <tr>
            <td>İstifadə temperaturu</td>
            <td>+5° C - +30° C</td>
        </tr>
        <tr>
            <td>Su/quru qarışıq</td>
            <td>
                2,4–2,6 litr su / 10 kq quru qarışıq<br>
                6,0–6,5 litr su / 25 kq quru qarışıq
            </td>
        </tr>
        <tr>
            <td>Qarışdırıldıqdan sonra gözləmə müddəti</td>
            <td>3–5 dəqiqə</td>
        </tr>
        <tr>
            <td>İstifadə müddəti</td>
            <td>2 saat</td>
        </tr>
        <tr>
            <td>Aralıq doldurma işinə başlama vaxtı</td>
            <td>
                divarda — 24 saatdan sonra<br>
                döşəmədə — 48 saatdan sonra
            </td>
        </tr>
        <tr>
            <td>Qablaşdırma</td>
            <td>2 və ya 3 qat kraft kağızı və 1 qatlı polietilendən ibarət olan 10 və 25 kq-lıq kisələrdə</td>
        </tr>
    </table>

    <table>
        <tr>
            <th></th>
            <th>EN 12004-1 standartının tələblərinə uyğun olaraq</th>
            <th>Test nəticələri</th>
        </tr>
        <tr>
            <td>Qabıq bağlama müddəti</td>
            <td>≥30 dəqiqə</td>
            <td>30–40 dəqiqə</td>
        </tr>
        <tr>
            <td>Sürüşmə</td>
            <td>&lt;0,5 mm</td>
            <td>0</td>
        </tr>
        <tr>
            <td>Qopma müqaviməti</td>
            <td>&gt;0,5 N/mm²</td>
            <td>1–1,5 N/mm²</td>
        </tr>
    </table>

    <div class=""section-title"">Tövsiyə olunan daraqlı malanın dişlərinin ölçüləri və ölçülərə uyğun yapışdırıcının yayılma qalınlığı</div>
    <table>
        <tr>
            <th>Üzlük plitələrin ölçülərinə uyğun sahəsi</th>
            <th>Daraqlı malanın təsviri və dişlərinin ölçüləri, mm</th>
            <th>Yapışdırıcının yayılma qalınlığı, mm</th>
        </tr>
        <tr>
            <td>&lt;25 sm²</td>
            <td>3</td>
            <td>1–2</td>
        </tr>
        <tr>
            <td>25–100 sm²</td>
            <td>4</td>
            <td>2–3</td>
        </tr>
        <tr>
            <td>100–600 sm²</td>
            <td>6</td>
            <td>3–4</td>
        </tr>
        <tr>
            <td>600–1800 sm²</td>
            <td>8</td>
            <td>4–5</td>
        </tr>
        <tr>
            <td>1800 sm²</td>
            <td>10</td>
            <td>5–6</td>
        </tr>
    </table>

    <div class=""section-title"">Sərfiyyat</div>
    <p>
        İstifadə zamanı yapışdırıcının səthə çəkilmiş qalınlığından və dişli-daraqlı malanın dişlərinin
        ölçülərindən asılı olaraq 1 m²-ə sərf olunan quru qarışığın miqdarı (kq-la):
    </p>

    <table>
        <tr>
            <th>“Matanət A” HYBRID keramika yapışdırıcısı (boz) (kq)</th>
            <th>1 mm qalınlıq üçün quru qarışığın sərfiyyatı (kq/m²)</th>
            <th>Qarışdırılacaq suyun miqdarı (litr) — 25 kq üçün</th>
            <th>3 mm</th>
            <th>4 mm</th>
            <th>6 mm</th>
            <th>8 mm</th>
            <th>10 mm</th>
        </tr>
        <tr>
            <td>25</td>
            <td>1,28</td>
            <td>6–6,5</td>
            <td>1,3 kq</td>
            <td>2,6 kq</td>
            <td>3,9 kq</td>
            <td>5,1 kq</td>
            <td>6,4 kq</td>
        </tr>
    </table>

    <div class=""section-title"">Saxlama müddəti və qaydası</div>
    <p>
        Açılmamış kisələrdə, quru şəraitdə, üst-üstə maksimum 11 sıra olmaqla 12 ay saxlanıla bilər.
    </p>

    <div class=""section-title"">Xəbərdarlıqlar</div>
    <ul>
        <li>yapışdırıcının sement əsaslı və qatqılarla zəngin olduğunu nəzərə alaraq dəriyə və ya gözə düşdükdə dərhal su ilə yuyun;</li>
        <li>istifadə qaydalarında qeyd edilməyən heç bir əlavə maddə qatmayın;</li>
        <li>yuxarıdakı göstəricilər +23°C±2°C temperatur və 50% + 5% rütubətli şəraitdə aparılan laboratoriya sınaqları nəticəsində əldə olunmuşdur. Şəraitdən asılı olaraq bu göstəricilər dəyişə bilər.</li>
    </ul>

</body>
</html>"
                    });
                    #endregion
                    #region SecondProduct
                    sub.Products.Add(new ProductItem
                    {
                        Name = "Keramika və dekorativ daş yapıştırıcısı",
                        Description = data.Title,
                        ImageUrl = "plite2.jpg",
                        Price = 16,
                        AboutTheProduct = @"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <style>
        body {
            font-family: Arial, Helvetica, sans-serif;
            font-size: 14px;
            color: #243B53;
            line-height: 1.6;
            margin: 0;
            padding: 0;
            background-color: #ffffff;
        }

        .section-title {
            background: #EAEAEA;
            color: #243B53;
            font-weight: bold;
            font-size: 18px;
            text-transform: uppercase;
            padding: 10px 14px;
            margin: 18px 0 10px 0;
        }

        .sub-title {
            background: #EFEFEF;
            color: #243B53;
            font-weight: bold;
            font-size: 16px;
            padding: 8px 12px;
            margin: 16px 0 10px 0;
        }

        p {
            margin: 0 0 12px 0;
        }

        ul {
            margin: 0 0 12px 20px;
            padding: 0;
        }

        li {
            margin-bottom: 6px;
        }

        table {
            width: 100%;
            border-collapse: collapse;
            margin: 12px 0 18px 0;
            font-size: 14px;
        }

        th, td {
            border: 1px solid #A7B3C2;
            padding: 10px;
            vertical-align: top;
            text-align: left;
        }

        th {
            background-color: #F1F1F1;
            font-weight: bold;
        }

        .note {
            margin-top: 10px;
        }
    </style>
</head>
<body>

    <div class=""section-title"">Məhsul haqqında</div>
    <p>
        Sement əsaslı, məhsulun istifadə imkanlarını artıran qatqılarla zəngin, yüksək keyfiyyətli,
        hibrid və nano texnologiyalı, kombinasiyalı polimerləşdirilmiş, birkomponentli,
        boz rəngli keramika yapışdırıcısıdır.
    </p>
    <p>
        EN 12004-1 standartının C1TE sinfinin tələblərinə uyğundur.
    </p>

    <div class=""section-title"">İstifadə sahələri</div>
    <p>Bu keramika yapışdırıcısı:</p>
    <ul>
        <li>daxili və xarici məkanlarda;</li>
        <li>kiçik və orta ölçülü üzlük materialların beton, suvaq, döşəmə suvağı kimi səthlərin üzərinə yapışdırılmasında istifadə olunur.</li>
    </ul>

    <div class=""section-title"">Üstünlükləri</div>
    <ul>
        <li>Rahat hazırlanır və tətbiq olunur;</li>
        <li>Şaquli və üfüqi səthlərdə sürüşmə ehtimalı yoxdur;</li>
        <li>Yuxarıdan aşağıya doğru keramika, şüşə mozaika, şüşə bloklar və digər üzlük plitələri yapışdırmaq mümkündür;</li>
        <li>Şaxtaya və istiliyə davamlıdır.</li>
    </ul>

    <div class=""section-title"">İstifadə qaydaları</div>
    <p>
        <b>Səthin hazırlanması.</b> İstifadə olunacaq səthlərin sağlam, quru, təmiz və ölçülərinin düzgün
        olmasına diqqət etməli, səth yapışma müqavimətini zəiflədən hər növ yağ, parafin və digər
        qalıqlardan tamamilə təmizlənməlidir. Səthdə 5–6 mm dərinliyində çatlar və ya boşluqlar
        olarsa, onlar bir gün əvvəl Fikret təmir qarışığı və ya digər oxşar məhsulla doldurulmalıdır.
        Tətbiq olunacaq səth eyni zamanda əvvəldən nəmləndirilməli, ROKOL Binder Astar və ya digər
        oxşar məhsulla astarlanmalıdır.
    </p>

    <div class=""sub-title"">Qarışığın hazırlanması</div>
    <p>
        25 kq-lıq quru qarışığı ehmalca 6,0 – 6,5 litr suyun üzərinə boşaldıb kürəciklər yox olana qədər
        qarışdırın. Qarışığın özünü tutması üçün 3–5 dəqiqə gözləyin, istifadəyə başlamazdan əvvəl
        yenidən qarışdırın.
    </p>

    <div class=""sub-title"">Tətbiqi</div>
    <p>
        Malanın düz tərəfi ilə qarışığı nazik qatla səthə yayın. Sonra plitələrin ölçüsünə uyğun olaraq
        səthə vurulmuş qarışığı malanın dişli-daraqlı tərəfi ilə daralayın. İri ölçülü plitələrin
        yapışdırılmasında kombinə edilmiş yapışdırma üsulundan (bu üsulla yapışdırıcı həm səthə çəkilir
        və daraqlanır, həm də üzlük plitənin arxasına yaxılaraq daraqlanmış səthə yapışdırılır)
        istifadə olunması tövsiyə olunur.
    </p>
    <p>
        Plitənin yapışdırılma müddətini səthin üzərinə çəkilmiş qarışığa barmaqla azacıq toxunmaqla
        (basmaqla) müəyyən edin. Qarışıq barmağa yapışarsa, o zaman plitələri səthə yapışdırmaq olar.
        Əgər səthin üzərinə çəkilmiş qarışıq üz bağlayarsa, o zaman onu sıyırıb atmalı və həmin səthin
        üzərinə yenidən qarışıq vuraraq daraqlı mala ilə daraqladıqdan sonra plitələr yapışdırılmalıdır.
    </p>
    <p>
        Hazırlanan qarışıq 2 saat ərzində işlədilməlidir. İstifadə müddəti keçmiş və ya qabıq bağlamış
        qarışığı istifadə etmək məsləhət görülmür. Aralıq doldurma işi divarda 24 saatdan, döşəmədə isə
        48 saatdan sonra icra edilməlidir.
    </p>

    <div class=""sub-title"">Tövsiyələr</div>
    <p>
        “Matanat A” HYBRID keramika yapışdırıcısı istifadə olunarkən mühitin temperaturu +5°C-dən aşağı
        və +30°C-dən yuxarıdırsa, lazımi temperatur təmin olunmalıdır. İsti, yağışlı və küləkli
        havalarda istifadə olunmamalıdır. Su hopması yüksək olan keramika istifadə edəcəksinizsə,
        plitələri tətbiqdən öncə su ilə doyurulmalıdır. İstifadə müddəti keçmiş qarışığa qətiyyən su
        və ya yapışdırıcı əlavə edilməməlidir. İstifadədən sonra bütün alət və ləvazimatlar su ilə
        təmizlənməlidir.
    </p>

    <div class=""section-title"">Texniki göstəricilər</div>
    <table>
        <tr>
            <th>Göstərici</th>
            <th>Məlumat</th>
        </tr>
        <tr>
            <td>Rəngi</td>
            <td>boz</td>
        </tr>
        <tr>
            <td>İstifadə temperaturu</td>
            <td>+5° C - +30° C</td>
        </tr>
        <tr>
            <td>Su/quru qarışıq</td>
            <td>
                2,4–2,6 litr su / 10 kq quru qarışıq<br>
                6,0–6,5 litr su / 25 kq quru qarışıq
            </td>
        </tr>
        <tr>
            <td>Qarışdırıldıqdan sonra gözləmə müddəti</td>
            <td>3–5 dəqiqə</td>
        </tr>
        <tr>
            <td>İstifadə müddəti</td>
            <td>2 saat</td>
        </tr>
        <tr>
            <td>Aralıq doldurma işinə başlama vaxtı</td>
            <td>
                divarda — 24 saatdan sonra<br>
                döşəmədə — 48 saatdan sonra
            </td>
        </tr>
        <tr>
            <td>Qablaşdırma</td>
            <td>2 və ya 3 qat kraft kağızı və 1 qatlı polietilendən ibarət olan 10 və 25 kq-lıq kisələrdə</td>
        </tr>
    </table>

    <table>
        <tr>
            <th></th>
            <th>EN 12004-1 standartının tələblərinə uyğun olaraq</th>
            <th>Test nəticələri</th>
        </tr>
        <tr>
            <td>Qabıq bağlama müddəti</td>
            <td>≥30 dəqiqə</td>
            <td>30–40 dəqiqə</td>
        </tr>
        <tr>
            <td>Sürüşmə</td>
            <td>&lt;0,5 mm</td>
            <td>0</td>
        </tr>
        <tr>
            <td>Qopma müqaviməti</td>
            <td>&gt;0,5 N/mm²</td>
            <td>1–1,5 N/mm²</td>
        </tr>
    </table>

    <div class=""section-title"">Tövsiyə olunan daraqlı malanın dişlərinin ölçüləri və ölçülərə uyğun yapışdırıcının yayılma qalınlığı</div>
    <table>
        <tr>
            <th>Üzlük plitələrin ölçülərinə uyğun sahəsi</th>
            <th>Daraqlı malanın təsviri və dişlərinin ölçüləri, mm</th>
            <th>Yapışdırıcının yayılma qalınlığı, mm</th>
        </tr>
        <tr>
            <td>&lt;25 sm²</td>
            <td>3</td>
            <td>1–2</td>
        </tr>
        <tr>
            <td>25–100 sm²</td>
            <td>4</td>
            <td>2–3</td>
        </tr>
        <tr>
            <td>100–600 sm²</td>
            <td>6</td>
            <td>3–4</td>
        </tr>
        <tr>
            <td>600–1800 sm²</td>
            <td>8</td>
            <td>4–5</td>
        </tr>
        <tr>
            <td>1800 sm²</td>
            <td>10</td>
            <td>5–6</td>
        </tr>
    </table>

    <div class=""section-title"">Sərfiyyat</div>
    <p>
        İstifadə zamanı yapışdırıcının səthə çəkilmiş qalınlığından və dişli-daraqlı malanın dişlərinin
        ölçülərindən asılı olaraq 1 m²-ə sərf olunan quru qarışığın miqdarı (kq-la):
    </p>

    <table>
        <tr>
            <th>“Matanət A” HYBRID keramika yapışdırıcısı (boz) (kq)</th>
            <th>1 mm qalınlıq üçün quru qarışığın sərfiyyatı (kq/m²)</th>
            <th>Qarışdırılacaq suyun miqdarı (litr) — 25 kq üçün</th>
            <th>3 mm</th>
            <th>4 mm</th>
            <th>6 mm</th>
            <th>8 mm</th>
            <th>10 mm</th>
        </tr>
        <tr>
            <td>25</td>
            <td>1,28</td>
            <td>6–6,5</td>
            <td>1,3 kq</td>
            <td>2,6 kq</td>
            <td>3,9 kq</td>
            <td>5,1 kq</td>
            <td>6,4 kq</td>
        </tr>
    </table>

    <div class=""section-title"">Saxlama müddəti və qaydası</div>
    <p>
        Açılmamış kisələrdə, quru şəraitdə, üst-üstə maksimum 11 sıra olmaqla 12 ay saxlanıla bilər.
    </p>

    <div class=""section-title"">Xəbərdarlıqlar</div>
    <ul>
        <li>yapışdırıcının sement əsaslı və qatqılarla zəngin olduğunu nəzərə alaraq dəriyə və ya gözə düşdükdə dərhal su ilə yuyun;</li>
        <li>istifadə qaydalarında qeyd edilməyən heç bir əlavə maddə qatmayın;</li>
        <li>yuxarıdakı göstəricilər +23°C±2°C temperatur və 50% + 5% rütubətli şəraitdə aparılan laboratoriya sınaqları nəticəsində əldə olunmuşdur. Şəraitdən asılı olaraq bu göstəricilər dəyişə bilər.</li>
    </ul>

</body>
</html>"
                       
                    });
                    #endregion
                    #region ThirdProduct
                    sub.Products.Add(new ProductItem
                    {
                        Name = "Elastik və yüksək performanslı plitə yapıştırıcısı",
                        Description = data.Title,
                        ImageUrl = "plite3.jpg",
                        Price = 50,
                        AboutTheProduct = @"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <style>
        body {
            font-family: Arial, Helvetica, sans-serif;
            font-size: 14px;
            color: #243B53;
            line-height: 1.6;
            margin: 0;
            padding: 0;
            background-color: #ffffff;
        }

        .section-title {
            background: #EAEAEA;
            color: #243B53;
            font-weight: bold;
            font-size: 18px;
            text-transform: uppercase;
            padding: 10px 14px;
            margin: 18px 0 10px 0;
        }

        .sub-title {
            background: #EFEFEF;
            color: #243B53;
            font-weight: bold;
            font-size: 16px;
            padding: 8px 12px;
            margin: 16px 0 10px 0;
        }

        p {
            margin: 0 0 12px 0;
        }

        ul {
            margin: 0 0 12px 20px;
            padding: 0;
        }

        li {
            margin-bottom: 6px;
        }

        table {
            width: 100%;
            border-collapse: collapse;
            margin: 12px 0 18px 0;
            font-size: 14px;
        }

        th, td {
            border: 1px solid #A7B3C2;
            padding: 10px;
            vertical-align: top;
            text-align: left;
        }

        th {
            background-color: #F1F1F1;
            font-weight: bold;
        }

        .note {
            margin-top: 10px;
        }
    </style>
</head>
<body>

    <div class=""section-title"">Məhsul haqqında</div>
    <p>
        Sement əsaslı, məhsulun istifadə imkanlarını artıran qatqılarla zəngin, yüksək keyfiyyətli,
        hibrid və nano texnologiyalı, kombinasiyalı polimerləşdirilmiş, birkomponentli,
        boz rəngli keramika yapışdırıcısıdır.
    </p>
    <p>
        EN 12004-1 standartının C1TE sinfinin tələblərinə uyğundur.
    </p>

    <div class=""section-title"">İstifadə sahələri</div>
    <p>Bu keramika yapışdırıcısı:</p>
    <ul>
        <li>daxili və xarici məkanlarda;</li>
        <li>kiçik və orta ölçülü üzlük materialların beton, suvaq, döşəmə suvağı kimi səthlərin üzərinə yapışdırılmasında istifadə olunur.</li>
    </ul>

    <div class=""section-title"">Üstünlükləri</div>
    <ul>
        <li>Rahat hazırlanır və tətbiq olunur;</li>
        <li>Şaquli və üfüqi səthlərdə sürüşmə ehtimalı yoxdur;</li>
        <li>Yuxarıdan aşağıya doğru keramika, şüşə mozaika, şüşə bloklar və digər üzlük plitələri yapışdırmaq mümkündür;</li>
        <li>Şaxtaya və istiliyə davamlıdır.</li>
    </ul>

    <div class=""section-title"">İstifadə qaydaları</div>
    <p>
        <b>Səthin hazırlanması.</b> İstifadə olunacaq səthlərin sağlam, quru, təmiz və ölçülərinin düzgün
        olmasına diqqət etməli, səth yapışma müqavimətini zəiflədən hər növ yağ, parafin və digər
        qalıqlardan tamamilə təmizlənməlidir. Səthdə 5–6 mm dərinliyində çatlar və ya boşluqlar
        olarsa, onlar bir gün əvvəl Fikret təmir qarışığı və ya digər oxşar məhsulla doldurulmalıdır.
        Tətbiq olunacaq səth eyni zamanda əvvəldən nəmləndirilməli, ROKOL Binder Astar və ya digər
        oxşar məhsulla astarlanmalıdır.
    </p>

    <div class=""sub-title"">Qarışığın hazırlanması</div>
    <p>
        25 kq-lıq quru qarışığı ehmalca 6,0 – 6,5 litr suyun üzərinə boşaldıb kürəciklər yox olana qədər
        qarışdırın. Qarışığın özünü tutması üçün 3–5 dəqiqə gözləyin, istifadəyə başlamazdan əvvəl
        yenidən qarışdırın.
    </p>

    <div class=""sub-title"">Tətbiqi</div>
    <p>
        Malanın düz tərəfi ilə qarışığı nazik qatla səthə yayın. Sonra plitələrin ölçüsünə uyğun olaraq
        səthə vurulmuş qarışığı malanın dişli-daraqlı tərəfi ilə daralayın. İri ölçülü plitələrin
        yapışdırılmasında kombinə edilmiş yapışdırma üsulundan (bu üsulla yapışdırıcı həm səthə çəkilir
        və daraqlanır, həm də üzlük plitənin arxasına yaxılaraq daraqlanmış səthə yapışdırılır)
        istifadə olunması tövsiyə olunur.
    </p>
    <p>
        Plitənin yapışdırılma müddətini səthin üzərinə çəkilmiş qarışığa barmaqla azacıq toxunmaqla
        (basmaqla) müəyyən edin. Qarışıq barmağa yapışarsa, o zaman plitələri səthə yapışdırmaq olar.
        Əgər səthin üzərinə çəkilmiş qarışıq üz bağlayarsa, o zaman onu sıyırıb atmalı və həmin səthin
        üzərinə yenidən qarışıq vuraraq daraqlı mala ilə daraqladıqdan sonra plitələr yapışdırılmalıdır.
    </p>
    <p>
        Hazırlanan qarışıq 2 saat ərzində işlədilməlidir. İstifadə müddəti keçmiş və ya qabıq bağlamış
        qarışığı istifadə etmək məsləhət görülmür. Aralıq doldurma işi divarda 24 saatdan, döşəmədə isə
        48 saatdan sonra icra edilməlidir.
    </p>

    <div class=""sub-title"">Tövsiyələr</div>
    <p>
        “Matanat A” HYBRID keramika yapışdırıcısı istifadə olunarkən mühitin temperaturu +5°C-dən aşağı
        və +30°C-dən yuxarıdırsa, lazımi temperatur təmin olunmalıdır. İsti, yağışlı və küləkli
        havalarda istifadə olunmamalıdır. Su hopması yüksək olan keramika istifadə edəcəksinizsə,
        plitələri tətbiqdən öncə su ilə doyurulmalıdır. İstifadə müddəti keçmiş qarışığa qətiyyən su
        və ya yapışdırıcı əlavə edilməməlidir. İstifadədən sonra bütün alət və ləvazimatlar su ilə
        təmizlənməlidir.
    </p>

    <div class=""section-title"">Texniki göstəricilər</div>
    <table>
        <tr>
            <th>Göstərici</th>
            <th>Məlumat</th>
        </tr>
        <tr>
            <td>Rəngi</td>
            <td>boz</td>
        </tr>
        <tr>
            <td>İstifadə temperaturu</td>
            <td>+5° C - +30° C</td>
        </tr>
        <tr>
            <td>Su/quru qarışıq</td>
            <td>
                2,4–2,6 litr su / 10 kq quru qarışıq<br>
                6,0–6,5 litr su / 25 kq quru qarışıq
            </td>
        </tr>
        <tr>
            <td>Qarışdırıldıqdan sonra gözləmə müddəti</td>
            <td>3–5 dəqiqə</td>
        </tr>
        <tr>
            <td>İstifadə müddəti</td>
            <td>2 saat</td>
        </tr>
        <tr>
            <td>Aralıq doldurma işinə başlama vaxtı</td>
            <td>
                divarda — 24 saatdan sonra<br>
                döşəmədə — 48 saatdan sonra
            </td>
        </tr>
        <tr>
            <td>Qablaşdırma</td>
            <td>2 və ya 3 qat kraft kağızı və 1 qatlı polietilendən ibarət olan 10 və 25 kq-lıq kisələrdə</td>
        </tr>
    </table>

    <table>
        <tr>
            <th></th>
            <th>EN 12004-1 standartının tələblərinə uyğun olaraq</th>
            <th>Test nəticələri</th>
        </tr>
        <tr>
            <td>Qabıq bağlama müddəti</td>
            <td>≥30 dəqiqə</td>
            <td>30–40 dəqiqə</td>
        </tr>
        <tr>
            <td>Sürüşmə</td>
            <td>&lt;0,5 mm</td>
            <td>0</td>
        </tr>
        <tr>
            <td>Qopma müqaviməti</td>
            <td>&gt;0,5 N/mm²</td>
            <td>1–1,5 N/mm²</td>
        </tr>
    </table>

    <div class=""section-title"">Tövsiyə olunan daraqlı malanın dişlərinin ölçüləri və ölçülərə uyğun yapışdırıcının yayılma qalınlığı</div>
    <table>
        <tr>
            <th>Üzlük plitələrin ölçülərinə uyğun sahəsi</th>
            <th>Daraqlı malanın təsviri və dişlərinin ölçüləri, mm</th>
            <th>Yapışdırıcının yayılma qalınlığı, mm</th>
        </tr>
        <tr>
            <td>&lt;25 sm²</td>
            <td>3</td>
            <td>1–2</td>
        </tr>
        <tr>
            <td>25–100 sm²</td>
            <td>4</td>
            <td>2–3</td>
        </tr>
        <tr>
            <td>100–600 sm²</td>
            <td>6</td>
            <td>3–4</td>
        </tr>
        <tr>
            <td>600–1800 sm²</td>
            <td>8</td>
            <td>4–5</td>
        </tr>
        <tr>
            <td>1800 sm²</td>
            <td>10</td>
            <td>5–6</td>
        </tr>
    </table>

    <div class=""section-title"">Sərfiyyat</div>
    <p>
        İstifadə zamanı yapışdırıcının səthə çəkilmiş qalınlığından və dişli-daraqlı malanın dişlərinin
        ölçülərindən asılı olaraq 1 m²-ə sərf olunan quru qarışığın miqdarı (kq-la):
    </p>

    <table>
        <tr>
            <th>“Matanət A” HYBRID keramika yapışdırıcısı (boz) (kq)</th>
            <th>1 mm qalınlıq üçün quru qarışığın sərfiyyatı (kq/m²)</th>
            <th>Qarışdırılacaq suyun miqdarı (litr) — 25 kq üçün</th>
            <th>3 mm</th>
            <th>4 mm</th>
            <th>6 mm</th>
            <th>8 mm</th>
            <th>10 mm</th>
        </tr>
        <tr>
            <td>25</td>
            <td>1,28</td>
            <td>6–6,5</td>
            <td>1,3 kq</td>
            <td>2,6 kq</td>
            <td>3,9 kq</td>
            <td>5,1 kq</td>
            <td>6,4 kq</td>
        </tr>
    </table>

    <div class=""section-title"">Saxlama müddəti və qaydası</div>
    <p>
        Açılmamış kisələrdə, quru şəraitdə, üst-üstə maksimum 11 sıra olmaqla 12 ay saxlanıla bilər.
    </p>

    <div class=""section-title"">Xəbərdarlıqlar</div>
    <ul>
        <li>yapışdırıcının sement əsaslı və qatqılarla zəngin olduğunu nəzərə alaraq dəriyə və ya gözə düşdükdə dərhal su ilə yuyun;</li>
        <li>istifadə qaydalarında qeyd edilməyən heç bir əlavə maddə qatmayın;</li>
        <li>yuxarıdakı göstəricilər +23°C±2°C temperatur və 50% + 5% rütubətli şəraitdə aparılan laboratoriya sınaqları nəticəsində əldə olunmuşdur. Şəraitdən asılı olaraq bu göstəricilər dəyişə bilər.</li>
    </ul>

</body>
</html>"
                    });
                    #endregion
                    #region FourthProduct
                    sub.Products.Add(new ProductItem
                    {
                        Name = "Keramika yapıştırıcısı",
                        Description = data.Title,
                        ImageUrl = "plite4.jpg",
                        Price = 30,
                        AboutTheProduct = @"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <style>
        body {
            font-family: Arial, Helvetica, sans-serif;
            font-size: 14px;
            color: #243B53;
            line-height: 1.6;
            margin: 0;
            padding: 0;
            background-color: #ffffff;
        }

        .section-title {
            background: #EAEAEA;
            color: #243B53;
            font-weight: bold;
            font-size: 18px;
            text-transform: uppercase;
            padding: 10px 14px;
            margin: 18px 0 10px 0;
        }

        .sub-title {
            background: #EFEFEF;
            color: #243B53;
            font-weight: bold;
            font-size: 16px;
            padding: 8px 12px;
            margin: 16px 0 10px 0;
        }

        p {
            margin: 0 0 12px 0;
        }

        ul {
            margin: 0 0 12px 20px;
            padding: 0;
        }

        li {
            margin-bottom: 6px;
        }

        table {
            width: 100%;
            border-collapse: collapse;
            margin: 12px 0 18px 0;
            font-size: 14px;
        }

        th, td {
            border: 1px solid #A7B3C2;
            padding: 10px;
            vertical-align: top;
            text-align: left;
        }

        th {
            background-color: #F1F1F1;
            font-weight: bold;
        }

        .note {
            margin-top: 10px;
        }
    </style>
</head>
<body>

    <div class=""section-title"">Məhsul haqqında</div>
    <p>
        Sement əsaslı, məhsulun istifadə imkanlarını artıran qatqılarla zəngin, yüksək keyfiyyətli,
        hibrid və nano texnologiyalı, kombinasiyalı polimerləşdirilmiş, birkomponentli,
        boz rəngli keramika yapışdırıcısıdır.
    </p>
    <p>
        EN 12004-1 standartının C1TE sinfinin tələblərinə uyğundur.
    </p>

    <div class=""section-title"">İstifadə sahələri</div>
    <p>Bu keramika yapışdırıcısı:</p>
    <ul>
        <li>daxili və xarici məkanlarda;</li>
        <li>kiçik və orta ölçülü üzlük materialların beton, suvaq, döşəmə suvağı kimi səthlərin üzərinə yapışdırılmasında istifadə olunur.</li>
    </ul>

    <div class=""section-title"">Üstünlükləri</div>
    <ul>
        <li>Rahat hazırlanır və tətbiq olunur;</li>
        <li>Şaquli və üfüqi səthlərdə sürüşmə ehtimalı yoxdur;</li>
        <li>Yuxarıdan aşağıya doğru keramika, şüşə mozaika, şüşə bloklar və digər üzlük plitələri yapışdırmaq mümkündür;</li>
        <li>Şaxtaya və istiliyə davamlıdır.</li>
    </ul>

    <div class=""section-title"">İstifadə qaydaları</div>
    <p>
        <b>Səthin hazırlanması.</b> İstifadə olunacaq səthlərin sağlam, quru, təmiz və ölçülərinin düzgün
        olmasına diqqət etməli, səth yapışma müqavimətini zəiflədən hər növ yağ, parafin və digər
        qalıqlardan tamamilə təmizlənməlidir. Səthdə 5–6 mm dərinliyində çatlar və ya boşluqlar
        olarsa, onlar bir gün əvvəl Fikret təmir qarışığı və ya digər oxşar məhsulla doldurulmalıdır.
        Tətbiq olunacaq səth eyni zamanda əvvəldən nəmləndirilməli, ROKOL Binder Astar və ya digər
        oxşar məhsulla astarlanmalıdır.
    </p>

    <div class=""sub-title"">Qarışığın hazırlanması</div>
    <p>
        25 kq-lıq quru qarışığı ehmalca 6,0 – 6,5 litr suyun üzərinə boşaldıb kürəciklər yox olana qədər
        qarışdırın. Qarışığın özünü tutması üçün 3–5 dəqiqə gözləyin, istifadəyə başlamazdan əvvəl
        yenidən qarışdırın.
    </p>

    <div class=""sub-title"">Tətbiqi</div>
    <p>
        Malanın düz tərəfi ilə qarışığı nazik qatla səthə yayın. Sonra plitələrin ölçüsünə uyğun olaraq
        səthə vurulmuş qarışığı malanın dişli-daraqlı tərəfi ilə daralayın. İri ölçülü plitələrin
        yapışdırılmasında kombinə edilmiş yapışdırma üsulundan (bu üsulla yapışdırıcı həm səthə çəkilir
        və daraqlanır, həm də üzlük plitənin arxasına yaxılaraq daraqlanmış səthə yapışdırılır)
        istifadə olunması tövsiyə olunur.
    </p>
    <p>
        Plitənin yapışdırılma müddətini səthin üzərinə çəkilmiş qarışığa barmaqla azacıq toxunmaqla
        (basmaqla) müəyyən edin. Qarışıq barmağa yapışarsa, o zaman plitələri səthə yapışdırmaq olar.
        Əgər səthin üzərinə çəkilmiş qarışıq üz bağlayarsa, o zaman onu sıyırıb atmalı və həmin səthin
        üzərinə yenidən qarışıq vuraraq daraqlı mala ilə daraqladıqdan sonra plitələr yapışdırılmalıdır.
    </p>
    <p>
        Hazırlanan qarışıq 2 saat ərzində işlədilməlidir. İstifadə müddəti keçmiş və ya qabıq bağlamış
        qarışığı istifadə etmək məsləhət görülmür. Aralıq doldurma işi divarda 24 saatdan, döşəmədə isə
        48 saatdan sonra icra edilməlidir.
    </p>

    <div class=""sub-title"">Tövsiyələr</div>
    <p>
        “Matanat A” HYBRID keramika yapışdırıcısı istifadə olunarkən mühitin temperaturu +5°C-dən aşağı
        və +30°C-dən yuxarıdırsa, lazımi temperatur təmin olunmalıdır. İsti, yağışlı və küləkli
        havalarda istifadə olunmamalıdır. Su hopması yüksək olan keramika istifadə edəcəksinizsə,
        plitələri tətbiqdən öncə su ilə doyurulmalıdır. İstifadə müddəti keçmiş qarışığa qətiyyən su
        və ya yapışdırıcı əlavə edilməməlidir. İstifadədən sonra bütün alət və ləvazimatlar su ilə
        təmizlənməlidir.
    </p>

    <div class=""section-title"">Texniki göstəricilər</div>
    <table>
        <tr>
            <th>Göstərici</th>
            <th>Məlumat</th>
        </tr>
        <tr>
            <td>Rəngi</td>
            <td>boz</td>
        </tr>
        <tr>
            <td>İstifadə temperaturu</td>
            <td>+5° C - +30° C</td>
        </tr>
        <tr>
            <td>Su/quru qarışıq</td>
            <td>
                2,4–2,6 litr su / 10 kq quru qarışıq<br>
                6,0–6,5 litr su / 25 kq quru qarışıq
            </td>
        </tr>
        <tr>
            <td>Qarışdırıldıqdan sonra gözləmə müddəti</td>
            <td>3–5 dəqiqə</td>
        </tr>
        <tr>
            <td>İstifadə müddəti</td>
            <td>2 saat</td>
        </tr>
        <tr>
            <td>Aralıq doldurma işinə başlama vaxtı</td>
            <td>
                divarda — 24 saatdan sonra<br>
                döşəmədə — 48 saatdan sonra
            </td>
        </tr>
        <tr>
            <td>Qablaşdırma</td>
            <td>2 və ya 3 qat kraft kağızı və 1 qatlı polietilendən ibarət olan 10 və 25 kq-lıq kisələrdə</td>
        </tr>
    </table>

    <table>
        <tr>
            <th></th>
            <th>EN 12004-1 standartının tələblərinə uyğun olaraq</th>
            <th>Test nəticələri</th>
        </tr>
        <tr>
            <td>Qabıq bağlama müddəti</td>
            <td>≥30 dəqiqə</td>
            <td>30–40 dəqiqə</td>
        </tr>
        <tr>
            <td>Sürüşmə</td>
            <td>&lt;0,5 mm</td>
            <td>0</td>
        </tr>
        <tr>
            <td>Qopma müqaviməti</td>
            <td>&gt;0,5 N/mm²</td>
            <td>1–1,5 N/mm²</td>
        </tr>
    </table>

    <div class=""section-title"">Tövsiyə olunan daraqlı malanın dişlərinin ölçüləri və ölçülərə uyğun yapışdırıcının yayılma qalınlığı</div>
    <table>
        <tr>
            <th>Üzlük plitələrin ölçülərinə uyğun sahəsi</th>
            <th>Daraqlı malanın təsviri və dişlərinin ölçüləri, mm</th>
            <th>Yapışdırıcının yayılma qalınlığı, mm</th>
        </tr>
        <tr>
            <td>&lt;25 sm²</td>
            <td>3</td>
            <td>1–2</td>
        </tr>
        <tr>
            <td>25–100 sm²</td>
            <td>4</td>
            <td>2–3</td>
        </tr>
        <tr>
            <td>100–600 sm²</td>
            <td>6</td>
            <td>3–4</td>
        </tr>
        <tr>
            <td>600–1800 sm²</td>
            <td>8</td>
            <td>4–5</td>
        </tr>
        <tr>
            <td>1800 sm²</td>
            <td>10</td>
            <td>5–6</td>
        </tr>
    </table>

    <div class=""section-title"">Sərfiyyat</div>
    <p>
        İstifadə zamanı yapışdırıcının səthə çəkilmiş qalınlığından və dişli-daraqlı malanın dişlərinin
        ölçülərindən asılı olaraq 1 m²-ə sərf olunan quru qarışığın miqdarı (kq-la):
    </p>

    <table>
        <tr>
            <th>“Matanət A” HYBRID keramika yapışdırıcısı (boz) (kq)</th>
            <th>1 mm qalınlıq üçün quru qarışığın sərfiyyatı (kq/m²)</th>
            <th>Qarışdırılacaq suyun miqdarı (litr) — 25 kq üçün</th>
            <th>3 mm</th>
            <th>4 mm</th>
            <th>6 mm</th>
            <th>8 mm</th>
            <th>10 mm</th>
        </tr>
        <tr>
            <td>25</td>
            <td>1,28</td>
            <td>6–6,5</td>
            <td>1,3 kq</td>
            <td>2,6 kq</td>
            <td>3,9 kq</td>
            <td>5,1 kq</td>
            <td>6,4 kq</td>
        </tr>
    </table>

    <div class=""section-title"">Saxlama müddəti və qaydası</div>
    <p>
        Açılmamış kisələrdə, quru şəraitdə, üst-üstə maksimum 11 sıra olmaqla 12 ay saxlanıla bilər.
    </p>

    <div class=""section-title"">Xəbərdarlıqlar</div>
    <ul>
        <li>yapışdırıcının sement əsaslı və qatqılarla zəngin olduğunu nəzərə alaraq dəriyə və ya gözə düşdükdə dərhal su ilə yuyun;</li>
        <li>istifadə qaydalarında qeyd edilməyən heç bir əlavə maddə qatmayın;</li>
        <li>yuxarıdakı göstəricilər +23°C±2°C temperatur və 50% + 5% rütubətli şəraitdə aparılan laboratoriya sınaqları nəticəsində əldə olunmuşdur. Şəraitdən asılı olaraq bu göstəricilər dəyişə bilər.</li>
    </ul>

</body>
</html>"
                    });
                    #endregion
                }
                else
                {
                    sub.Products.Add(new ProductItem
                    {
                        Name = $"{ subName} - Məhsul 1",
                        Description = data.Title,
                        ImageUrl = "product.png",
                        Price = 25
                    });

                    sub.Products.Add(new ProductItem
                    {
                        Name = $"{subName} - Məhsul 2",
                        Description = data.Title,
                        ImageUrl = "product.png",
                        Price = 20
                    });
                }

                root.SubCategories.Add(sub);
            }

            RootCategories.Add(root);
        }
    }

    private void ExpandRootCategory(string key)
    {
        var target = RootCategories.FirstOrDefault(x =>
            string.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase));

        if (target == null)
            return;

        foreach (var cat in RootCategories.Where(x => x != target))
        {
            cat.IsExpanded = false;

            foreach (var sub in cat.SubCategories)
                sub.IsExpanded = false;
        }

        target.IsExpanded = true;
    }

    private static IEnumerable<string> GetRootCategoryOrder()
    {
        return new[] { "INSAAT", "FASAD", "YER", "QATQI" };
    }

    private static string GetCategoryImage(string key)
    {
        return key switch
        {
            "INSAAT" => "product_type3",
            "FASAD" => "product_type2",
            "YER" => "product_type1",
            "QATQI" => "product_type4",
            _ => "product.png"
        };
    }

    [RelayCommand]
    private void ToggleRootCategory(ProductRootCategorySection section)
    {
        if (section == null)
            return;

        var willExpand = !section.IsExpanded;

        foreach (var cat in RootCategories.Where(x => x != section))
        {
            cat.IsExpanded = false;

            foreach (var sub in cat.SubCategories)
                sub.IsExpanded = false;
        }

        section.IsExpanded = willExpand;

        if (!willExpand)
        {
            foreach (var sub in section.SubCategories)
                sub.IsExpanded = false;
        }
    }

    [RelayCommand]
    private void ToggleSubCategory(ProductSubCategorySection section)
    {
        if (section == null)
            return;

        var parent = RootCategories.FirstOrDefault(x => x.SubCategories.Contains(section));
        if (parent == null)
            return;

        var willExpand = !section.IsExpanded;

        foreach (var sub in parent.SubCategories.Where(x => x != section))
            sub.IsExpanded = false;

        section.IsExpanded = willExpand;
    }

    [RelayCommand]
    private void ClearSearch()
    {
        SearchText = string.Empty;
    }

    [RelayCommand]
    private async Task SelectProductAsync(ProductItem item)
    {
        if (item == null)
            return;

        //await Application.Current.MainPage.DisplayAlert(
        //    "Məhsul",
        //    $"{item.Name}\nQiymət: {item.Price:0.##} ₼",
        //    "OK");


        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Shell.Current.GoToAsync($"//{nameof(ProductDetailPage)}", new Dictionary<string, object>
            {
                ["ProductItem"] = item
            });
        });
    }
}