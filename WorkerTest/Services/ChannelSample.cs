using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace WorkerTest.Services
{
    public class ChannelSample : BackgroundService
    {
        private readonly ILogger<ChannelSample> _logger;

        public ChannelSample(ILogger<ChannelSample> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("GracePeriodManagerService is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                await SingleProducerSingleConsumer();
            }
        }

        public async Task SingleProducerSingleConsumer()
        {
            var channel = Channel.CreateUnbounded<string>();

            // In this example, the consumer keeps up with the producer

            var producer1 = new Producer(channel.Writer, 1, 2000, _logger);
            var consumer1 = new Consumer(channel.Reader, 1, 1500, _logger);

            Task consumerTask1 = consumer1.ConsumeData(); // begin consuming
            Task producerTask1 = producer1.BeginProducing(); // begin producing

            await producerTask1.ContinueWith(_ => channel.Writer.Complete());

            await consumerTask1;
        }
    }

    public class Producer
    {
        private readonly ChannelWriter<string> _writer;
        private readonly int _identifier;
        private readonly int _delay;
        private readonly ILogger _logger;

        public Producer(ChannelWriter<string> writer, int identifier, int delay, ILogger<ChannelSample> logger)
        {
            _writer = writer;
            _identifier = identifier;
            _delay = delay;
            _logger = logger;
        }

        public async Task BeginProducing()
        {
            _logger.LogInformation($"PRODUCER ({_identifier}): Starting");

            for (var i = 0; i < 10; i++)
            {
                await Task.Delay(_delay); // simulate producer building/fetching some data

                var msg = $"P{_identifier} - {DateTime.UtcNow:G}";

                _logger.LogInformation($"PRODUCER ({_identifier}): Creating {msg}");

                await _writer.WriteAsync(msg);
            }

            _logger.LogInformation($"PRODUCER ({_identifier}): Completed");
        }
    }

    public class Consumer
    {
        private readonly ChannelReader<string> _reader;
        private readonly int _identifier;
        private readonly int _delay;
        private readonly ILogger _logger;

        public Consumer(ChannelReader<string> reader, int identifier, int delay, ILogger<ChannelSample> logger)
        {
            _reader = reader;
            _identifier = identifier;
            _delay = delay;
            _logger = logger;
        }

        public async Task ConsumeData()
        {
            _logger.LogInformation($"CONSUMER ({_identifier}): Starting");

            while (await _reader.WaitToReadAsync())
            {
                if (_reader.TryRead(out var timeString))
                {
                    await Task.Delay(_delay); // simulate processing time

                    _logger.LogInformation($"CONSUMER ({_identifier}): Consuming {timeString}");
                }
            }

            _logger.LogInformation($"CONSUMER ({_identifier}): Completed");
        }
    }
}
