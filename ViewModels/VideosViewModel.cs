using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MetanetA_MobileApp.Model;
using MetanetA_MobileApp.Services.UIState;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace MetanetA_MobileApp.ViewModels
{
    public partial class VideosViewModel : BaseViewModel
    {
        // bütün videolar (əsas siyahı)
        public ObservableCollection<VideoItem> Videos { get; } = new();

        // UI-nin göstərəcəyi filtr olunmuş siyahı
        public ObservableCollection<VideoItem> FilteredVideos { get; } = new();

        [ObservableProperty] private VideoItem selectedVideo;

        // WebView Source
        [ObservableProperty] private WebViewSource playerSource;

        // Overlay mesaj
        [ObservableProperty] private bool showSelectHint = true;

        // Search
        [ObservableProperty] private string searchText;

        // "no result" mesajı
        [ObservableProperty] private bool showNoResults;

        public VideosViewModel(BottomMenuState menuState) : base(menuState)
        {
            Videos.Add(new VideoItem
            {
                Title = "Rokol",
                Description = "Təbiətə inteqrasiya",
                Url = "https://www.youtube.com/watch?v=6-x2cuSvdO8"
            });

            Videos.Add(new VideoItem
            {
                Title = "Rokol! Fasad boyaları!",
                Description = "Fasad üçün boya seçimləri",
                Url = "https://www.youtube.com/watch?v=RhSd3sWwGe4"
            });

            Videos.Add(new VideoItem
            {
                Title = "Rokol boya - Rəngi hiss et doya-doya",
                Description = "Rənglər və məhsullar haqqında",
                Url = "https://www.youtube.com/watch?v=-r2AN2POc6Q"
            });

            // ilk açılışda hamısını göstər
            ApplyFilter();

            // ilk videonu aç
            SelectedVideo = Videos.FirstOrDefault();
        }

        partial void OnSearchTextChanged(string value)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            FilteredVideos.Clear();

            var key = (SearchText ?? "").Trim();

            var filtered = string.IsNullOrWhiteSpace(key)
                ? Videos
                : new ObservableCollection<VideoItem>(
                    Videos.Where(v =>
                        (!string.IsNullOrWhiteSpace(v.Title) &&
                         v.Title.Contains(key, StringComparison.OrdinalIgnoreCase))
                        ||
                        (!string.IsNullOrWhiteSpace(v.Description) &&
                         v.Description.Contains(key, StringComparison.OrdinalIgnoreCase))
                    )
                  );

            foreach (var item in filtered)
                FilteredVideos.Add(item);

            ShowNoResults = !string.IsNullOrWhiteSpace(key) && FilteredVideos.Count == 0;
        }

        partial void OnSelectedVideoChanged(VideoItem value)
        {
            if (value is null)
            {
                PlayerSource = null;
                ShowSelectHint = true;
                return;
            }

            var id = ExtractYouTubeId(value.Url);
            if (string.IsNullOrWhiteSpace(id))
            {
                PlayerSource = null;
                ShowSelectHint = true;
                return;
            }

            // ✅ embed URL
            var iframeUrl =
                $"https://www.youtube-nocookie.com/embed/{id}" +
                "?playsinline=1&rel=0&modestbranding=1&autoplay=0";

            var html = $@"
<!doctype html>
<html>
<head>
  <meta name='viewport' content='width=device-width, initial-scale=1.0, maximum-scale=1.0' />
  <style>
    html,body {{ margin:0; padding:0; background:#000; height:100%; }}
    .wrap {{ position:relative; width:100%; height:100%; }}
    iframe {{ position:absolute; top:0; left:0; width:100%; height:100%; border:0; }}
  </style>
</head>
<body>
  <div class='wrap'>
    <iframe
      src='{iframeUrl}'
      allow='accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share'
      allowfullscreen>
    </iframe>
  </div>
</body>
</html>";

            PlayerSource = new HtmlWebViewSource
            {
                Html = html,
                BaseUrl = "https://www.youtube-nocookie.com"
            };

            ShowSelectHint = false;
        }

        [RelayCommand]
        private async System.Threading.Tasks.Task OpenInYouTubeAsync()
        {
            if (SelectedVideo?.Url is null)
                return;

            await Launcher.OpenAsync(SelectedVideo.Url);
        }

        private static string ExtractYouTubeId(string urlOrId)
        {
            if (string.IsNullOrWhiteSpace(urlOrId))
                return "";

            urlOrId = urlOrId.Trim();

            // artıq ID verilibsə
            if (!urlOrId.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                return urlOrId;

            var uri = new Uri(urlOrId);

            // youtu.be/ID
            if (uri.Host.Contains("youtu.be", StringComparison.OrdinalIgnoreCase))
                return uri.AbsolutePath.Trim('/');

            // youtube.com/watch?v=ID
            var query = uri.Query.TrimStart('?')
                .Split('&', StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in query)
            {
                var kv = part.Split('=', 2);
                if (kv.Length == 2 && kv[0] == "v")
                    return Uri.UnescapeDataString(kv[1]);
            }

            // /shorts/ID və ya /embed/ID
            var seg = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < seg.Length - 1; i++)
            {
                if (seg[i] == "shorts" || seg[i] == "embed")
                    return seg[i + 1];
            }

            return "";
        }
    }
}
