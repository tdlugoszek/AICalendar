using Microsoft.AspNetCore.Builder;

namespace AICalendar.ApiService.Application.AI
{
    internal static class AiModule
    {
        public static IEndpointRouteBuilder MapAiRoutes(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup(prefix: "/api/v1/ai")
                .WithTags("AI")
                .WithOpenApi();

            group.MapAI();

            return group;
        }
    }
}
