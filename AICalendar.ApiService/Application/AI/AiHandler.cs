using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using System.Collections.Generic;
using System.Threading;

namespace AICalendar.ApiService.Application.AI
{
    internal sealed class AiHandler(IChatClient client, ChatOptions options )
    {
      

        public IAsyncEnumerable<ChatResponseUpdate> HandleChatAsync(
            Guid currentUserId,
            string prompt,

            CancellationToken cancellationToken = default)
        {
            var messages = new List<ChatMessage>
            {
                new(ChatRole.User, prompt)
            };

            return client.GetStreamingResponseAsync(messages, options, cancellationToken);
        }
    }
}
