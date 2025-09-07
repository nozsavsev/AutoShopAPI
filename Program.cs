using AutoShopAPI.DbContexts;
using AutoShopAPI.Mappings;
using AutoShopAPI.Repositories;
using AutoShopAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace AutoShopAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddProblemDetails();

            //setup database
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<AutoShopDbContext>(options =>
                options.UseNpgsql(connectionString));

            builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);

            builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<ICarRepository, CarRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            builder.Services.AddScoped<ICarService, CarService>();
            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var myAllowSpecificOrigins = "_myAllowSpecificOrigins";

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: myAllowSpecificOrigins,
                                  policy =>
                                  {
                                       var corsOrigins = builder.Configuration.GetSection("corsConfig").Value;
                                       var env = builder.Environment.EnvironmentName;
                                       if (!string.IsNullOrEmpty(corsOrigins))
                                      {
                                          var origins = corsOrigins.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                                                  .Select(o => o.Trim())
                                                                  .ToArray();
                                          policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                                      }
                                      else
                                      {
                                           // No configured origins; in development, allow localhost defaults, otherwise deny by default
                                           if (builder.Environment.IsDevelopment())
                                           {
                                               policy.WithOrigins("http://localhost:3000", "http://localhost:5005").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                                           }
                                      }
                                  });
            });

            var app = builder.Build();

            app.UseExceptionHandler();

            // Always use CORS in production
            app.UseCors(myAllowSpecificOrigins);

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.MapControllers();

            if (app.Environment.IsDevelopment())
            {
                using var scope = app.Services.CreateScope();
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<AutoShopDbContext>();
                    context.Database.Migrate();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while migrating the database.");
                }
            }

            app.Run();
        }
    }
}
