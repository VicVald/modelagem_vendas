using modelagem_vendas.src.models;
using modelagem_vendas.src.repositories.interfaces;

namespace modelagem_vendas.src.controller;

public static class PedidoController
{
    public static void MapPedidoRoutes(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/pedidos")
                       .WithTags("Pedidos");

        group.MapPost("/", async (PedidoInput input, IPedidoRepository repo) =>
        {
            try
            {
                var id = await repo.CreateOrderAsync(input.ClienteId, input.ProdutoIds);
                return Results.Created($"/api/pedidos/{id}", new { id, input.ClienteId, input.ProdutoIds });
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        });

        group.MapGet("/{id:int}", async (int id, IPedidoRepository repo) =>
        {
            var order = await repo.GetOrderByIdAsync(id);
            return order is not null ? Results.Ok(order) : Results.NotFound();
        });

        group.MapGet("/cliente/{clienteId:int}", async (int clienteId, IPedidoRepository repo) =>
        {
            var orders = await repo.ListOrdersByClientAsync(clienteId);
            return Results.Ok(orders);
        });

        group.MapDelete("/{id:int}", async (int id, IPedidoRepository repo) =>
        {
            var success = await repo.DeleteOrderAsync(id);
            return success ? Results.Ok(new { message = "Pedido removido com sucesso" }) : Results.NotFound();
        });

        group.MapGet("/{id:int}/detalhes", async (int id, IPedidoRepository repo) =>
        {
            var details = await repo.GetOrderDetailsAsync(id);
            return details.Any() ? Results.Ok(details) : Results.NotFound();
        });

        group.MapGet("/relatorio-vendas", async (DateTime startDate, DateTime endDate, IPedidoRepository repo) =>
        {
            var summary = await repo.GetSalesSummaryAsync(startDate, endDate);
            return Results.Ok(summary);
        });
    }
}

public record PedidoInput(int ClienteId, List<int> ProdutoIds);
