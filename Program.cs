using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    var repositoryDirectory =Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
    var repositoryPath = $"{repositoryDirectory}/data/dates.txt";

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddCors();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Host.UseSerilog((context, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration));

    builder.Services.AddSingleton<IDaysRepository>(new DaysTextFileRepository(new FileInfo(repositoryPath)));

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseSwagger(options =>
    {
        options.SerializeAsV2 = true;
    });

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });

    app.MapGet("/days/{yyyyMMdd}/add/days/{daysToAdd}", async (string yyyyMMdd, long daysToAdd, IDaysRepository daysRepository) =>
    {
        if (DateTime.TryParseExact(yyyyMMdd, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var dateTime))
        {
            var result = await GetDayInfo(daysRepository, dateTime.AddDays(daysToAdd));
            return Results.Ok(result);
        }

        return Results.BadRequest();
    });

    app.MapGet("/days/{yyyyMMdd}/add/workingDays/{daysToAdd}", async (string yyyyMMdd, long daysToAdd, IDaysRepository daysRepository) =>
    {
        var daysAdded = 0L;        
        var incrementor = daysToAdd > 0 ? 1 : -1;

        if (DateTime.TryParseExact(yyyyMMdd, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var dateTime))
        {
            var nextDayInfo = await GetDayInfo(daysRepository, dateTime);
            var result = dateTime;
            while (daysToAdd != daysAdded)
            {
                result = result.AddDays(incrementor);
                nextDayInfo = await GetDayInfo(daysRepository, result);
                if (nextDayInfo.DayType == DayType.WorkingDay) {
                    daysAdded += incrementor;
                }
            }
            return Results.Ok(nextDayInfo);
        }

        return Results.BadRequest();
    });

    app.MapGet("/days/{yyyyMMdd}", async (string yyyyMMdd, IDaysRepository daysRepository) =>
    {
        if (DateTime.TryParseExact(yyyyMMdd, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var dateTime))
        {
            return Results.Ok(await GetDayInfo(daysRepository, dateTime));
        }
        return Results.BadRequest();

    });

    app.MapGet("/days", async (IDaysRepository daysRepository) =>
        Results.Ok(await daysRepository.GetAll())
    );       

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

async Task<DayInfo> GetDayInfo(IDaysRepository daysRepository, DateTime dateTime)
{
    var dayInfos = await daysRepository.Find(x => x.Day.Date == dateTime.Date);
    if (dayInfos.Count() == 0)
    {
        var result = new DayInfo()
        {
            Day = dateTime,
            DayType = IsItDayOff(dateTime) ? DayType.DayOff : DayType.WorkingDay,
        };
        Log.Debug("Date {Day} found in registry is {DayTypeAsString}.", result.Day, result.DayTypeAsString);
        return result;
    }
    return dayInfos.First();
}

bool IsItDayOff(DateTime dateTime) => dateTime.DayOfWeek == DayOfWeek.Saturday || dateTime.DayOfWeek == DayOfWeek.Sunday;
