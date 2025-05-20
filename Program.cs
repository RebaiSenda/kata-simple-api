using KataSimpleAPI.Services;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
// Configuration RabbitMQ
//builder.Services.Configure<RabbitMQConfig>(builder.Configuration.GetSection("RabbitMQ"));
//builder.Services.AddSingleton<IBookingConsumerService, BookingConsumerService>();
//builder.Services.AddHostedService<BookingConsumerHostedService>();

//Version2 config RabbitMQ
// Configuration RabbitMQ
builder.Services.Configure<RabbitMQConfig>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddHostedService<RabbitMQConsumerService>();
builder.Services.AddScoped<IFakeBookingProcessor, FakeBookingProcessor>();
// Ajoutez la configuration d'authentification
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "https://localhost:5001";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false
        };
    });

// Configuration de l'autorisation
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("KataSimpleAPI", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "KataSimpleAPI");
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

// Ajoutez ces lignes
app.UseAuthentication();
app.UseAuthorization();

// Modifiez cette ligne pour appliquer l'autorisation
app.MapControllers();

app.Run();
