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
using MetanetA_MobileApp.View;
using MetanetA_MobileApp.View.Profile;

namespace MetanetA_MobileApp.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly SalesCatalogService _catalog;
    private readonly IUserSession _userSession;
    private readonly UserInfo userInfo;

    [ObservableProperty]
    private CartState cart;

    [ObservableProperty]
    private CartService cartService;

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private bool isSearchResultsVisible;

    [ObservableProperty]
    private string overallScoreText = "Choose target roles";

    [ObservableProperty]
    private string strongestRoleName = string.Empty;

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
        this.userInfo = userInfo;
        Cart = cart;
        CartService = cartService;
        _catalog = catalog;

        _userSession.CurrentUser ??= userInfo;

        _catalog.Products.CollectionChanged += Products_CollectionChanged;

        RefreshScore();
    }

    [RelayCommand]
    private void RefreshScore()
    {
        var currentUser = _userSession.CurrentUser ?? userInfo;

        var result = CalculateOverallScore(currentUser);

        HasOverallScore = result.HasScore;
        OverallScoreText = result.HasScore ? $"{result.Score}%" : "Choose target roles";
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

        var k = Normalize(key);

        var results = _catalog.Products
            .Where(p => p != null && !string.IsNullOrWhiteSpace(p.Name))
            .Select(p => new
            {
                p,
                score = Score(Normalize(p.Name), k)
            })
            .Where(x => x.score < 1000)
            .OrderBy(x => x.score)
            .ThenBy(x => x.p.Name)
            .Take(8)
            .Select(x => x.p)
            .ToList();

        foreach (var item in results)
            SearchResults.Add(item);

        IsSearchResultsVisible = SearchResults.Count > 0;
    }

    private static string Normalize(string s)
    {
        s = (s ?? string.Empty).Trim().ToLowerInvariant();
        while (s.Contains("  "))
            s = s.Replace("  ", " ");
        return s;
    }

    private static int Score(string name, string key)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(key))
            return 1000;

        if (name.StartsWith(key, StringComparison.Ordinal))
            return 0;

        var idx = name.IndexOf(key, StringComparison.Ordinal);
        if (idx >= 0)
            return 10 + idx;

        var parts = key.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length > 1 && parts.All(p => name.Contains(p, StringComparison.Ordinal)))
            return 80;

        return 1000;
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
        if (user?.Job is null)
        {
            return new OverallScoreUiResult
            {
                HasScore = false,
                Score = 0,
                StrongestRoleName = string.Empty,
                ImprovementHintText = "Choose target roles"
            };
        }

        var template = BuildTemplateFromSelectedJob(user.Job);
        if (template.Count == 0)
        {
            return new OverallScoreUiResult
            {
                HasScore = false,
                Score = 0,
                StrongestRoleName = user.Job.JobName,
                ImprovementHintText = "No role template skills"
            };
        }

        var candidateSkills = (user.SelectedSkills ?? new List<UserSkillInfo>())
            .Where(x => x is not null && (!string.IsNullOrWhiteSpace(x.SkillName) || x.SkillId > 0))
            .GroupBy(x => x.SkillId > 0 ? $"id:{x.SkillId}" : $"name:{Normalize(x.SkillName)}")
            .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.Signal).First());

        var denominator = template.Sum(x => x.Weight);
        if (denominator <= 0)
        {
            return new OverallScoreUiResult
            {
                HasScore = false,
                Score = 0,
                StrongestRoleName = user.Job.JobName,
                ImprovementHintText = "No role template skills"
            };
        }

        var numerator = template.Sum(item =>
        {
            var skill = FindCandidateSkill(candidateSkills, item);
            return item.Weight * (skill?.Signal ?? 0d);
        });

        var readiness = numerator / denominator;
        var score = RoundHalfUp(readiness);
        var hint = BuildImprovementHint(template, candidateSkills, denominator);

        return new OverallScoreUiResult
        {
            HasScore = true,
            Score = score,
            StrongestRoleName = user.Job.JobName,
            ImprovementHintText = hint
        };
    }

    private static List<RoleSkillTemplateItem> BuildTemplateFromSelectedJob(JobFamily job)
    {
        return job.Seniorities
            .SelectMany(seniority => seniority.Positions)
            .SelectMany(position => position.Skills)
            .Where(skill => skill is not null && !string.IsNullOrWhiteSpace(skill.SkillName))
            .GroupBy(skill => skill.Id > 0 ? $"id:{skill.Id}" : $"name:{Normalize(skill.SkillName)}")
            .Select(group =>
            {
                var skill = group.First();
                return new RoleSkillTemplateItem
                {
                    SkillId = skill.Id,
                    SkillName = skill.SkillName,
                    Weight = group.Max(x => WeightFromSkillComplexity(x.SkillComplexity))
                };
            })
            .ToList();
    }

    private static int WeightFromSkillComplexity(string? complexity)
    {
        var value = (complexity ?? string.Empty).Trim().ToLowerInvariant();
        return value == "high" ? 2 : 1;
    }

    private static UserSkillInfo? FindCandidateSkill(
        Dictionary<string, UserSkillInfo> candidateSkills,
        RoleSkillTemplateItem templateItem)
    {
        if (templateItem.SkillId > 0 && candidateSkills.TryGetValue($"id:{templateItem.SkillId}", out var byId))
            return byId;

        if (!string.IsNullOrWhiteSpace(templateItem.SkillName) &&
            candidateSkills.TryGetValue($"name:{Normalize(templateItem.SkillName)}", out var byName))
            return byName;

        return null;
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
            .Where(x => x.Skill is null || !x.Skill.IsVerified && !string.Equals(x.Skill.Status, "verified", StringComparison.OrdinalIgnoreCase))
            .Select(x =>
            {
                var signal = x.Skill?.Signal ?? 0d;
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
        var roundedGain = RoundHalfUp(best.Gain);
        var prefix = best.Skill is null ? "Add & verify" : "Verify";

        return $"{prefix} {best.Template.SkillName} → ≈ +{roundedGain}";
    }

    private static int RoundHalfUp(double value)
    {
        return (int)Math.Clamp(Math.Floor(value + 0.5d), 0, 100);
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
