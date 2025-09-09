using LinkDev.Talabat.APIs.Controllers.Errors;
using LinkDev.Talabat.APIs.Extensions;
using LinkDev.Talabat.APIs.Services;
using LinkDev.Talabat.Core.Application;
using LinkDev.Talabat.Core.Application.Abstraction;
using LinkDev.Talabat.Infrastructure.Persistence;
using LinkDev.Talabat.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace LinkDev.Talabat.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
                
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers().AddApplicationPart(typeof(Controllers.AssemblyInformation).Assembly);

            // Add services to the container.

            builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = false;
                options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var errors = actionContext.ModelState.Where(P => P.Value!.Errors.Count > 0)
                                   .SelectMany(P => P.Value!.Errors)
                                   .Select(E => E.ErrorMessage);

                    return new BadRequestObjectResult(new ApiValidationErrorResponse()
                    {
                        Errors = errors
                    });
                };
            })
                .AddApplicationPart(typeof(Controllers.AssemblyInformation).Assembly);
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy", policyBuilder =>
                {
                    policyBuilder
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                       //.WithOrigins(builder.Configuration["Urls:FrontendUrl"]!) // «” Œœ«„ —«»ÿ frontend
                       .WithOrigins("http://localhost:4200", "https://localhost:4200")
                        .AllowCredentials(); // ··”„«Õ »«·ﬂÊﬂÌ“ Ê«· Êﬂ‰« 
                });
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddPersistenceServices(builder.Configuration);
            builder.Services.AddApplicationServices();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddIdentityServices(builder.Configuration);
            builder.Services.AddHttpContextAccessor().AddScoped(typeof(ILoggedInUserService), typeof(LoggedInUserService));
            var app = builder.Build();
            #region Database Initializer
            // Initialize the database and seed data
               await app.InitializeStoreContextAsync();
            #endregion
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();
            app.UseStatusCodePagesWithReExecute("/Errors/{0}");
            app.UseStaticFiles();
            app.UseCors("MyPolicy");
            
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
