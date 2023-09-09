namespace HaWeb.BackgroundTask;

using System.Threading.Channels;

public interface IBackgroundTaskQueue {
    ValueTask QueueBackgroundWorkItemAsync(Func<CancellationToken, ValueTask> workItem);
    ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken);
}

public interface IMonitorLoop {
    public void StartMonitorLoop();
}

public class BackgroundTaskQueue : IBackgroundTaskQueue {
    private readonly Channel<Func<CancellationToken, ValueTask>> _queue;

    public BackgroundTaskQueue(int capacity) {
        var options = new BoundedChannelOptions(capacity) {
            FullMode = BoundedChannelFullMode.Wait
        };
        _queue = Channel.CreateBounded<Func<CancellationToken, ValueTask>>(options);
    }

    public async ValueTask QueueBackgroundWorkItemAsync(Func<CancellationToken, ValueTask> workItem)  {
        if (workItem == null) {
            throw new ArgumentNullException(nameof(workItem));
        }
        await _queue.Writer.WriteAsync(workItem);
    }

    public async ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(
        CancellationToken cancellationToken
    ) {
        var workItem = await _queue.Reader.ReadAsync(cancellationToken);
        return workItem;
    }
}

public class QueuedHostedService : BackgroundService {
    private readonly ILogger<QueuedHostedService> _logger;
    public IBackgroundTaskQueue TaskQueue { get; }

    public QueuedHostedService(IBackgroundTaskQueue taskQueue, ILogger<QueuedHostedService> logger) {
        TaskQueue = taskQueue;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        await BackgroundProcessing(stoppingToken);
    }

    private async Task BackgroundProcessing(CancellationToken stoppingToken) {
        while (!stoppingToken.IsCancellationRequested) {
            var workItem = await TaskQueue.DequeueAsync(stoppingToken);

            try {
                await workItem(stoppingToken);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error occurred executing {WorkItem}.", nameof(workItem));
            }
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken) {
        _logger.LogInformation("Queued Hosted Service is stopping.");

        await base.StopAsync(stoppingToken);
    }
}

public class MonitorLoop : IMonitorLoop {
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly ILogger _logger;
    private readonly CancellationToken _cancellationToken;

    public MonitorLoop(
        IBackgroundTaskQueue taskQueue,
        ILogger<MonitorLoop> logger,
        IHostApplicationLifetime applicationLifetime
    ) {
        _taskQueue = taskQueue;
        _logger = logger;
        _cancellationToken = applicationLifetime.ApplicationStopping;
    }

    public void StartMonitorLoop() {
        _logger.LogInformation("MonitorAsync Loop is starting.");

        // Run a console user input loop in a background thread
        Task.Run(async () => await MonitorAsync());
    }

    private async ValueTask MonitorAsync() {
        while (!_cancellationToken.IsCancellationRequested) {
            var keyStroke = Console.ReadKey();

            if (keyStroke.Key == ConsoleKey.W) {
                // Enqueue a background work item
                await _taskQueue.QueueBackgroundWorkItemAsync(BuildWorkItem);
            }
        }
    }

    private async ValueTask BuildWorkItem(CancellationToken token) {
        // Simulate three 5-second tasks to complete
        // for each enqueued work item

        int delayLoop = 0;
        var guid = Guid.NewGuid().ToString();

        _logger.LogInformation("Queued Background Task {Guid} is starting.", guid);

        while (!token.IsCancellationRequested && delayLoop < 3) {
            try {
                await Task.Delay(TimeSpan.FromSeconds(5), token);
            }
            catch (OperationCanceledException) {
                // Prevent throwing if the Delay is cancelled
            }

            delayLoop++;

            _logger.LogInformation(
                "Queued Background Task {Guid} is running. " + "{DelayLoop}/3",
                guid,
                delayLoop
            );
        }

        if (delayLoop == 3) {
            _logger.LogInformation("Queued Background Task {Guid} is complete.", guid);
        }
        else {
            _logger.LogInformation("Queued Background Task {Guid} was cancelled.", guid);
        }
    }
}
