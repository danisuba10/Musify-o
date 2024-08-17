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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(
    builder.Configuration.GetConnectionString("WebApiDatabase"), new MySqlServerVersion(new Version(8, 0, 23))));

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddMediatR(typeof(AddAlbum.Handler).Assembly);
builder.Services.AddMediatR(typeof(AddSongsToAlbum.Handler).Assembly);
builder.Services.AddMediatR(typeof(GetAllAlbums.Handler).Assembly);
builder.Services.AddMediatR(typeof(GetAlbumByID.Handler).Assembly);
builder.Services.AddMediatR(typeof(RemoveAlbumByID.Handler).Assembly);
builder.Services.AddMediatR(typeof(UpdateAlbumByID.Handler).Assembly);

builder.Services.AddMediatR(typeof(AddArtist.Handler).Assembly);
builder.Services.AddMediatR(typeof(DeleteArtistByID.Handler).Assembly);
builder.Services.AddMediatR(typeof(UpdateArtistByID.Handler).Assembly);

builder.Services.AddMediatR(typeof(AddSong.Handler).Assembly);
builder.Services.AddMediatR(typeof(GetAllSongs.Handler).Assembly);
builder.Services.AddMediatR(typeof(GetSongByID.Handler).Assembly);
builder.Services.AddMediatR(typeof(RemoveSongByID.Handler).Assembly);
builder.Services.AddMediatR(typeof(SearchSongs.Handler).Assembly);
builder.Services.AddMediatR(typeof(UpdateSongByID.Handler).Assembly);

builder.Services.AddMediatR(typeof(GetAllUsers.Handler).Assembly);
builder.Services.AddMediatR(typeof(GetUserByUserName.Handler).Assembly);
builder.Services.AddMediatR(typeof(LoginUser.Handler).Assembly);
builder.Services.AddMediatR(typeof(RegisterUser.Handler).Assembly);

builder.Services.AddMediatR(typeof(GlobalSearch.Handler).Assembly);

builder.Services.AddAutoMapper(typeof(AlbumMappingProfile).Assembly);
builder.Services.AddAutoMapper(typeof(SongMappingProfile).Assembly);
builder.Services.AddAutoMapper(typeof(ArtistMappingProfile).Assembly);
builder.Services.AddAutoMapper(typeof(UserMappingProfile).Assembly);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
