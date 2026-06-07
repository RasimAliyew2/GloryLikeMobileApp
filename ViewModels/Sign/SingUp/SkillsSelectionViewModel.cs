using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MetanetA_MobileApp.Model;
using MetanetA_MobileApp.Services.Abstractions;
using MetanetA_MobileApp.ViewModels.Sign.SingUp;

namespace MetanetA_MobileApp.ViewModels;

public partial class SkillsSelectionViewModel : ObservableObject
{
    private readonly ISkillAndJobApiService _skillAndJobApiService;
    private bool _isLoaded;

    public SkillsSelectionViewModel(ISkillAndJobApiService skillAndJobApiService)
    {
        _skillAndJobApiService = skillAndJobApiService;
    }

    public ObservableCollection<SelectableItem<JobFamily>> JobFamilyOptions { get; } = new();
    public ObservableCollection<SelectableItem<Seniority>> SeniorityOptions { get; } = new();
    public ObservableCollection<SelectableItem<Position>> PositionOptions { get; } = new();
    public ObservableCollection<SkillSelectionItem> SkillOptions { get; } = new();

    [ObservableProperty]
    private SelectionStep currentStep = SelectionStep.JobFamily;

    [ObservableProperty]
    private JobFamily? selectedJobFamily;

    [ObservableProperty]
    private Seniority? selectedSeniority;

    [ObservableProperty]
    private Position? selectedPosition;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string? errorMessage;

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public bool IsJobFamilyStep => CurrentStep == SelectionStep.JobFamily;
    public bool IsSeniorityStep => CurrentStep == SelectionStep.Seniority;
    public bool IsPositionStep => CurrentStep == SelectionStep.Position;
    public bool IsSkillStep => CurrentStep == SelectionStep.Skill;

    public bool ShowBreadcrumb => CurrentStep != SelectionStep.JobFamily;

    public string BreadcrumbText => CurrentStep switch
    {
        SelectionStep.Seniority => SelectedJobFamily?.JobName ?? string.Empty,
        SelectionStep.Position => $"{SelectedJobFamily?.JobName} · {SelectedSeniority?.Name}",
        SelectionStep.Skill => $"{SelectedPosition?.Name} · {SelectedSeniority?.Name}",
        _ => string.Empty
    };

    public string SectionTitle => CurrentStep switch
    {
        SelectionStep.JobFamily => "JOB FAMILY",
        SelectionStep.Seniority => "SENIORITY",
        SelectionStep.Position => "POSITION",
        SelectionStep.Skill => SelectedPosition is null
            ? "SUGGESTED SKILLS"
            : $"SUGGESTED SKILLS FOR {SelectedPosition.Name.ToUpper()}",
        _ => string.Empty
    };

    public int SelectedSkillCount => SkillOptions.Count(x => x.IsSelected);

    public bool HasSelectedSkills => SelectedSkillCount > 0;

    public List<Skill> SelectedSkills =>
        SkillOptions
            .Where(x => x.IsSelected)
            .Select(x => x.Skill)
            .ToList();

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (_isLoaded)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = null;

            var result = await _skillAndJobApiService.GetJobFamiliesAsync();

            JobFamilyOptions.Clear();

            foreach (var item in result.OrderBy(x => x.JobName))
            {
                JobFamilyOptions.Add(new SelectableItem<JobFamily>(item, item.JobName));
            }

            _isLoaded = true;
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
    private void SelectJobFamily(SelectableItem<JobFamily>? item)
    {
        if (item is null)
            return;

        MarkSingleSelected(JobFamilyOptions, item);

        SelectedJobFamily = item.Value;
        SelectedSeniority = null;
        SelectedPosition = null;

        SeniorityOptions.Clear();
        PositionOptions.Clear();
        SkillOptions.Clear();

        foreach (var seniority in item.Value.Seniorities.OrderBy(x => GetSeniorityOrder(x.Name)))
        {
            SeniorityOptions.Add(new SelectableItem<Seniority>(seniority, seniority.Name));
        }

        CurrentStep = SelectionStep.Seniority;
        RefreshComputedProperties();
    }

    [RelayCommand]
    private void SelectSeniority(SelectableItem<Seniority>? item)
    {
        if (item is null)
            return;

        MarkSingleSelected(SeniorityOptions, item);

        SelectedSeniority = item.Value;
        SelectedPosition = null;

        PositionOptions.Clear();
        SkillOptions.Clear();

        foreach (var position in item.Value.Positions.OrderBy(x => x.Name))
        {
            PositionOptions.Add(new SelectableItem<Position>(position, position.Name));
        }

        CurrentStep = SelectionStep.Position;
        RefreshComputedProperties();
    }

    [RelayCommand]
    private void SelectPosition(SelectableItem<Position>? item)
    {
        if (item is null)
            return;

        MarkSingleSelected(PositionOptions, item);

        SelectedPosition = item.Value;

        SkillOptions.Clear();

        foreach (var skill in item.Value.Skills.OrderBy(x => x.SkillName))
        {
            SkillOptions.Add(new SkillSelectionItem(skill));
        }

        CurrentStep = SelectionStep.Skill;
        RefreshComputedProperties();
    }

    [RelayCommand]
    private void ToggleSkill(SkillSelectionItem? item)
    {
        if (item is null)
            return;

        item.IsSelected = !item.IsSelected;
        RefreshComputedProperties();
    }

    [RelayCommand]
    private async Task BackAsync()
    {
        switch (CurrentStep)
        {
            case SelectionStep.Skill:
                SkillOptions.Clear();
                SelectedPosition = null;

                foreach (var item in PositionOptions)
                    item.IsSelected = false;

                CurrentStep = SelectionStep.Position;
                break;

            case SelectionStep.Position:
                PositionOptions.Clear();
                SkillOptions.Clear();
                SelectedSeniority = null;
                SelectedPosition = null;

                foreach (var item in SeniorityOptions)
                    item.IsSelected = false;

                CurrentStep = SelectionStep.Seniority;
                break;

            case SelectionStep.Seniority:
                SeniorityOptions.Clear();
                PositionOptions.Clear();
                SkillOptions.Clear();
                SelectedJobFamily = null;
                SelectedSeniority = null;
                SelectedPosition = null;

                foreach (var item in JobFamilyOptions)
                    item.IsSelected = false;

                CurrentStep = SelectionStep.JobFamily;
                break;

            case SelectionStep.JobFamily:
                if (Shell.Current is not null)
                    await Shell.Current.GoToAsync("..");
                break;
        }

        RefreshComputedProperties();
    }

    [RelayCommand]
    private async Task SkipAsync()
    {
        if (Shell.Current is not null)
            await Shell.Current.GoToAsync("..");
    }

    private static void MarkSingleSelected<T>(
        IEnumerable<SelectableItem<T>> items,
        SelectableItem<T> selectedItem)
    {
        foreach (var item in items)
        {
            item.IsSelected = item == selectedItem;
        }
    }

    private static int GetSeniorityOrder(string name)
    {
        return name switch
        {
            "Junior" => 1,
            "Middle" => 2,
            "Senior" => 3,
            "Lead" => 4,
            "Head" => 5,
            _ => 99
        };
    }

    partial void OnCurrentStepChanged(SelectionStep value) => RefreshComputedProperties();
    partial void OnSelectedJobFamilyChanged(JobFamily? value) => RefreshComputedProperties();
    partial void OnSelectedSeniorityChanged(Seniority? value) => RefreshComputedProperties();
    partial void OnSelectedPositionChanged(Position? value) => RefreshComputedProperties();
    partial void OnErrorMessageChanged(string? value) => OnPropertyChanged(nameof(HasError));

    private void RefreshComputedProperties()
    {
        OnPropertyChanged(nameof(IsJobFamilyStep));
        OnPropertyChanged(nameof(IsSeniorityStep));
        OnPropertyChanged(nameof(IsPositionStep));
        OnPropertyChanged(nameof(IsSkillStep));
        OnPropertyChanged(nameof(ShowBreadcrumb));
        OnPropertyChanged(nameof(BreadcrumbText));
        OnPropertyChanged(nameof(SectionTitle));
        OnPropertyChanged(nameof(SelectedSkillCount));
        OnPropertyChanged(nameof(HasSelectedSkills));
        OnPropertyChanged(nameof(SelectedSkills));
    }
}