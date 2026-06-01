using modelagem_vendas.src.models;
using modelagem_vendas.src.repositories.interfaces;

namespace modelagem_vendas.src.controller;

public static class MaterialController
{
    public static void MapMaterialRoutes(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/materiais")
                       .WithTags("Materiais");

        group.MapPost("/", async (MaterialInput input, IMaterialRepository repo) =>
        {
            var id = await repo.AddMaterialAsync(input.Nome, input.Quantidade, input.Medida);
            return Results.Created($"/api/materiais/{id}", new { id, input.Nome, input.Quantidade, input.Medida });
        });

        group.MapGet("/", async (IMaterialRepository repo) =>
        {
            var materiais = await repo.GetAllMateriaisAsync();
            return Results.Ok(materiais);
        });

        group.MapGet("/{id:int}", async (int id, IMaterialRepository repo) =>
        {
            var material = await repo.GetMaterialByIdAsync(id);
            return material is not null ? Results.Ok(material) : Results.NotFound();
        });

        group.MapPut("/{id:int}", async (int id, MaterialInput input, IMaterialRepository repo) =>
        {
            var success = await repo.UpdateMaterialAsync(id, input.Nome, input.Quantidade, input.Medida);
            return success ? Results.Ok(new { message = "Material atualizado com sucesso" }) : Results.NotFound();
        });

        group.MapDelete("/{id:int}", async (int id, IMaterialRepository repo) =>
        {
            var success = await repo.DeleteMaterialAsync(id);
            return success ? Results.Ok(new { message = "Material removido com sucesso" }) : Results.NotFound();
        });

        group.MapGet("/baixo-estoque", async (int? threshold, IMaterialRepository repo) =>
        {
            var materiais = await repo.GetLowStockMaterialsAsync(threshold ?? 10);
            return Results.Ok(materiais);
        });

        group.MapPost("/{id:int}/ajustar-estoque", async (int id, AdjustStockInput input, IMaterialRepository repo) =>
        {
            var success = await repo.AdjustMaterialStockAsync(id, input.Delta, input.Tipo);
            return success ? Results.Ok(new { message = "Estoque do material ajustado com sucesso e histórico registrado" }) : Results.NotFound();
        });
    }
}

public record MaterialInput(string Nome, int Quantidade, string Medida);
public record AdjustStockInput(int Delta, string Tipo);
