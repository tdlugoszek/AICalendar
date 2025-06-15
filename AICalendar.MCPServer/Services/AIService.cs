using AICalendar.Shared.Models;
using OpenAI;
using OpenAI.Chat;
using System.Runtime.CompilerServices;

namespace AICalendar.MCPServer.Services;

public interface IAIService
{
    IAsyncEnumerable<string> GetStreamingResponseAsync(string message, CancellationToken cancellationToken = default);
}

public class AIService : IAIService
{
    private readonly ChatClient _chatClient;

    public AIService(OpenAIClient openAIClient)
    {
        _chatClient = openAIClient.GetChatClient("gpt-4");
    }

    public async IAsyncEnumerable<string> GetStreamingResponseAsync(
        string message,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // Use fully qualified OpenAI ChatMessage types to avoid conflicts
        var messages = new List<OpenAI.Chat.ChatMessage>
        {
            new SystemChatMessage("You are a helpful AI assistant for a calendar application. Help users manage their calendar events, schedule meetings, and answer questions about their calendar."),
            new UserChatMessage(message)
        };

        var options = new ChatCompletionOptions
        {
            MaxOutputTokenCount = 1000,  // Updated property name
            Temperature = 0.7f
        };

        await foreach (var update in _chatClient.CompleteChatStreamingAsync(messages, options, cancellationToken))
        {
            if (update.ContentUpdate.Count > 0)
            {
                foreach (var contentPart in update.ContentUpdate)
                {
                    yield return contentPart.Text ?? string.Empty;
                }
            }
        }
    }
}