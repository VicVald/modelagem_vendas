using modelagem_vendas.src.models;
using modelagem_vendas.src.repositories.interfaces;

namespace modelagem_vendas.src.controller;

public static class ClientController
{
    public static void MapClientRoutes(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/clientes")
                       .WithTags("Clientes");

        group.MapPost("/", async (ClienteInput input, IClienteRepository repo) =>
        {
            var id = await repo.AddClientAsync(input.Nome, input.Cpf);
            return Results.Created($"/api/clientes/{id}", new { id, input.Nome });
        });

        group.MapGet("/", async (IClienteRepository repo) =>
        {
            var clientes = await repo.GetAllClientesAsync();
            return Results.Ok(clientes);
        });

        group.MapGet("/{id:int}", async (int id, IClienteRepository repo) =>
        {
            var cliente = await repo.GetClienteByIdAsync(id);
            return cliente is not null ? Results.Ok(cliente) : Results.NotFound();
        });

        group.MapGet("/cpf/{cpf}", async (string cpf, IClienteRepository repo) =>
        {
            var cliente = await repo.GetClienteByCpfAsync(cpf);
            return cliente is not null ? Results.Ok(cliente) : Results.NotFound();
        });

        group.MapPut("/{id:int}", async (int id, UpdateClienteInput input, IClienteRepository repo) =>
        {
            var success = await repo.UpdateClienteAsync(id, input.Nome);
            return success ? Results.Ok(new { message = "Cliente atualizado com sucesso" }) : Results.NotFound();
        });

        group.MapDelete("/{id:int}", async (int id, IClienteRepository repo) =>
        {
            var success = await repo.DeleteClienteAsync(id);
            return success ? Results.Ok(new { message = "Cliente removido com sucesso" }) : Results.NotFound();
        });
    }
}

public record ClienteInput(string Nome, string Cpf);
public record UpdateClienteInput(string Nome);
