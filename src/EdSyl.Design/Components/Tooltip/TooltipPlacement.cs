namespace EdSyl.Design.Components;

[JsonConverter(typeof(JsonStringEnumConverter<TooltipPlacement>))]
public enum TooltipPlacement
{
    [JsonStringEnumMemberName("top")]
    [Display(Name = "Top")]
    Top = 0,

    [JsonStringEnumMemberName("bottom")]
    [Display(Name = "Bottom")]
    Bottom = 1,

    [JsonStringEnumMemberName("left")]
    [Display(Name = "Left")]
    Left = 2,

    [JsonStringEnumMemberName("right")]
    [Display(Name = "Right")]
    Right = 3,
}
