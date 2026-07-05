using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MetanetA_MobileApp.Model;
using MetanetA_MobileApp.Services;
using MetanetA_MobileApp.Services.Abstractions;
using MetanetA_MobileApp.Services.Cart;
using MetanetA_MobileApp.Services.Sales;
using MetanetA_MobileApp.Services.UIState;
using MetanetA_MobileApp.View.Profile;

namespace MetanetA_MobileApp.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly SalesCatalogService _catalog;
    private readonly IUserSession _userSession;
    private readonly UserInfo _fallbackUserInfo;

    [ObservableProperty]
    private CartState cart;

    [ObservableProperty]
    private CartService cartService;

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private bool isSearchResultsVisible;

    // Bind this to the big number on MainPage.
    // Example: <Label Text="{Binding OverallScoreText}" />
    // It returns only the number, not "%", so your existing "/100" label can stay.
    [ObservableProperty]
    private string overallScoreText = "0";

    [ObservableProperty]
    private int overallScoreValue;

    public string OverallScorePercentText => $"{OverallScoreValue}%";

    partial void OnOverallScoreValueChanged(int value)
    {
        OnPropertyChanged(nameof(OverallScorePercentText));
    }

    [ObservableProperty]
    private string strongestRoleName = "Score";

    [ObservableProperty]
    private string improvementHintText = string.Empty;

    [ObservableProperty]
    private bool hasOverallScore;

    public ObservableCollection<SalesItem> SearchResults { get; } = new();

    public MainViewModel(
        BottomMenuState menuState,
        CartState cart,
        CartService cartService,
        SalesCatalogService catalog,
        IUserSession userSession,
        UserInfo userInfo) : base(menuState)
    {
        _userSession = userSession;
        _fallbackUserInfo = userInfo;

        Cart = cart;
        CartService = cartService;
        _catalog = catalog;

        _userSession.CurrentUser ??= userInfo;

        _catalog.Products.CollectionChanged += Products_CollectionChanged;

        RefreshScore();
    }

    [RelayCommand]
    public void RefreshScore()
    {
        var currentUser = _userSession.CurrentUser ?? _fallbackUserInfo;
        var result = CalculateOverallScore(currentUser);

        HasOverallScore = result.HasScore;
        OverallScoreValue = result.Score;
        OverallScoreText = result.Score.ToString();
        StrongestRoleName = result.StrongestRoleName;
        ImprovementHintText = result.ImprovementHintText;
    }

    private void Products_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        ApplyMainSearch();
    }

    partial void OnSearchTextChanged(string value)
    {
        ApplyMainSearch();
    }

    private void ApplyMainSearch()
    {
        SearchResults.Clear();

        var key = (SearchText ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(key))
        {
            IsSearchResultsVisible = false;
            return;
        }

        var normalizedKey = Normalize(key);

        var results = _catalog.Products
            .Where(product => product != null && !string.IsNullOrWhiteSpace(product.Name))
            .Select(product => new
            {
                Product = product,
                Score = SearchScore(Normalize(product.Name), normalizedKey)
            })
            .Where(x => x.Score < 1000)
            .OrderBy(x => x.Score)
            .ThenBy(x => x.Product.Name)
            .Take(8)
            .Select(x => x.Product)
            .ToList();

        foreach (var item in results)
            SearchResults.Add(item);

        IsSearchResultsVisible = SearchResults.Count > 0;
    }

    [RelayCommand]
    private void ClearMainSearch()
    {
        SearchText = string.Empty;
        SearchResults.Clear();
        IsSearchResultsVisible = false;
    }

    [RelayCommand]
    private async Task OpenSuggestedProductAsync(SalesItem item)
    {
        if (item == null)
            return;

        ClearMainSearch();
        await Task.CompletedTask;
    }

    private static OverallScoreUiResult CalculateOverallScore(UserInfo? user)
    {
        var selectedSkills = (user?.SelectedSkills ?? new List<UserSkillInfo>())
            .Where(skill => skill is not null && (!string.IsNullOrWhiteSpace(skill.SkillName) || skill.SkillId > 0))
            .ToList();

        // First preference: calculate against selected JobFamily role template.
        // Fallback: if Job was not preserved in session yet, calculate over selected skills themselves.
        // This prevents the MainPage from showing "--" while SkillsPage already has scored skills.
        var template = user?.Job is null
            ? new List<RoleSkillTemplateItem>()
            : BuildTemplateFromSelectedJob(user.Job);

        if (template.Count == 0 && selectedSkills.Count > 0)
            template = BuildTemplateFromSelectedSkills(selectedSkills);

        if (template.Count == 0)
        {
            return new OverallScoreUiResult
            {
                HasScore = false,
                Score = 0,
                StrongestRoleName = "Score",
                ImprovementHintText = "Add and verify skills"
            };
        }

        var candidateSkills = selectedSkills
            .GroupBy(skill => BuildSkillKey(skill.SkillId, skill.SkillName))
            .ToDictionary(
                group => group.Key,
                group => group.OrderByDescending(skill => GetSkillSignal(skill)).First());

        var denominator = template.Sum(item => item.Weight);
        if (denominator <= 0)
        {
            return new OverallScoreUiResult
            {
                HasScore = false,
                Score = 0,
                StrongestRoleName = user?.Job?.JobName ?? "Score",
                ImprovementHintText = "No role template skills"
            };
        }

        var numerator = template.Sum(item =>
        {
            var skill = FindCandidateSkill(candidateSkills, item);
            var signal = skill is null ? 0d : GetSkillSignal(skill);
            return item.Weight * signal;
        });

        var readiness = numerator / denominator;
        var score = RoundHalfUp(readiness);
        var hint = BuildImprovementHint(template, candidateSkills, denominator);

        return new OverallScoreUiResult
        {
            HasScore = true,
            Score = score,
            StrongestRoleName = user?.Job?.JobName ?? "Current skills",
            ImprovementHintText = hint
        };
    }

    private static List<RoleSkillTemplateItem> BuildTemplateFromSelectedJob(JobFamily job)
    {
        var result = new List<RoleSkillTemplateItem>();

        if (job.Seniorities is null)
            return result;

        foreach (var seniority in job.Seniorities)
        {
            if (seniority?.Positions is null)
                continue;

            foreach (var position in seniority.Positions)
            {
                if (position?.Skills is null)
                    continue;

                foreach (var skill in position.Skills)
                {
                    if (skill is null || string.IsNullOrWhiteSpace(skill.SkillName))
                        continue;

                    result.Add(new RoleSkillTemplateItem
                    {
                        SkillId = skill.Id,
                        SkillName = skill.SkillName,
                        Weight = WeightFromSkillComplexity(skill.SkillComplexity)
                    });
                }
            }
        }

        return result
            .GroupBy(item => BuildSkillKey(item.SkillId, item.SkillName))
            .Select(group => new RoleSkillTemplateItem
            {
                SkillId = group.First().SkillId,
                SkillName = group.First().SkillName,
                Weight = group.Max(item => item.Weight)
            })
            .ToList();
    }

    private static List<RoleSkillTemplateItem> BuildTemplateFromSelectedSkills(List<UserSkillInfo> selectedSkills)
    {
        return selectedSkills
            .Where(skill => skill is not null && (!string.IsNullOrWhiteSpace(skill.SkillName) || skill.SkillId > 0))
            .Select(skill => new RoleSkillTemplateItem
            {
                SkillId = skill.SkillId,
                SkillName = skill.SkillName,
                Weight = WeightFromSkillComplexity(skill.SkillComplexity)
            })
            .GroupBy(item => BuildSkillKey(item.SkillId, item.SkillName))
            .Select(group => new RoleSkillTemplateItem
            {
                SkillId = group.First().SkillId,
                SkillName = group.First().SkillName,
                Weight = group.Max(item => item.Weight)
            })
            .ToList();
    }

    private static int WeightFromSkillComplexity(string? complexity)
    {
        // Word spec says role-template weight is core=2, secondary=1.
        // Current mobile model has SkillComplexity instead of Core/Secondary.
        // Mapping for MVP: high => core(2), everything else => secondary(1).
        var value = (complexity ?? string.Empty).Trim().ToLowerInvariant();
        return value == "high" ? 2 : 1;
    }

    private static UserSkillInfo? FindCandidateSkill(
        Dictionary<string, UserSkillInfo> candidateSkills,
        RoleSkillTemplateItem templateItem)
    {
        var key = BuildSkillKey(templateItem.SkillId, templateItem.SkillName);
        if (candidateSkills.TryGetValue(key, out var exact))
            return exact;

        if (!string.IsNullOrWhiteSpace(templateItem.SkillName))
        {
            var nameKey = BuildSkillKey(0, templateItem.SkillName);
            if (candidateSkills.TryGetValue(nameKey, out var byName))
                return byName;
        }

        return null;
    }

    private static double GetSkillSignal(UserSkillInfo skill)
    {
        // Prefer UserSkillInfo.Signal because that model already contains the spec logic:
        // verified => CS, self_declared => min(CS, 40), absent => 0.
        // The fallback below keeps this MainViewModel safe if an older UserSkillInfo is used.
        var modelSignal = skill.Signal;
        if (modelSignal > 0)
            return ClampScore(modelSignal);

        var credibility = GetCredibilityScore(skill);
        var status = (skill.Status ?? string.Empty).Trim().ToLowerInvariant();

        if (skill.IsVerified || status == "verified")
            return credibility;

        if (status == "absent")
            return 0;

        return Math.Min(credibility, 40d);
    }

    private static double GetCredibilityScore(UserSkillInfo skill)
    {
        if (skill.CredibilityScore > 0)
            return ClampScore(skill.CredibilityScore);

        var knowledge = skill.KnowledgeScore > 0 ? skill.KnowledgeScore : skill.Knowledge;
        var experience = skill.ExperienceScore > 0 ? skill.ExperienceScore : skill.Experience;

        return ClampScore((knowledge * 0.45d) + (experience * 0.55d));
    }

    private static string BuildImprovementHint(
        List<RoleSkillTemplateItem> template,
        Dictionary<string, UserSkillInfo> candidateSkills,
        int denominator)
    {
        if (denominator <= 0)
            return string.Empty;

        const double target = 70d;

        var candidates = template
            .Select(item => new
            {
                Template = item,
                Skill = FindCandidateSkill(candidateSkills, item)
            })
            .Where(x => x.Skill is null || !IsVerified(x.Skill))
            .Select(x =>
            {
                var signal = x.Skill is null ? 0d : GetSkillSignal(x.Skill);
                var gain = x.Template.Weight * Math.Max(target - signal, 0d) / denominator;

                return new
                {
                    x.Template,
                    x.Skill,
                    Gain = gain
                };
            })
            .Where(x => x.Gain > 0)
            .OrderByDescending(x => x.Gain)
            .ThenByDescending(x => x.Template.Weight)
            .ThenBy(x => x.Template.SkillName)
            .ToList();

        if (candidates.Count == 0)
            return "All key skills verified";

        var best = candidates[0];
        var prefix = best.Skill is null ? "Add & verify" : "Verify";
        var roundedGain = RoundHalfUp(best.Gain);

        return $"{prefix} {best.Template.SkillName} → ≈ +{roundedGain}";
    }

    private static bool IsVerified(UserSkillInfo skill)
    {
        return skill.IsVerified ||
               string.Equals(skill.Status, "verified", StringComparison.OrdinalIgnoreCase);
    }

    private static string BuildSkillKey(int id, string? name)
    {
        if (id > 0)
            return $"id:{id}";

        return $"name:{Normalize(name ?? string.Empty)}";
    }

    private static string Normalize(string value)
    {
        value = (value ?? string.Empty).Trim().ToLowerInvariant();

        while (value.Contains("  ", StringComparison.Ordinal))
            value = value.Replace("  ", " ", StringComparison.Ordinal);

        return value;
    }

    private static int SearchScore(string name, string key)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(key))
            return 1000;

        if (name.StartsWith(key, StringComparison.Ordinal))
            return 0;

        var index = name.IndexOf(key, StringComparison.Ordinal);
        if (index >= 0)
            return 10 + index;

        var parts = key.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length > 1 && parts.All(part => name.Contains(part, StringComparison.Ordinal)))
            return 80;

        return 1000;
    }

    private static int RoundHalfUp(double value)
    {
        return (int)Math.Clamp(Math.Floor(value + 0.5d), 0, 100);
    }

    private static double ClampScore(double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
            return 0;

        return Math.Clamp(value, 0d, 100d);
    }

    private sealed class OverallScoreUiResult
    {
        public bool HasScore { get; set; }
        public int Score { get; set; }
        public string StrongestRoleName { get; set; } = string.Empty;
        public string ImprovementHintText { get; set; } = string.Empty;
    }

    private sealed class RoleSkillTemplateItem
    {
        public int SkillId { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public int Weight { get; set; }
    }
}
