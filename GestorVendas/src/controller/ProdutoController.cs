using modelagem_vendas.src.models;
using modelagem_vendas.src.repositories.interfaces;

namespace modelagem_vendas.src.controller;

public static class ProdutoController
{
    public static void MapProdutoRoutes(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/produtos")
                       .WithTags("Produtos");

        // --- Basic Product Endpoints ---

        group.MapPost("/", async (ProdutoInput input, IProdutoRepository repo) =>
        {
            try
            {
                var id = await repo.AddProdutoAsync(input.Valor, input.MateriaisId);
                return Results.Created($"/api/produtos/{id}", new { id, input.Valor, input.MateriaisId });
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });

        group.MapGet("/", async (IProdutoRepository repo) =>
        {
            try
            {
                var produtos = await repo.GetAllProdutosAsync();
                return Results.Ok(produtos);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });

        group.MapGet("/{id:int}", async (int id, IProdutoRepository repo) =>
        {
            try
            {
                var produto = await repo.GetProdutoByIdAsync(id);
                return produto is not null ? Results.Ok(produto) : Results.NotFound(new { message = "Produto não encontrado" });
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });

        group.MapPut("/{id:int}", async (int id, ProdutoInput input, IProdutoRepository repo) =>
        {
            try
            {
                var success = await repo.UpdateProdutoAsync(id, input.Valor, input.MateriaisId);
                return success ? Results.Ok(new { message = "Produto atualizado com sucesso" }) : Results.NotFound(new { message = "Produto não encontrado" });
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });

        group.MapDelete("/{id:int}", async (int id, IProdutoRepository repo) =>
        {
            try
            {
                var success = await repo.DeleteProdutoAsync(id);
                return success ? Results.Ok(new { message = "Produto removido com sucesso" }) : Results.NotFound(new { message = "Produto não encontrado" });
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });

        // --- Product Ingredients / Materials Composition Endpoints ---

        group.MapPost("/{id:int}/materiais", async (int id, AddMaterialInput input, IProdutoRepository repo) =>
        {
            try
            {
                await repo.LinkMaterialProdutoAsync(input.MateriaisId, id);
                return Results.Ok(new { message = "Material associado ao produto com sucesso" });
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });

        group.MapDelete("/{id:int}/materiais/{materialId:int}", async (int id, int materialId, IProdutoRepository repo) =>
        {
            try
            {
                await repo.RemoveMaterialProdutoAsync(id, materialId);
                return Results.Ok(new { message = "Associação removida com sucesso" });
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });

        group.MapGet("/{id:int}/materiais", async (int id, IProdutoRepository repo) =>
        {
            try
            {
                var materiais = await repo.GetMateriaisByProdutoAsync(id);
                return Results.Ok(materiais);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });

        group.MapGet("/material/{materialId:int}", async (int materialId, IProdutoRepository repo) =>
        {
            try
            {
                var produtos = await repo.GetProductsByMaterialAsync(materialId);
                return Results.Ok(produtos);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });
    }
}

public record ProdutoInput(decimal Valor, int? MateriaisId = null);
public record AddMaterialInput(int MateriaisId);

