using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MetanetA_MobileApp.Model;
using MetanetA_MobileApp.Services.UIState;

namespace MetanetA_MobileApp.ViewModels.Job
{
    public partial class OpportunitiesViewModel : BaseViewModel
    {
        public ObservableCollection<OpportunityItem> Opportunities { get; } = new();

        [ObservableProperty]
        private string searchText;

        public string RolesCountText => $"{Opportunities.Count} roles for you";

        public OpportunitiesViewModel(BottomMenuState menuState) : base(menuState)
        {
            AddSampleData();
        }

        private void AddSampleData()
        {
            AddOpportunity(new OpportunityItem
            {
                LogoLetter = "L",
                Company = "Linear",
                PostedAgo = "2d",
                Title = "Senior Product Designer",
                Location = "Remote · EU",
                WorkType = "Remote",
                Level = "Senior",
                Salary = "$110k – $140k",
                Score = 94,
                ScoreColor = "#09B889",
                IsExpanded = true,
                AboutRole = "Lead end-to-end product design for Linear's collaboration tools. Partner tightly with engineering and ship features used by thousands of teams.",
                Responsibilities = "• Own design end-to-end across discovery and delivery\n• Build and evolve the design system with engineering\n• Run usability sessions with power users",
                MatchedSkills = "Figma, Design Systems, Prototyping, User Research",
                MissingSkills = "Motion Design",
                MatchNote = "You match the core craft and research skills. Adding Motion Design would lift your fit by ~6%."
            });

            AddOpportunity(new OpportunityItem
            {
                LogoLetter = "S",
                Company = "Stripe",
                PostedAgo = "1d",
                Title = "Frontend Engineer, React",
                Location = "Berlin, DE",
                WorkType = "Hybrid",
                Level = "Middle",
                Salary = "€75k – €95k",
                Score = 87,
                ScoreColor = "#09B889",
                AboutRole = "Build customer-facing payment dashboards and improve frontend performance across product surfaces.",
                Responsibilities = "• Build React components\n• Improve page performance\n• Work with backend APIs",
                MatchedSkills = "React, TypeScript, UI Components",
                MissingSkills = "Payments API",
                MatchNote = "Strong frontend match. Payment domain knowledge would improve your profile."
            });
        }

        public void AddOpportunity(OpportunityItem item)
        {
            Opportunities.Add(item);
            OnPropertyChanged(nameof(RolesCountText));
        }

        [RelayCommand]
        private void ToggleOpportunity(OpportunityItem item)
        {
            if (item == null)
                return;

            item.IsExpanded = !item.IsExpanded;
            RefreshItem(item);
        }

        [RelayCommand]
        private void ToggleSave(OpportunityItem item)
        {
            if (item == null)
                return;

            item.IsSaved = !item.IsSaved;
            RefreshItem(item);
        }

        [RelayCommand]
        private async Task Apply(OpportunityItem item)
        {
            if (item == null)
                return;

            await Shell.Current.DisplayAlert("Apply", $"{item.Title} üçün müraciət edildi.", "OK");
        }

        private void RefreshItem(OpportunityItem item)
        {
            var index = Opportunities.IndexOf(item);
            if (index < 0)
                return;

            Opportunities[index] = item;
        }
    }
}