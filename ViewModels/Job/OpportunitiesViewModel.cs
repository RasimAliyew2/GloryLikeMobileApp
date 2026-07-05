using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MetanetA_MobileApp.Model;
using MetanetA_MobileApp.Services;
using MetanetA_MobileApp.Services.Abstractions;
using MetanetA_MobileApp.Services.UIState;

namespace MetanetA_MobileApp.ViewModels.Job;

public partial class OpportunitiesViewModel : BaseViewModel
{
    private readonly IJobOffersApiService _jobOffersApiService;
    private readonly IUserSession _userSession;
    private bool _isLoaded;

    public ObservableCollection<OpportunityItem> Opportunities { get; } = new();

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string? errorMessage;

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public bool HasOpportunities => Opportunities.Count > 0;

    public bool HasNoOpportunities => !HasOpportunities && !IsBusy && !HasError;

    public string RolesCountText => $"{Opportunities.Count} roles for you";

    public OpportunitiesViewModel(
        BottomMenuState menuState,
        IJobOffersApiService jobOffersApiService,
        IUserSession userSession)
        : base(menuState)
    {
        _jobOffersApiService = jobOffersApiService;
        _userSession = userSession;
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (_isLoaded)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = null;
            Opportunities.Clear();

            var jobOffers = await _jobOffersApiService.GetJobOffersAsync();
            var builtOpportunities = BuildOpportunities(jobOffers);

            foreach (var opportunity in builtOpportunities)
                Opportunities.Add(opportunity);

            _isLoaded = true;
            RefreshCollectionState();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        _isLoaded = false;
        await LoadAsync();
    }

    [RelayCommand]
    private void ToggleOpportunity(OpportunityItem? item)
    {
        if (item is null)
            return;

        item.IsExpanded = !item.IsExpanded;
        RefreshItem(item);
    }

    [RelayCommand]
    private void ToggleSave(OpportunityItem? item)
    {
        if (item is null)
            return;

        item.IsSaved = !item.IsSaved;
        RefreshItem(item);
    }

    [RelayCommand]
    private async Task ApplyAsync(OpportunityItem? item)
    {
        if (item is null)
            return;

        await Shell.Current.DisplayAlert(
            "Apply",
            $"{item.Title} üçün müraciət edildi.",
            "OK");
    }

    private List<OpportunityItem> BuildOpportunities(List<JobOfferApiItem> jobOffers)
    {
        var selectedJobName = _userSession.CurrentUser?.Job?.JobName;
        var candidateSkills = BuildCandidateSkillScoreMap();

        var groups = jobOffers
            .Where(x => !string.IsNullOrWhiteSpace(x.RequiredJob))
            .Where(x => !string.IsNullOrWhiteSpace(x.Skills))
            .Where(x => x.SkillsWeight > 0)
            .Where(x => string.IsNullOrWhiteSpace(selectedJobName) ||
                        Normalize(x.RequiredJob) == Normalize(selectedJobName))
            .GroupBy(x => new
            {
                RequiredJob = x.RequiredJob.Trim(),
                Name = x.Name,
                Description = x.Description,
                Seniority = string.IsNullOrWhiteSpace(x.Seniority) ? "Middle" : x.Seniority.Trim()
            })
            .ToList();

        var result = new List<OpportunityItem>();
        var index = 0;

        foreach (var group in groups)
        {
            var requiredSkills = group
                .Where(x => !string.IsNullOrWhiteSpace(x.Skills) && x.SkillsWeight > 0)
                .Select(x => new RequiredSkillTemplate
                {
                    SkillName = x.Skills.Trim(),
                    Weight = x.SkillsWeight
                })
                .GroupBy(x => Normalize(x.SkillName))
                .Select(g => new RequiredSkillTemplate
                {
                    SkillName = g.First().SkillName,
                    Weight = g.Max(x => x.Weight)
                })
                .ToList();

            if (requiredSkills.Count == 0)
                continue;

            var score = CalculateRoleReadiness(requiredSkills, candidateSkills);
            var matchedSkills = requiredSkills
                .Where(x => candidateSkills.ContainsKey(Normalize(x.SkillName)))
                .Select(x => x.SkillName)
                .OrderBy(x => x)
                .ToList();

            var missingSkills = requiredSkills
                .Where(x => !candidateSkills.ContainsKey(Normalize(x.SkillName)))
                .Select(x => x.SkillName)
                .OrderBy(x => x)
                .ToList();

            result.Add(new OpportunityItem
            {
                
                LogoLetter = GetLogoLetter(group.Key.RequiredJob),
                Company = group.Key.RequiredJob,
                PostedAgo = index == 0 ? "2d" : $"{Math.Min(index + 1, 7)}d",
                Title = group.Key.Name,// BuildTitle(group.Key.RequiredJob, group.Key.Seniority),
                Location = "Role template ·",
                WorkType = "Role",
                Level = group.Key.Seniority,
                Salary = $"{requiredSkills.Count} skills",
                Score = score,
                ScoreColor = GetScoreColor(score),
                IsExpanded = index == 0,
       
                AboutRole = group.Key.Description,// BuildAboutRole(group.Key.RequiredJob, group.Key.Seniority, requiredSkills.Count),
                Responsibilities = BuildResponsibilities(requiredSkills),
                MatchedSkills = matchedSkills.Count == 0 ? "No matched skills yet" : string.Join(", ", matchedSkills),
                MissingSkills = missingSkills.Count == 0 ? "No missing required skills" : string.Join(", ", missingSkills.Take(8)),
                MatchNote = BuildMatchNote(score, matchedSkills.Count, requiredSkills.Count)
            });

            index++;
        }

        return result
            .OrderByDescending(x => x.Score)
            .ThenBy(x => x.Title)
            .ToList();
    }

    private Dictionary<string, double> BuildCandidateSkillScoreMap()
    {
        var selectedSkills = _userSession.CurrentUser?.SelectedSkills ?? new List<UserSkillInfo>();

        return selectedSkills
            .Where(x => !string.IsNullOrWhiteSpace(x.SkillName))
            .GroupBy(x => Normalize(x.SkillName))
            .ToDictionary(
                g => g.Key,
                g => g.Max(x => GetCandidateSkillScore(x)),
                StringComparer.OrdinalIgnoreCase);
    }

    private static double GetCandidateSkillScore(UserSkillInfo skill)
    {
        if (skill.DepthScore > 0)
            return Math.Clamp(skill.DepthScore, 0, 100);

        if (skill.KnowledgeScore > 0)
            return Math.Clamp(skill.KnowledgeScore, 0, 100);

        if (skill.Depth > 0)
            return Math.Clamp(skill.Depth, 0, 100);

        return 0;
    }

    private static int CalculateRoleReadiness(
        List<RequiredSkillTemplate> requiredSkills,
        Dictionary<string, double> candidateSkills)
    {
        var denominator = requiredSkills.Sum(x => x.Weight);

        if (denominator <= 0)
            return 0;

        var numerator = requiredSkills.Sum(x =>
        {
            var key = Normalize(x.SkillName);
            var candidateScore = candidateSkills.TryGetValue(key, out var value) ? value : 0d;
            return x.Weight * candidateScore;
        });

        var readiness = numerator / denominator;

        // Round half up: 40.5 -> 41.
        return (int)Math.Clamp(Math.Floor(readiness + 0.5d), 0, 100);
    }

    private static string Normalize(string value)
    {
        return value.Trim().ToLowerInvariant();
    }

    private static string GetLogoLetter(string requiredJob)
    {
        var trimmed = requiredJob.Trim();
        return string.IsNullOrWhiteSpace(trimmed)
            ? "J"
            : trimmed[0].ToString().ToUpperInvariant();
    }

    private static string BuildTitle(string requiredJob, string seniority)
    {
        return string.IsNullOrWhiteSpace(seniority)
            ? requiredJob
            : $"{seniority} {requiredJob}";
    }

    private static string GetScoreColor(int score)
    {
        return score switch
        {
            >= 85 => "#10B981",
            >= 70 => "#6D5EF2",
            >= 50 => "#F59E0B",
            _ => "#EF4444"
        };
    }

    private static string BuildAboutRole(string requiredJob, string seniority, int skillCount)
    {
        return $"No Description for noü";
    }

    private static string BuildResponsibilities(List<RequiredSkillTemplate> requiredSkills)
    {
        var topSkills = requiredSkills
            .OrderByDescending(x => x.Weight)
            .Take(5)
            .Select(x => $"• {x.SkillName} — weight {x.Weight}");

        return string.Join("\n", topSkills);
    }

    private static string BuildMatchNote(int score, int matchedCount, int requiredCount)
    {
        if (requiredCount <= 0)
            return "This role has no required skills and was excluded from scoring.";

        return $"Role readiness is {score}%. Matched {matchedCount} of {requiredCount} required skills. The score uses Σ(wᵢ × sᵢ) / Σ(wᵢ).";
    }

    private void RefreshItem(OpportunityItem item)
    {
        var index = Opportunities.IndexOf(item);
        if (index < 0)
            return;

        Opportunities[index] = item;
    }

    private void RefreshCollectionState()
    {
        OnPropertyChanged(nameof(RolesCountText));
        OnPropertyChanged(nameof(HasOpportunities));
        OnPropertyChanged(nameof(HasNoOpportunities));
    }

    partial void OnErrorMessageChanged(string? value)
    {
        OnPropertyChanged(nameof(HasError));
        OnPropertyChanged(nameof(HasNoOpportunities));
    }

    partial void OnIsBusyChanged(bool value)
    {
        OnPropertyChanged(nameof(HasNoOpportunities));
    }

    private class RequiredSkillTemplate
    {
        public string SkillName { get; set; } = string.Empty;
        public int Weight { get; set; }
    }
}
