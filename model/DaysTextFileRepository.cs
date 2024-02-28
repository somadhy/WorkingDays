using System.Globalization;
using Serilog;

/// <summary>
/// Repo for days
/// </summary>
public class DaysTextFileRepository (FileInfo daysFile, uint cacheLifetimeSeconds = 600) : IDaysRepository
{


    private DateTime cacheLifetime;

    private HashSet<DayInfo> cache = [];
    
    /// <summary>
    /// Create new day info in registry.
    /// </summary>
    /// <param name="dayInfo">Info.</param>
    /// <exception cref="NotImplementedException"></exception>
    public Task Create(DayInfo dayInfo)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Search for day info in registry.
    /// </summary>
    /// <param name="where">Where function.</param>
    /// <returns></returns>
    public async Task<IEnumerable<DayInfo>> Find(Func<DayInfo, bool> where)
    {
        return await EnshureFresh(() => cache.Where(where));
    }

    /// <summary>
    /// Geting all day info from registry.
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<DayInfo>> GetAll()
    {
        return await EnshureFresh(() => cache);
    }

    /// <summary>
    /// Update day info in registry.
    /// </summary>
    /// <param name="dayInfo">Updated info.</param>
    public Task Update(DayInfo dayInfo)
    {
        throw new NotImplementedException();
    }

    private async Task<IEnumerable<DayInfo>> EnshureFresh(Func<IEnumerable<DayInfo>> method)
    {
        if(cacheLifetime <= DateTime.UtcNow) {
            await Reload();
            cacheLifetime = DateTime.UtcNow.AddSeconds(cacheLifetimeSeconds);
        }        
        return method();
    }

    private async Task Reload()
    {
        Log.Debug("Cache expired. Refresh.");
        cache.Clear();
        using var reader = new StreamReader(daysFile.FullName);

        while (!reader.EndOfStream)
        {
            string[] line = (await reader.ReadLineAsync())?.Split(';') ?? [];

            if (!DateTime.TryParseExact(line[0], "yyyy.MM.dd", null, DateTimeStyles.None, out DateTime dateTime))
            {
                continue;
            }

            cache.Add(new DayInfo()
            {
                Day = dateTime,
                DayType = line.Length > 1 
                        && Enum.TryParse<DayType>(line[1], out DayType dayType)
                            ? dayType
                            : DayType.Unknown,
            });
        }
        Log.Debug("Cache reloaded.");   
    }
}