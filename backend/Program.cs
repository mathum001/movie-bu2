using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
namespace Backend;



public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddDbContext<ApplicationContext>(options =>
        {
            options.UseNpgsql(
                "Host=localhost;Database=movies;Username=postgres;Password=password"
            );
        });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowReactApp", builder =>
            {
                builder.WithOrigins("http://localhost:3000") // Ange den URL där din React-app körs
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        builder.Services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy(
                "create_movie",
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                }
            );
            options.AddPolicy(
                "remove_movie",
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                }
            );
            options.AddPolicy(
                "update_movie",
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                }
            );
            options.AddPolicy(
                "get_movies",
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                }
            );
            options.AddPolicy(
                "detail_movie",
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                }
            );
        });

        builder
            .Services.AddIdentityCore<User>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationContext>()
            .AddApiEndpoints();

        builder.Services.AddScoped<MovieService>();
        builder.Services.AddTransient<IClaimsTransformation, UserClaimsTransformation>();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddScoped<IMovieRepository, MovieRepository>();

        var app = builder.Build();

        app.UseCors("AllowReactApp");

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapIdentityApi<User>();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseHttpsRedirection();
        app.MapControllers();

        app.Run();
    }
}

public class UserClaimsTransformation : IClaimsTransformation
{
    UserManager<User> userManager;

    public UserClaimsTransformation(UserManager<User> userManager)
    {
        this.userManager = userManager;
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        ClaimsIdentity claims = new ClaimsIdentity();

        var id = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (id != null)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                var userRoles = await userManager.GetRolesAsync(user);
                foreach (var userRole in userRoles)
                {
                    claims.AddClaim(new Claim(ClaimTypes.Role, userRole));
                }
            }
        }

        principal.AddIdentity(claims);
        return await Task.FromResult(principal);
    }
}