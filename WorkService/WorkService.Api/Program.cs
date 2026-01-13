var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// 🔴 BU SATIR ZORUNLU
builder.Services.AddEndpointsApiExplorer();

// OpenAPI (Microsoft.AspNetCore.OpenApi)
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();