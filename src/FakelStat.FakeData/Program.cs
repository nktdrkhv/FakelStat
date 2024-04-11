using FakelStat.Models;
using PetaPoco;
using PetaPoco.Providers;

var db = DatabaseConfiguration.Build()
         .UsingConnectionString("Data Source=fakel.sqlite3;Version=3;")
         .UsingProvider<SQLiteDatabaseProvider>()
         .UsingDefaultMapper<ConventionMapper>(mapper =>
         {
             mapper.ToDbConverter =
                sourceProperty =>
                    sourceProperty.PropertyType.Equals(typeof(DateOnly))
                         ? obj => ((DateOnly)obj).ToString("O")
                         : obj => obj;
             mapper.FromDbConverter =
                 (targetProperty, sourceType) =>
                    targetProperty.PropertyType.Name switch
                    {
                        nameof(DateOnly) => obj => DateOnly.FromDateTime(DateTime.Parse(obj.ToString()!)),
                        nameof(TimeOnly) => obj => TimeOnly.FromDateTime(DateTime.Parse(obj.ToString()!)),
                        _ => obj => Convert.ChangeType(obj, targetProperty.PropertyType),
                    };
         })
         .Create();

IEnumerable<DateTime> EachDay(DateTime startDay, DateTime endDay)
{
    for (var day = startDay.Date; day <= endDay; day = day.AddDays(1))
        yield return day;
}

IEnumerable<DateTime> EachMoment(DateTime startTime, DateTime endTime)
{
    for (var time = startTime; time <= endTime; time = time.AddMinutes(10))
        yield return time;
}

int CurrentLoad(DateTime moment, int previousLoad)
{
    var hour = moment.Hour;
    var delta = Random.Shared.Next(5);
    var newLoad = (GetSign(hour) || previousLoad < 10) && previousLoad < 80
                    ? previousLoad + delta
                    : previousLoad - delta;
    return newLoad;
}

bool GetSign(int hour) => hour switch
{
    int n when (n > 7) && (n < 9) => true,
    int n when (n >= 9) && (n < 11) => Random.Shared.Next(100) / 100.0 <= 0.3,
    int n when (n >= 11) && (n < 13) => Random.Shared.Next(100) / 100.0 <= 0.7,
    int n when (n >= 13) && (n < 15) => Random.Shared.Next(100) / 100.0 <= 0.3,
    int n when (n >= 15) && (n < 17) => Random.Shared.Next(100) / 100.0 <= 0.7,
    int n when (n >= 17) && (n < 19) => Random.Shared.Next(100) / 100.0 <= 0.3,
    int n when (n >= 19) && (n < 21) => Random.Shared.Next(100) / 100.0 <= 0.7,
    int n when (n >= 21) && (n <= 23) => Random.Shared.Next(100) / 100.0 <= 0.3,
    _ => false
};

var startDay = new DateTime(2024, 2, 1);
var endDay = new DateTime(2024, 3, 1);
foreach (var day in EachDay(startDay, endDay))
{
    var (startTime, endTime) = day.DayOfWeek switch
    {
        DayOfWeek.Monday or DayOfWeek.Tuesday or DayOfWeek.Tuesday
        or DayOfWeek.Wednesday or DayOfWeek.Thursday or DayOfWeek.Friday
            => (day.AddMinutes(7 * 60 + 5), day.AddMinutes(23 * 60)),
        // => (new DateTime(DateOnly.FromDateTime(day), TimeOnly.FromTimeSpan(TimeSpan.FromHours(7)), DateTimeKind.Local),
        //     new DateTime(DateOnly.FromDateTime(day), TimeOnly.FromTimeSpan(TimeSpan.FromHours(23)), DateTimeKind.Local)),
        DayOfWeek.Saturday or DayOfWeek.Sunday
            => (day.AddMinutes(9 * 60 + 5), day.AddMinutes(21 * 60)),
        // => (new DateTime(DateOnly.FromDateTime(day), TimeOnly.FromTimeSpan(TimeSpan.FromHours(9)), DateTimeKind.Local),
        //     new DateTime(DateOnly.FromDateTime(day), TimeOnly.FromTimeSpan(TimeSpan.FromHours(21)), DateTimeKind.Local)),
        _ => throw new NotImplementedException()
    };

    int previousLoad = 0;
    foreach (var moment in EachMoment(startTime, endTime))
    {
        var load = CurrentLoad(moment, previousLoad);
        var momentumLoad = new MomentumLoad
        {
            Amount = load,
            Day = DateOnly.FromDateTime(moment),
            Time = TimeOnly.FromDateTime(moment)
        };
        var id = db.Insert(momentumLoad);
        previousLoad = load;
        Console.WriteLine($"{id} - {momentumLoad.Amount} - {momentumLoad.Day} - {momentumLoad.Time}");
    }
}

//var dayLoad = db.Fetch<MomentumLoad>($"SELECT * FROM {nameof(MomentumLoad)} WHERE Day <= @0", day);
var dayLoad = db.Fetch<MomentumLoad>($"SELECT * FROM {nameof(MomentumLoad)}");
foreach (var a in dayLoad)
{
    Console.WriteLine($"{a.Id} - {a.Amount} - {a.Day} - {a.Time}");
}