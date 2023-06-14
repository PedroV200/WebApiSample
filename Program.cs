using Auth0.AspNetCore.Authentication; //requiero el paquete de auth0 instalado previamente x nuget

// using de politicas CORS para consultar api local desde localhost de React
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure();

// se agrega configuracion de auth0
builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = "dev-7qwkde4r318nfwz7.us.auth0.com";
    options.ClientId = "RnSKO2i02EoE9cpB7gtY8i0m2WstwGpo";
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Agrega el middleware CORS antes de app.Run()
app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

// habilito autenticacion y autorizacion de auth0
app.UseAuthentication();
app.UseAuthorization();

app.UseAuthorization();

app.MapControllers();

app.Run();
