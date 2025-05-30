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

            //setup database
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<AutoShopDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);

            builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<ICarRepository, CarRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            builder.Services.AddScoped<ICarService, CarService>();
            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Configure CORS
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    var allowedOrigins = builder.Configuration.GetSection("CORS:AllowedOrigins").Get<string[]>();
                    var allowedMethods = builder.Configuration.GetSection("CORS:AllowedMethods").Get<string[]>();
                    var allowedHeaders = builder.Configuration.GetSection("CORS:AllowedHeaders").Get<string[]>();
                    var allowCredentials = builder.Configuration.GetValue<bool>("CORS:AllowCredentials", true);

                    if (allowedOrigins != null && allowedOrigins.Length > 0)
                    {
                        policy.WithOrigins(allowedOrigins);
                    }
                    else if (builder.Environment.IsDevelopment())
                    {
                        policy.WithOrigins(
                            "http://localhost:3000",
                            "https://localhost:3000",
                            "http://localhost:5005",
                            "https://localhost:5005"
                        );
                    }
                    else
                    {
                        policy.WithOrigins("https://shop.nozsa.com");
                    }

                    if (allowedMethods != null && allowedMethods.Length > 0)
                    {
                        policy.WithMethods(allowedMethods);
                    }
                    else
                    {
                        policy.AllowAnyMethod();
                    }

                    if (allowedHeaders != null && allowedHeaders.Length > 0)
                    {
                        policy.WithHeaders(allowedHeaders);
                    }
                    else
                    {
                        policy.AllowAnyHeader();
                    }

                    if (allowCredentials)
                    {
                        policy.AllowCredentials();
                    }
                });
            });

            var app = builder.Build();

            // Always use CORS in production
            app.UseCors();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            {
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
