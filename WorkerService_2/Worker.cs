using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WorkerService_2.Models;

namespace WorkerService_2
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        private HttpClient _client;


        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }


        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _client = new HttpClient();
            _logger.LogInformation("The service has been started.");

            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _client.Dispose();
            _logger.LogInformation("The service has been stopped.");
            return base.StopAsync(cancellationToken);
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)

            {
                var httpClient = HttpClientFactory.Create();

                var url = "https://api.openweathermap.org/data/2.5/onecall?lat=59.12729599999999&lon=15.139514200000007&exclude=hourly,daily,minutely&appid=dd12ce9c149277026665ac05adb84297&units=metric";
                HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(url);
                try
                {
                    if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        var content = httpResponseMessage.Content;
                        var data = await content.ReadAsAsync<Rootobject>();
                        _logger.LogInformation($"The temprature is: {data}");

                        if (data.current.temp > 35)
                        {
                            _logger.LogInformation("The temprature is too high");
                        }
                    }
                }
                catch
                {
                    _logger.LogInformation($"There was an Error: {httpResponseMessage.StatusCode}");
                }


                await Task.Delay(60 * 1000, stoppingToken);
            }
        }
    }

}
