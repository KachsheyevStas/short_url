using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using short_url.Helpers;
using short_url.Services;
using short_url.Authorization;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
{
    var services = builder.Services;
    services.AddDbContext<DataContext>();

    services.AddCors();
    services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
    services.AddScoped<IUserService, UserService>();
    services.AddScoped<IJwtUtils, JwtUtils>();
    services.AddScoped<IUrlShortenerService, UrlShortenerService>();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    services.AddControllers();
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();
    services.AddAuthentication();
    services.AddAuthorization();

}
var app = builder.Build();

/*using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
    dataContext.Database.Migrate();
}*/

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

{
    app.UseCors(x => x
       .AllowAnyOrigin()
       .AllowAnyMethod()
       .AllowAnyHeader());
    app.UseAuthentication();
    app.UseAuthorization();

    // global error handler
    app.UseMiddleware<ErrorHandlerMiddleware>();
    app.UseMiddleware<short_url.Authorization.JwtMiddleware>();
    app.UseHttpsRedirection();
    app.UseRouting();
    app.MapControllers();
    app.MapGet("/{short_code}", async (
    [FromRoute(Name = "short_code")] string ShortCode,
    IUrlShortenerService shortenService,
    CancellationToken cancellationToken
    ) =>
    {
        var destinationUrl = await shortenService.GetDestinationUrlAsync(ShortCode, cancellationToken);

        return Results.Redirect(destinationUrl);
    });
}

app.Run("https://localhost:7090");
