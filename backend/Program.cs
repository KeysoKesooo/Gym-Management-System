using GymManagement.Data;
using GymManagement.Services.JwtService;

//Middleware
using GymManagement.Core.Middleware.Session;
using GymManagement.Core.Middleware.RJMiddleware;


//User
using GymManagement.Core.Repositories.IntUserRepository;
using GymManagement.Core.Services.IntUserService;
//Auth
using GymManagement.Core.Services.IntAuthService;

//Attendance
using GymManagement.Core.Repositories.IntAttendanceRepository;
using GymManagement.Core.Services.IntAttendanceService;

//Walkin
using GymManagement.Core.Repositories.IntWalkinRepository;
using GymManagement.Core.Services.IntWalkinService;


//Redis Session
using GymManagement.Core.Services.RedisService;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// ----------------------
// 1️⃣ Redis Connection
// ----------------------
// Read Redis config
var redisHost = builder.Configuration["Redis:Host"] ?? "127.0.0.1";


// Configure Redis options
var redisOptions = new ConfigurationOptions
{
    EndPoints = { $"{redisHost}" },
    ConnectRetry = 5,
    ConnectTimeout = 10000,
    SyncTimeout = 5000,
    AbortOnConnectFail = false,
    AllowAdmin = true
};

// Connect to Redis
var redis = ConnectionMultiplexer.Connect(redisOptions);
builder.Services.AddSingleton<IConnectionMultiplexer>(redis);


// ----------------------
// 1️⃣ Database Context
// ----------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 39)),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null
        )
    ));


// ----------------------
// 2️⃣ CORS
// ----------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ----------------------
// 3️⃣ JWT Authentication
// ----------------------
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["Jwt:Key"]
                    ?? throw new InvalidOperationException("JWT Key is not configured")
                )
            )
        };
    });

// ----------------------
// 4️⃣ Role-based Authorization
// ----------------------
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("Staff", policy => policy.RequireRole("Staff"));
    options.AddPolicy("Member", policy => policy.RequireRole("Member"));
});

// ----------------------
// 5️⃣ Register Services
// ----------------------
builder.Services.AddScoped<JwtService>();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();
builder.Services.AddScoped<IWalkinRepository, WalkinRepository>();

// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IWalkinService, WalkinService>();
builder.Services.AddSingleton<RedisSessionService>();



// ----------------------
// 6️⃣ Controllers & Swagger
// ----------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ----------------------
// 7️⃣ Middleware
// ----------------------
app.UseCors("AllowFrontend");
app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<RedisSessionMiddleware>();
// Use Redis JWT middleware **before authentication**
app.UseMiddleware<RedisJwtMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
