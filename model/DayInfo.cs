
/// <summary>
/// Type.
/// </summary>
public enum DayType {
    Unknown = 0,
    DayOff,
    WorkingDay,
}

/// <summary>
/// Day info.
/// </summary>
public class DayInfo {

    /// <summary>
    /// Types.
    /// </summary>
    public static IReadOnlyDictionary<DayType, string> dayTypes = new Dictionary<DayType, string> () {
        {DayType.Unknown, "Нет данных"},
        {DayType.DayOff, "Выходной день"},
        {DayType.WorkingDay, "Рабочий день"},
    };

    /// <summary>
    /// Date.
    /// </summary>
    public DateTime Day { get; set; }

    /// <summary>
    /// Type.
    /// </summary>
    public DayType DayType { get; set; }

    /// <summary>
    /// Type as string.
    /// </summary>
    public string DayTypeAsString => dayTypes[DayType];
    
}