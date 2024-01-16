using StockControlApi.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using StockControlApi.Application.Services.Interfaces;
using StockControlApi.Application.Services;
using StockControlApi.Domain.Entities;
using StockControlApi.Application.WebSocketManager.Handlers;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StockControlApi.Application.WebSocketManager;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure the HTTP request pipeline.

string security_key = builder.Configuration.GetSection("TokenAuthentication")["SecretKey"];
var symetricSecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(security_key));

string connection = builder.Configuration.GetConnectionString("localMySqlConnection")!;
builder.Services.AddDbContext<AppDbContext>(options => options.UseMySql(connection, ServerVersion.AutoDetect(connection)));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.TokenValidationParameters.ValidateIssuerSigningKey = true;
    options.TokenValidationParameters.IssuerSigningKey = symetricSecurityKey;
    options.TokenValidationParameters.ValidAudience = builder.Configuration.GetSection("TokenAuthentication")["Audience"];
    options.TokenValidationParameters.ValidIssuer = builder.Configuration.GetSection("TokenAuthentication")["Issuer"];
    options.TokenValidationParameters.ClockSkew = TimeSpan.Zero;
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRazorPages();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
}
);
builder.Services.AddWebSocketManager();
builder.Services.AddSingleton<RoomHandler>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder => builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
    );
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IMovementService, MovementService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IPushNotificationService, PushNotificationService>();

///


builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = String.Empty;
    options.Password.RequiredLength = 6;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CaseLuan"));
var webSocketOptions = new WebSocketOptions() { KeepAliveInterval = TimeSpan.FromSeconds(120), };
app.UseCors("CorsPolicy");

app.UseExceptionHandler("/Error");
// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
app.UseHsts();


app.UseWebSockets(webSocketOptions);
app.MapWebSocketManager("/ws", app.Services.GetService<RoomHandler>()!);

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dataContext.Database.Migrate();

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

    var user = dataContext.Users.FirstOrDefault(x => x.Email == "mrempto@keener.io");

    if (user is null)
    {
        await userManager.CreateAsync(new User
        {
            Name = "teste",
            UserName = "mrempto@keener.io",
            Email = "mrempto@keener.io",
        }, "teste123");
    }
}

app.Run();
