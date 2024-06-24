using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using loja.models;
using loja.data;
using loja.services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Loja.models;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Configuração do serviço LojaDbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<LojaDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36))));

// Adicione serviços ao contêiner.
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<FornecedorService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<VendaService>();

// Configuração de autenticação JWT
var key = Encoding.ASCII.GetBytes("sua-chave-secreta-mais-longa-aqui");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

// Adicione os serviços de autorização
builder.Services.AddAuthorization();

var app = builder.Build();

// Configurar as requisições HTTP 
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthentication(); // Adicione esta linha
app.UseAuthorization(); // Adicione esta linha

// Método para gerar o token JWT
string GenerateToken(string email)
{
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.ASCII.GetBytes("sua-chave-secreta-mais-longa-aqui");
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new[] { new Claim("email", email) }),
        Expires = DateTime.UtcNow.AddHours(1),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };
    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
}

// Endpoint de login
app.MapPost("/login", async (HttpContext context, UsuarioService usuarioService) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();
    var json = JsonDocument.Parse(body);
    var email = json.RootElement.GetProperty("email").GetString();
    var senha = json.RootElement.GetProperty("senha").GetString();

    if (email is null || senha is null)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsync("Email e senha são obrigatórios");
        return;
    }

    var usuario = await usuarioService.GetUsuarioByEmailAndSenhaAsync(email, senha);
    if (usuario == null)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Usuário ou senha inválidos");
        return;
    }

    var token = GenerateToken(email);
    await context.Response.WriteAsync(token);
});

// Protegendo rotas com autenticação JWT
app.MapGet("/produtos", async (ProductService productService) =>
{
    var produtos = await productService.GetAllProductsAsync();
    return Results.Ok(produtos);
}).RequireAuthorization();

app.MapGet("/produtos/{id}", async (int id, ProductService productService) =>
{
    var produto = await productService.GetProductByIdAsync(id);
    if (produto == null)
    {
        return Results.NotFound($"Product with ID {id} not found.");
    }
    return Results.Ok(produto);
}).RequireAuthorization();

app.MapPost("/produtos", async (Produto produto, ProductService productService) =>
{
    await productService.AddProductAsync(produto);
    return Results.Created($"/produtos/{produto.Id}", produto);
}).RequireAuthorization();

app.MapPut("/produtos/{id}", async (int id, Produto produto, ProductService productService) =>
{
    if (id != produto.Id)
    {
        return Results.BadRequest("Product ID mismatch.");
    }

    await productService.UpdateProductAsync(produto);
    return Results.Ok();
}).RequireAuthorization();

app.MapDelete("/produtos/{id}", async (int id, ProductService productService) =>
{
    await productService.DeleteProductAsync(id);
    return Results.Ok();
}).RequireAuthorization();

app.MapGet("/fornecedores", async (FornecedorService fornecedorService) =>
{
    var fornecedores = await fornecedorService.GetAllFornecedoresAsync();
    return Results.Ok(fornecedores);
}).RequireAuthorization();

app.MapGet("/fornecedores/{id}", async (int id, FornecedorService fornecedorService) =>
{
    var fornecedor = await fornecedorService.GetFornecedorByIdAsync(id);
    if (fornecedor == null)
    {
        return Results.NotFound($"Fornecedor with ID {id} not found.");
    }
    return Results.Ok(fornecedor);
}).RequireAuthorization();

app.MapPost("/fornecedores", async (Fornecedor fornecedor, FornecedorService fornecedorService) =>
{
    await fornecedorService.AddFornecedorAsync(fornecedor);
    return Results.Created($"/fornecedor/{fornecedor.Id}", fornecedor);
}).RequireAuthorization();

app.MapPut("/fornecedores/{id}", async (int id, Fornecedor fornecedor, FornecedorService fornecedorService) =>
{
    if (id != fornecedor.Id)
    {
        return Results.BadRequest("Fornecedor ID mismatch.");
    }

    await fornecedorService.UpdateFornecedorAsync(fornecedor);
    return Results.Ok();
}).RequireAuthorization();

app.MapDelete("/fornecedores/{id}", async (int id, FornecedorService fornecedorService) =>
{
    await fornecedorService.DeleteFornecedorAsync(id);
    return Results.Ok();
}).RequireAuthorization();

app.MapGet("/usuarios", async (UsuarioService usuarioService) =>
{
    var usuarios = await usuarioService.GetAllUsuariosAsync();
    return Results.Ok(usuarios);
}).RequireAuthorization();

app.MapGet("/usuarios/{id}", async (int id, UsuarioService usuarioService) =>
{
    var usuario = await usuarioService.GetUsuarioByIdAsync(id);
    if (usuario == null)
    {
        return Results.NotFound($"Usuario with ID {id} not found.");
    }
    return Results.Ok(usuario);
}).RequireAuthorization();

app.MapPost("/usuarios", async (Usuario usuario, UsuarioService usuarioService) =>
{
    await usuarioService.AddUsuarioAsync(usuario);
    return Results.Created($"/usuario/{usuario.Id}", usuario);
}).RequireAuthorization();

app.MapPut("/usuarios/{id}", async (int id, Usuario usuario, UsuarioService usuarioService) =>
{
    if (id != usuario.Id)
    {
        return Results.BadRequest("Usuario ID mismatch.");
    }

    await usuarioService.UpdateUsuarioAsync(usuario);
    return Results.Ok();
}).RequireAuthorization();

app.MapDelete("/usuarios/{id}", async (int id, UsuarioService usuarioService) =>
{
    await usuarioService.DeleteUsuarioAsync(id);
    return Results.Ok();
}).RequireAuthorization();

// Endpoints para Vendas
app.MapPost("/vendas", async (Venda venda, VendaService vendaService) =>
{
    var result = await vendaService.AddVendaAsync(venda);
    if (result == null)
    {
        return Results.BadRequest("Cliente ou Produto inválido.");
    }
    return Results.Created($"/vendas/{venda.Id}", venda);
}).RequireAuthorization();

app.MapGet("/vendas/produto/{produtoId}/detalhada", async (int produtoId, VendaService vendaService) =>
{
    var vendas = await vendaService.GetVendasByProdutoIdDetalhadaAsync(produtoId);
    return Results.Ok(vendas);
}).RequireAuthorization();

app.MapGet("/vendas/produto/{produtoId}/sumarizada", async (int produtoId, VendaService vendaService) =>
{
    var vendas = await vendaService.GetVendasByProdutoIdSumarizadaAsync(produtoId);
    return Results.Ok(vendas);
}).RequireAuthorization();

app.MapGet("/vendas/cliente/{clienteId}/detalhada", async (int clienteId, VendaService vendaService) =>
{
    var vendas = await vendaService.GetVendasByClienteIdDetalhadaAsync(clienteId);
    return Results.Ok(vendas);
}).RequireAuthorization();

app.MapGet("/vendas/cliente/{clienteId}/sumarizada", async (int clienteId, VendaService vendaService) =>
{
    var vendas = await vendaService.GetVendasByClienteIdSumarizadaAsync(clienteId);
    return Results.Ok(vendas);
}).RequireAuthorization();

app.Run();
