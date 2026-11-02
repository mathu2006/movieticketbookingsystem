using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Repositories;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services;
using MovieTicketBooking.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Database connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Movie dependency injection
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IMovieService, MovieService>();

// Theatre dependency injection
builder.Services.AddScoped<ITheatreRepository, TheatreRepository>();
builder.Services.AddScoped<ITheatreService, TheatreService>();

// Controllers and OpenAPI
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddScoped<IScreenRepository, ScreenRepository>();
builder.Services.AddScoped<IScreenService, ScreenService>();
builder.Services.AddScoped<ISeatRepository, SeatRepository>();
builder.Services.AddScoped<ISeatService, SeatService>();
builder.Services.AddScoped<IShowRepository, ShowRepository>();
builder.Services.AddScoped<IShowService, ShowService>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IBookingService, BookingService>();
var app = builder.Build();

// OpenAPI configuration
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();