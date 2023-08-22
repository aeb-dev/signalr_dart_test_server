using MessagePack;
using Microsoft.AspNetCore.SignalR;

namespace SignalR.Test.Hubs;

public class TestHub : Hub<IChatClient>
{
    private int _messageId = 0;

    private readonly ILogger _logger;

    public TestHub(ILogger<TestHub> logger)
    {
        _logger = logger;
    }

    public async Task<ServerResponse> AskAge(ClientRequest clientRequest)
    {
        _logger.LogInformation($"Client requested: {clientRequest}");

        ClientResponse clientResponse = await Clients.Caller.Calculate(new ServerRequest()
        {
            MessageId = Interlocked.Increment(ref _messageId),
            Message = "My age is 2 x 4",
            DateTime = DateTime.UtcNow,
            HelperText = "https://en.wikipedia.org/wiki/Multiplication",
            PossibleAnswers = new List<object?> { null, "", 1, 4, 8 }
        });

        _logger.LogInformation($"Client response: {clientResponse}");

        return new ServerResponse()
        {
            MessageId = Interlocked.Increment(ref _messageId),
            MessageIdOfRequest = clientRequest.MessageId,
            Message = "Clever!",
            DateTime = DateTime.UtcNow,
            Request = clientRequest,
        };
    }

    public async Task Greet()
    {
        _logger.LogInformation("We have been greeted");
    }

    public async IAsyncEnumerable<int> Count(IAsyncEnumerable<int> stream)
    {
        await foreach (int i in stream)
        {
            yield return i * 2;
        }
    }
}

public interface IChatClient
{
    Task<ClientResponse> Calculate(ServerRequest serverRequest);
    Task ReceiveMessage(string user, string message);
    Task<IAsyncEnumerable<int>> CountBack();
}

[MessagePackObject]
public record ClientRequest
{
    [Key(0)]
    public int MessageId { get; set; }

    [Key(1)]
    public required string Message { get; set; }

    [Key(2)]
    public DateTime DateTime { get; set; }
}

[MessagePackObject]
public record ServerResponse
{
    [Key(0)]
    public int MessageId { get; set; }

    [Key(1)]
    public int MessageIdOfRequest { get; set; }

    [Key(2)]
    public required string Message { get; set; }

    [Key(3)]
    public DateTime DateTime { get; set; }

    [Key(4)]
    public required ClientRequest Request { get; set; }

    [Key(5)]
    public string? TraceId { get; set; }
}

[MessagePackObject]
public record ServerRequest
{
    [Key(0)]
    public int MessageId { get; set; }

    [Key(1)]
    public required string Message { get; set; }

    [Key(2)]
    public DateTime DateTime { get; set; }

    [Key(3)]
    public string? HelperText { get; set; }

    [Key(4)]
    public List<object?>? PossibleAnswers { get; set; }
}

[MessagePackObject]
public record ClientResponse
{
    [Key(0)]
    public int MessageId { get; set; }

    [Key(1)]
    public int MessageIdOfRequest { get; set; }

    [Key(2)]
    public required string Message { get; set; }

    [Key(3)]
    public DateTime DateTime { get; set; }

    [Key(4)]
    public double ResponseConfidence { get; set; }

    [Key(5)]
    public Dictionary<string, object?>? BagOfData { get; set; }

    [Key(6)]
    public required ServerRequest Request { get; set; }

    [Key(7)]
    public string? TraceId { get; set; }
}