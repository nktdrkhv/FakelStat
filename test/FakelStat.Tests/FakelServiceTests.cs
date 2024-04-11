using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using FakelStat.Services;
using FakelStat.Options;
using Microsoft.Extensions.Configuration;

namespace FakelStat.Tests;

public class FakelServiceTests
{
    private readonly IConfiguration _conf;
    private readonly HttpClient _client;

    public FakelServiceTests()
    {
        _conf = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddUserSecrets<FakelService>()
            .Build();
        _client = new HttpClient(new HttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.GZip
        });
        var fakelOptions = _conf.GetSection("Fakel").Get<FakelOptions>();
        _client.DefaultRequestVersion = HttpVersion.Version20;
        foreach (var (title, value) in fakelOptions!.Headers)
            _client.DefaultRequestHeaders.Add(title, value);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", fakelOptions.Token);
    }

    [Fact]
    public async void CanGetCurrentLoad()
    {
        var fakelService = new FakelService(_client);
        var currentLoad = await fakelService.GetCurrentLoadAsync();
        Debug.WriteLine($"Current load is {currentLoad}");
        Assert.True(true);
    }

    [Fact]
    public async void CanGetTodaySchedule()
    {
        var fakelService = new FakelService(_client);
        var currentSchedule = await fakelService.GetDayScheduleAsync(DateTime.Now);
        Debug.WriteLine($"There are {currentSchedule.Count()} events");
        Assert.True(true);
    }
}