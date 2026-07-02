using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MetanetA_MobileApp.Model;
using MetanetA_MobileApp.Model.Questionnaires;
using MetanetA_MobileApp.Services.Abstractions;
using MetanetA_MobileApp.Services.UIState;
using MetanetA_MobileApp.View.SignUp;

namespace MetanetA_MobileApp.ViewModels.Sign.SingUp;

public partial class VerifySkillsViewModel : ObservableObject
{
    private readonly SkillVerificationState _skillVerificationState;
    private readonly ISkillQuestionnaireApiService _questionnaireApiService;

    private bool _isLoaded;

    public VerifySkillsViewModel(
        SkillVerificationState skillVerificationState,
        ISkillQuestionnaireApiService questionnaireApiService)
    {
        _skillVerificationState = skillVerificationState;
        _questionnaireApiService = questionnaireApiService;

        Steps.Add(new IdentityStepModel { Title = "Identity", IsCompleted = true });
        Steps.Add(new IdentityStepModel { Title = "Career", IsCompleted = true });
        Steps.Add(new IdentityStepModel { Title = "Skills", IsCompleted = true });
        Steps.Add(new IdentityStepModel { Title = "Verify", SubTitle = "4 min", IsActive = true });
        Steps.Add(new IdentityStepModel { Title = "Prefs" });
        Steps.Add(new IdentityStepModel { Title = "Ready" });
    }

    public ObservableCollection<IdentityStepModel> Steps { get; } = new();

    public ObservableCollection<SkillQuestionnaireSessionItem> Skills { get; } = new();

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string? errorMessage;

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public bool HasSkills => Skills.Count > 0;

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (_isLoaded)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = null;
            Skills.Clear();

            if (_skillVerificationState.SelectedSkills.Count == 0)
            {
                ErrorMessage = "Verify etmək üçün seçilmiş skill yoxdur.";
                return;
            }

            foreach (var selectedSkill in _skillVerificationState.SelectedSkills)
            {
                var session = new SkillQuestionnaireSessionItem(selectedSkill);
                Skills.Add(session);

                await LoadQuestionnaireForSkillAsync(session);
            }

            _isLoaded = true;
            OnPropertyChanged(nameof(HasSkills));
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

    private async Task LoadQuestionnaireForSkillAsync(SkillQuestionnaireSessionItem session)
    {
        try
        {
            session.IsLoading = true;
            session.ErrorMessage = null;

            var questionnaire = await _questionnaireApiService.GenerateQuestionnaireAsync(
                new GenerateSkillQuestionnaireRequest
                {
                    Skill = session.SelectedSkill.SkillName,
                    SkillComplexity = session.SelectedSkill.SkillComplexity,
                    Seniority = session.SelectedSkill.Seniority,
                    Language = session.SelectedSkill.Language
                });

            session.SetQuestionnaire(questionnaire);
            RefreshVisibleQuestions(session);
        }
        catch (Exception ex)
        {
            session.ErrorMessage = ex.Message;
        }
        finally
        {
            session.IsLoading = false;
        }
    }

    [RelayCommand]
    private void SelectOption(QuestionnaireOptionItem? option)
    {
        if (option is null)
            return;

        var question = option.Question;

        if (question.Type.Equals("single", StringComparison.OrdinalIgnoreCase))
        {
            foreach (var item in question.Options)
                item.IsSelected = false;

            option.IsSelected = true;
        }
        else
        {
            option.IsSelected = !option.IsSelected;
        }

        question.RefreshState();
        RefreshVisibleQuestions(question.Session);
    }

    [RelayCommand]
    private async Task SubmitSkillAsync(SkillQuestionnaireSessionItem? session)
    {
        if (session is null)
            return;

        if (!session.IsCompleted)
        {
            await Application.Current!.MainPage!.DisplayAlert(
                "Incomplete",
                "Bu skill üçün bütün görünən sualları cavablandır.",
                "OK");

            return;
        }

        var score = CalculateScore(session);

        await Application.Current!.MainPage!.DisplayAlert(
            "Skill verified",
            $"{session.SkillName}\nScore: {score.DepthScore}/100\nDepth: {score.DepthTier}\nOwnership: {score.OwnershipLevel}\nTask: {score.TaskComplexity}",
            "OK");
    }

    [RelayCommand]
    private async Task BackAsync()
    {
        await Shell.Current.GoToAsync($"//{nameof(SkillsSelectionPage)}");
    }

    [RelayCommand]
    private async Task SkipForNowAsync()
    {
        await Application.Current!.MainPage!.DisplayAlert(
            "Skipped",
            "Skill verification skipped for now.",
            "OK");
    }

    private void RefreshVisibleQuestions(SkillQuestionnaireSessionItem session)
    {
        var revealedQuestionIds = session.Questions
            .SelectMany(q => q.Options
                .Where(o => o.IsSelected)
                .SelectMany(o => q.Branching
                    .Where(b => b.IfOption == o.Id)
                    .Select(b => b.RevealQuestionId)))
            .ToHashSet();

        var visibleQuestions = session.Questions
            .Where(q => !q.HiddenByDefault || revealedQuestionIds.Contains(q.Id))
            .OrderBy(q => q.Order)
            .ToList();

        var visibleIds = visibleQuestions.Select(q => q.Id).ToHashSet();

        foreach (var hiddenQuestion in session.Questions.Where(q => !visibleIds.Contains(q.Id)))
        {
            foreach (var option in hiddenQuestion.Options)
                option.IsSelected = false;

            hiddenQuestion.RefreshState();
        }

        session.VisibleQuestions.Clear();

        foreach (var question in visibleQuestions)
            session.VisibleQuestions.Add(question);

        session.RefreshState();
    }

    private static SkillDepthScoreResult CalculateScore(SkillQuestionnaireSessionItem session)
    {
        var selectedOptions = session.VisibleQuestions
            .SelectMany(q => q.Options)
            .Where(o => o.IsSelected)
            .ToList();

        var rawComplexity = selectedOptions.Sum(o => o.Weights.Complexity);
        var rawOwnership = selectedOptions.Sum(o => o.Weights.Ownership);
        var rawDepth = selectedOptions.Sum(o => o.Weights.Depth);

        var maxComplexity = Math.Max(session.Scoring.MaxComplexity, 1);
        var maxOwnership = Math.Max(session.Scoring.MaxOwnership, 1);
        var maxDepth = Math.Max(session.Scoring.MaxDepth, 1);

        var complexityRatio = rawComplexity / (double)maxComplexity;
        var ownershipRatio = rawOwnership / (double)maxOwnership;
        var depthRatio = rawDepth / (double)maxDepth;

        var depthScore = (int)Math.Round(
            ((complexityRatio * 0.35) +
             (ownershipRatio * 0.30) +
             (depthRatio * 0.35)) * 100);

        depthScore = Math.Clamp(depthScore, 0, 100);

        return new SkillDepthScoreResult
        {
            DepthScore = depthScore,
            TaskComplexity = depthScore < 40 ? "routine" : depthScore < 75 ? "complex" : "strategic",
            OwnershipLevel = ownershipRatio < 0.40 ? "contributor" : ownershipRatio < 0.75 ? "owner" : "leader",
            DepthTier = depthScore < 35 ? "basic" :
                        depthScore < 65 ? "proficient" :
                        depthScore < 85 ? "advanced" :
                        "expert"
        };
    }

    partial void OnErrorMessageChanged(string? value)
    {
        OnPropertyChanged(nameof(HasError));
    }
}

public partial class SkillQuestionnaireSessionItem : ObservableObject
{
    public SkillQuestionnaireSessionItem(SelectedSkillForVerification selectedSkill)
    {
        SelectedSkill = selectedSkill;
    }

    public SelectedSkillForVerification SelectedSkill { get; }

    public string SkillName => SelectedSkill.SkillName;

    public ObservableCollection<QuestionnaireQuestionItem> Questions { get; } = new();

    public ObservableCollection<QuestionnaireQuestionItem> VisibleQuestions { get; } = new();

    public QuestionnaireScoringDto Scoring { get; private set; } = new();

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string? errorMessage;

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public int AnsweredCount => VisibleQuestions.Count(q => q.IsAnswered);

    public int TotalVisibleQuestions => VisibleQuestions.Count;

    public bool IsCompleted => TotalVisibleQuestions > 0 && VisibleQuestions.All(q => q.IsAnswered);

    public string ProgressText => $"{AnsweredCount}/{TotalVisibleQuestions} answered";

    public void SetQuestionnaire(SkillQuestionnaireResponse questionnaire)
    {
        Scoring = questionnaire.Scoring;
        Questions.Clear();
        VisibleQuestions.Clear();

        foreach (var question in questionnaire.Questions.OrderBy(q => q.Order))
        {
            Questions.Add(new QuestionnaireQuestionItem(this, question));
        }

        RefreshState();
    }

    public void RefreshState()
    {
        OnPropertyChanged(nameof(AnsweredCount));
        OnPropertyChanged(nameof(TotalVisibleQuestions));
        OnPropertyChanged(nameof(IsCompleted));
        OnPropertyChanged(nameof(ProgressText));
    }

    partial void OnErrorMessageChanged(string? value)
    {
        OnPropertyChanged(nameof(HasError));
    }
}

public partial class QuestionnaireQuestionItem : ObservableObject
{
    public QuestionnaireQuestionItem(
        SkillQuestionnaireSessionItem session,
        QuestionnaireQuestionDto dto)
    {
        Session = session;
        Id = dto.Id;
        Order = dto.Order;
        Dimension = dto.Dimension;
        HiddenByDefault = dto.HiddenByDefault;
        Text = dto.Text;
        Type = dto.Type;
        Branching = dto.Branching;

        foreach (var option in dto.Options)
        {
            Options.Add(new QuestionnaireOptionItem(this, option));
        }
    }

    public SkillQuestionnaireSessionItem Session { get; }

    public string Id { get; }

    public int Order { get; }

    public string Dimension { get; }

    public string DimensionCaption => Dimension.ToUpperInvariant();

    public bool HiddenByDefault { get; }

    public string Text { get; }

    public string Type { get; }

    public List<QuestionnaireBranchingRuleDto> Branching { get; }

    public ObservableCollection<QuestionnaireOptionItem> Options { get; } = new();

    public bool IsAnswered => Options.Any(o => o.IsSelected);

    public void RefreshState()
    {
        OnPropertyChanged(nameof(IsAnswered));
    }
}

public partial class QuestionnaireOptionItem : ObservableObject
{
    public QuestionnaireOptionItem(
        QuestionnaireQuestionItem question,
        QuestionnaireOptionDto dto)
    {
        Question = question;
        Id = dto.Id;
        Label = dto.Label;
        Weights = dto.Weights;
    }

    public QuestionnaireQuestionItem Question { get; }

    public string Id { get; }

    public string Label { get; }

    public QuestionnaireOptionWeightsDto Weights { get; }

    [ObservableProperty]
    private bool isSelected;
}

public class SkillDepthScoreResult
{
    public int DepthScore { get; set; }

    public string TaskComplexity { get; set; } = string.Empty;

    public string OwnershipLevel { get; set; } = string.Empty;

    public string DepthTier { get; set; } = string.Empty;
}