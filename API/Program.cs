using Persistence;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Application.Albums;
using Application.Songs;
using Application;
using Application.Artists;
using Application.Users;
using Application.Search;
using Application.Core;
using System.Text.Json.Serialization;
using Application.Images;
using Microsoft.EntityFrameworkCore.Design;
using DotNetEnv;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Application.Services;

Env.Load("../../.env");

var builder = WebApplication.CreateBuilder(args);

var jwtSettings = new
{
    Secret = Environment.GetEnvironmentVariable("Jwt_Secret"),
    Issuer = Environment.GetEnvironmentVariable("Jwt_Issuer"),
    Audience = Environment.GetEnvironmentVariable("Jwt_Audience")
};

Console.WriteLine("JWT Secret: " + jwtSettings.Secret);
Console.WriteLine("JWT Issuer: " + jwtSettings.Issuer);
Console.WriteLine("JWT Audience: " + jwtSettings.Audience);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalHost", builder =>
        builder.WithOrigins("http://localhost:3000", "http://127.0.0.1:3000", "http://192.168.0.184:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
    );
    options.AddPolicy("Prod", builder =>
        builder.WithOrigins("http://meloptica.stream", "https://meloptica.stream")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
    );
});

// Add services to the container.

//builder.Services.AddControllers();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Musify API", Version = "v1" });
        c.UseInlineDefinitionsForEnums();
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter 'Bearer' [space] and then your token in the text input below."
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });

var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__WebApiDatabase");
// var connectionString = Environment.GetEnvironmentVariable("CON");
// var connectionString = "server=mysql; database=" + Environment.GetEnvironmentVariable("MYSQL_DATABASE") + "; user=" + Environment.GetEnvironmentVariable("MYSQL_USER") + "; password=" + Environment.GetEnvironmentVariable("MYSQL_PASSWORD");
Console.WriteLine(connectionString);

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string not found in environment variables.");
}



//Maybe disable StringComparisonTranslations as per documentation indexes may not trigger
//every time
//https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/wiki/Configuration-Options
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0)), options =>
        options
            .EnableRetryOnFailure
            (
                maxRetryCount: 3,
                maxRetryDelay: System.TimeSpan.FromSeconds(5),
                errorNumbersToAdd: null
            )
            .EnableStringComparisonTranslations()
    ));

builder.Services.AddDbContextFactory<ApplicationDbContext>((IServiceProvider sp, DbContextOptionsBuilder options) =>
{
    var dbContextOptions = sp.GetRequiredService<DbContextOptions<ApplicationDbContext>>();
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0)), options =>
        options
            .EnableRetryOnFailure
            (
                maxRetryCount: 3,
                maxRetryDelay: System.TimeSpan.FromSeconds(5),
                errorNumbersToAdd: null
            )
            .EnableStringComparisonTranslations()
    );
}, ServiceLifetime.Scoped);


builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<JwtTokenService>();

builder.Services.AddMediatR(typeof(AddAlbum.Handler).Assembly);
builder.Services.AddMediatR(typeof(AddSongsToAlbum.Handler).Assembly);
builder.Services.AddMediatR(typeof(GetAllAlbums.Handler).Assembly);
builder.Services.AddMediatR(typeof(GetAlbumByID.Handler).Assembly);
builder.Services.AddMediatR(typeof(RemoveAlbumByID.Handler).Assembly);
builder.Services.AddMediatR(typeof(UpdateAlbumByID.Handler).Assembly);

builder.Services.AddMediatR(typeof(AddArtist.Handler).Assembly);
builder.Services.AddMediatR(typeof(RemoveArtistByID.Handler).Assembly);
builder.Services.AddMediatR(typeof(UpdateArtistByID.Handler).Assembly);

builder.Services.AddMediatR(typeof(AddSong.Handler).Assembly);
builder.Services.AddMediatR(typeof(GetAllSongs.Handler).Assembly);
builder.Services.AddMediatR(typeof(GetSongByID.Handler).Assembly);
builder.Services.AddMediatR(typeof(RemoveSongByID.Handler).Assembly);
builder.Services.AddMediatR(typeof(SearchSongs.Handler).Assembly);
builder.Services.AddMediatR(typeof(UpdateSongByID.Handler).Assembly);

builder.Services.AddMediatR(typeof(GetAllUsers.Handler).Assembly);
builder.Services.AddMediatR(typeof(GetUserByEmail.Handler).Assembly);
builder.Services.AddMediatR(typeof(LoginUser.Handler).Assembly);
builder.Services.AddMediatR(typeof(RegisterUser.Handler).Assembly);

builder.Services.AddMediatR(typeof(GlobalSearch.Handler).Assembly);

builder.Services.AddMediatR(typeof(UploadImage.Handler).Assembly);
builder.Services.AddMediatR(typeof(ResizeImage.Handler).Assembly);

builder.Services.AddAutoMapper(typeof(AlbumMappingProfile).Assembly);
builder.Services.AddAutoMapper(typeof(SongMappingProfile).Assembly);
builder.Services.AddAutoMapper(typeof(ArtistMappingProfile).Assembly);
builder.Services.AddAutoMapper(typeof(UserMappingProfile).Assembly);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "JwtBearer";
}).AddJwtBearer("JwtBearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }
}

if (app.Environment.IsProduction())
{
    Console.WriteLine("Production cors.");
    app.UseCors("Prod");
}
else
{
    Console.WriteLine("Dev cors.");
    app.UseCors("AllowLocalHost");
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker") || true)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
