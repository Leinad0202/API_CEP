using API_CEP.Data;
using API_CEP.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();


// registra o DbContext e cache como já tinha
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddMemoryCache();

// registra o HttpClient e a implementação do ICepService
builder.Services.AddHttpClient<ICepService, CepService>();

// Registrar o serviço CepService
builder.Services.AddScoped<CepService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var retry = 0;
    while (retry < 10)
    {
        try
        {
            db.Database.OpenConnection();
            db.Database.CloseConnection();
            break;
        }
        catch
        {
            retry++;
            Thread.Sleep(2000);
        }
    }

    db.Database.Migrate();
}


// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API CEP v1");
});

app.UseAuthorization();

app.MapControllers();

app.Run();
