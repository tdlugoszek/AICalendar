using Microsoft.Extensions.AI;

namespace AICalendar.ApiService.Application.AI
{
    internal static class AiEvent
    {
        public static RouteGroupBuilder MapAI(this RouteGroupBuilder routes)
        {
            routes.MapPost("/", Handler)
                .WithName("Ai")
                .WithSummary("AI");

            return routes;
        }

        private static IAsyncEnumerable<ChatResponseUpdate> Handler(AiHandler handler,
            Request request,
            CancellationToken cancellationToken)
        {
            Guid.TryParse("TODO", out var userId);
            return handler.HandleChatAsync(userId, request.Prompt, cancellationToken);
        }


    }

    internal record Request(string Prompt);
}
