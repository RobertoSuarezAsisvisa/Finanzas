using FinanzasMCP.Application.Categories.Commands;
using FinanzasMCP.Application.Categories.Handlers;
using FinanzasMCP.Application.Categories.Queries;
using FinanzasMCP.Application.Tags.Commands;
using FinanzasMCP.Application.Tags.Handlers;
using FinanzasMCP.Application.Tags.Queries;

namespace FinanzasMCP.McpServer.Api;

public static class CatalogEndpoints
{
    public static void MapCatalogEndpoints(this RouteGroupBuilder api)
    {
        MapCategories(api.MapGroup("/categories"));
        MapTags(api.MapGroup("/tags"));
    }

    private static void MapCategories(RouteGroupBuilder group)
    {
        group.MapGet("", async (GetCategoriesHandler handler, CancellationToken ct) =>
            Results.Ok(await handler.Handle(new GetCategoriesQuery(), ct)));

        group.MapPost("", async (CreateCategoryRequest request, CreateCategoryHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new CreateCategoryCommand(request.Name, request.Type, request.Icon, request.ParentId, request.IsSystem), ct);
            return Results.Created($"/api/v1/categories/{result.Id}", result);
        });

        group.MapPut("{id:guid}", async (Guid id, UpdateCategoryRequest request, UpdateCategoryHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new UpdateCategoryCommand(id, request.Name, request.Type, request.Icon, request.ParentId), ct);
            return Results.Ok(result);
        });

        group.MapDelete("{id:guid}", async (Guid id, DeleteCategoryHandler handler, CancellationToken ct) =>
        {
            await handler.Handle(new DeleteCategoryCommand(id), ct);
            return Results.NoContent();
        });
    }

    private static void MapTags(RouteGroupBuilder group)
    {
        group.MapGet("", async (GetTagsHandler handler, CancellationToken ct) =>
            Results.Ok(await handler.Handle(new GetTagsQuery(), ct)));

        group.MapPost("", async (CreateTagRequest request, CreateTagHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new CreateTagCommand(request.Name, request.Color), ct);
            return Results.Created($"/api/v1/tags/{result.Id}", result);
        });

        group.MapPut("{id:guid}", async (Guid id, UpdateTagRequest request, UpdateTagHandler handler, CancellationToken ct) =>
        {
            var result = await handler.Handle(new UpdateTagCommand(id, request.Name, request.Color), ct);
            return Results.Ok(result);
        });

        group.MapDelete("{id:guid}", async (Guid id, DeleteTagHandler handler, CancellationToken ct) =>
        {
            await handler.Handle(new DeleteTagCommand(id), ct);
            return Results.NoContent();
        });
    }
}
