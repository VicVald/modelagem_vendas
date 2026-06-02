using modelagem_vendas.src.repositories.interfaces;

namespace modelagem_vendas.src.controller;

public static class DashboardController
{
    public static void MapDashboardRoutes(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/dashboard")
                       .WithTags("Dashboard");

        group.MapGet("/melhores-produtos-quantidade", async (IDashboardRepository repo) =>
        {
            try
            {
                var data = await repo.GetMelhoresProdutosQuantidadeAsync();
                return Results.Ok(data);
            }
            catch (System.Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });

        group.MapGet("/melhores-produtos-valor", async (IDashboardRepository repo) =>
        {
            try
            {
                var data = await repo.GetMelhoresProdutosValorAsync();
                return Results.Ok(data);
            }
            catch (System.Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });

        group.MapGet("/ingredientes-mais-utilizados", async (IDashboardRepository repo) =>
        {
            try
            {
                var data = await repo.GetIngredientesMaisUtilizadosAsync();
                return Results.Ok(data);
            }
            catch (System.Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });
    }
}
