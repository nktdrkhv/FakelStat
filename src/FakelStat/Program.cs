using System.Net;
using System.Net.Http.Headers;
using FakelStat.Jobs;
using FakelStat.Options;
using FakelStat.Repositories;
using FakelStat.Services;
using FakelStat.Helpers;
using LinkDotNet.NCronJob;
using Microsoft.Extensions.Options;
using PetaPoco;
using PetaPoco.Providers;
using Telegram.Bot;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<FakelOptions>(builder.Configuration.GetSection(FakelOptions.SECTION));

//builder.Services.AddScoped<ProxyService>();
builder.Services.AddScoped<FakelService>();
builder.Services.AddScoped<TelegramService>();
builder.Services.AddHostedService<TelegramPollingService>();

builder.Services.AddScoped<IGeneratedPlotRepository, SqlGeneratedPlotRepository>();
builder.Services.AddScoped<IMomentumLoadRepository, SqlMomentumLoadRepository>();

builder.Services.AddHttpClient<FakelService>()
                .ConfigureHttpClient((services, client) =>
                {
                    var fakelOptions = services.GetRequiredService<IOptions<FakelOptions>>().Value;
                    client.DefaultRequestVersion = HttpVersion.Version20;
                    foreach (var (title, value) in fakelOptions.Headers)
                        client.DefaultRequestHeaders.Add(title, value);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", fakelOptions.Token);
                })
                .ConfigurePrimaryHttpMessageHandler(services =>
                {
                    //var proxyService = services.GetRequiredService<ProxyService>();
                    var handler = new HttpClientHandler()
                    {
                        AutomaticDecompression = DecompressionMethods.GZip
                        /*Proxy = proxyService.GetAwalableProxy()*/
                    };
                    return handler;
                });

builder.Services.AddHttpClient("telegram_bot_client")
                .AddTypedClient<ITelegramBotClient>((httpClient, _) =>
                {
                    TelegramBotClientOptions options = new(builder.Configuration["Telegram:Token"]!);
                    return new TelegramBotClient(options, httpClient);
                });

builder.Services.AddCronJob<GetCurrentLoad>(options => options.CronExpression = "10/10 0-15 * * 1-5");
builder.Services.AddCronJob<GetCurrentLoad>(options => options.CronExpression = "10/10 2-13 * * 6-0");
builder.Services.AddCronJob<GetTodaySchedule>(options => options.CronExpression = "0 13 * * *");

builder.Services.AddSingleton(_ => DatabaseConfiguration.Build()
        .UsingConnectionString(builder.Configuration.GetConnectionString("FakelDbSQlite"))
        .UsingProvider<SQLiteDatabaseProvider>()
        .UsingDefaultMapper<ConventionMapper>(PetaPocoHelper.ConfigureMapper)
        .WithoutAutoSelect());
builder.Services.AddScoped(sp => sp.GetRequiredService<IDatabaseBuildConfiguration>().Create());

var host = builder.Build();
host.Run();