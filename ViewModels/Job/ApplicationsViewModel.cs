using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using MetanetA_MobileApp.Model;

namespace MetanetA_MobileApp.ViewModels.Job
{
    public partial class ApplicationsViewModel : ObservableObject
    {
        public ObservableCollection<ApplicationModel> Applications { get; } = new();

        public string TotalText => $"{Applications.Count} total";
        public string AttentionText => $"{Applications.Count(x => x.NeedsAttention)} need attention";

        public ApplicationsViewModel()
        {
            AddSampleData();
        }

        private void AddSampleData()
        {
            AddApplication(new ApplicationModel
            {
                LogoLetter = "L",
                Company = "Linear",
                Title = "Senior Product Designer",
                AppliedText = "Applied Apr 10",
                Status = "Interview",
                Note = "Portfolio review · Apr 25, 4:00 PM",
                NeedsAttention = true,
                CardStrokeColor = "#BFC7FF",
                StatusBackgroundColor = "#EEF1FF",
                StatusTextColor = "#263DB7"
            });

            AddApplication(new ApplicationModel
            {
                LogoLetter = "S",
                Company = "Stripe",
                Title = "Frontend Engineer, React",
                AppliedText = "Applied Apr 15",
                Status = "Screening",
                Note = "",
                CardStrokeColor = "#E2E6EF",
                StatusBackgroundColor = "#FFF5DF",
                StatusTextColor = "#111827"
            });

            AddApplication(new ApplicationModel
            {
                LogoLetter = "N",
                Company = "Notion",
                Title = "Product Manager, Growth",
                AppliedText = "Applied Mar 28",
                Status = "Offer",
                Note = "Offer expires Apr 30",
                NeedsAttention = true,
                CardStrokeColor = "#BFC7FF",
                StatusBackgroundColor = "#FFE8EA",
                StatusTextColor = "#FF4F62"
            });
        }

        public void AddApplication(ApplicationModel item)
        {
            Applications.Add(item);
            OnPropertyChanged(nameof(TotalText));
            OnPropertyChanged(nameof(AttentionText));
        }
    }

}
