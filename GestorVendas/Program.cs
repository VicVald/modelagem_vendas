using modelagem_vendas.src.controller;
using modelagem_vendas.src.repositories;
using modelagem_vendas.src.repositories.interfaces;

var builder = WebApplication.CreateBuilder(args);

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IMaterialRepository, MaterialRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();

// Map Sprint 1 Routes
app.MapClientRoutes();
app.MapMaterialRoutes();
app.MapProdutoRoutes();
app.MapPedidoRoutes();
app.MapDashboardRoutes();

app.Run();

