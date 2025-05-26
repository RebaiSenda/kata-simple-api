using KataSimpleAPI.Services;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using static KataSimpleAPI.Services.SmtpEmailSender;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Configuration RabbitMQ
builder.Services.Configure<RabbitMQConfig>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddHostedService<RabbitMQConsumerService>();
builder.Services.AddScoped<IFakeBookingProcessor, FakeBookingProcessor>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ISmtpEmailSender, FakeEmailSender>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
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
