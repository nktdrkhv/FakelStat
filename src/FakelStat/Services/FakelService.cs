using System.Globalization;
using System.Text.Json;
using FakelStat.Models;

namespace FakelStat.Services;

public class FakelService(HttpClient httpClient)
{
    public async ValueTask<int> GetCurrentLoadAsync(CancellationToken ct = default)
    {
        var response = await httpClient.GetAsync("https://mobifitness.ru/api/v8/clubs/1325.json", ct);
        if (response.IsSuccessStatusCode)
        {
            var jsonStream = await response.Content.ReadAsStreamAsync(ct);
            var jsonDocument = JsonDocument.Parse(jsonStream);
            var currentLoad = jsonDocument.RootElement
                .EnumerateObject()
                .First(property => property.Name == "currentLoad").Value;
            return int.Parse(currentLoad.ToString());
        }
        else
            throw new Exception($"Can't get current load. Status code is {response.StatusCode}");
    }

    public async ValueTask<WorkoutLoad> GetWorkoutLoadAsync(Guid workoutId, CancellationToken ct = default)
    {
        var response = await httpClient.GetAsync($"https://mobifitness.ru/api/v8/schedule/{workoutId}/item.json?clubId=1325", ct);
        if (response.IsSuccessStatusCode)
        {
            var jsonStream = await response.Content.ReadAsStreamAsync(ct);
            var jsonDocument = JsonDocument.Parse(jsonStream);
            var availableSlots = jsonDocument.RootElement.GetProperty("availableSlots").GetInt32();
            var roomCapacity = jsonDocument.RootElement.GetProperty("room").GetProperty("capacity").GetInt32();
            return new(roomCapacity, availableSlots);
        }
        else
            throw new Exception($"Can't get current load. Status code is {response.StatusCode}");
    }

    public async ValueTask<IEnumerable<Workout>> GetDayScheduleAsync(DateTime day, CancellationToken ct = default)
    {
        var year = day.Year;
        var week = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(day, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        var response = await httpClient.GetAsync($"https://mobifitness.ru/api/v8/club/1325/schedule.json?year={year}&week={week}", ct);
        if (response.IsSuccessStatusCode)
        {
            var jsonStream = await response.Content.ReadAsStreamAsync(ct);
            var jsonDocument = JsonDocument.Parse(jsonStream);
            var schedule = jsonDocument.RootElement
                .EnumerateObject()
                .First(property => property.Name == "schedule").Value;
            var wourkoutList = new List<Workout>();
            foreach (var element in schedule.EnumerateArray())
            {
                var start = element.GetProperty("datetime").GetDateTimeOffset().DateTime;
                if (start.Date == day.Date)
                {
                    var length = element.GetProperty("length").GetInt32();
                    var id = element.GetProperty("id").GetGuid();
                    var workoutLoad = await GetWorkoutLoadAsync(id, ct);
                    wourkoutList.Add(
                        new Workout
                        {
                            ExternalId = id,
                            Title = element.GetProperty("activity").GetProperty("title").GetString()!,
                            Start = start,
                            End = start.AddMinutes(length),
                            Load = workoutLoad.RoomCapacity - workoutLoad.AvailableSlots
                        }
                    );
                }
            }
            return wourkoutList.OrderBy(w => w.Start);
        }
        else
            throw new Exception($"Can't get day schedule. Status code is {response.StatusCode}");
    }

    public record struct WorkoutLoad(int RoomCapacity, int AvailableSlots);
}