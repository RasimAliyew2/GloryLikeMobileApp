namespace MetanetA_MobileApp.Model;

public class UserWorkExperienceInfo
{
    public string CompanyName { get; set; } = string.Empty;
    public string PositionName { get; set; } = string.Empty;
    public string StartYear { get; set; } = string.Empty;
    public string EndYear { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;

    // Compatibility aliases: köhnə kod Position/From/Ending istifadə edirsə, qırılmasın.
    public string Position
    {
        get => PositionName;
        set => PositionName = value;
    }

    public string From
    {
        get => StartYear;
        set => StartYear = value;
    }

    public string Ending
    {
        get => EndYear;
        set => EndYear = value;
    }
}
