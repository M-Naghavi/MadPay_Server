using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using AutoMapper;
using MadPay724.Common.Helpers;
using MadPay724.Data.DatabaseContext;
using MadPay724.Repository.Infrastructure;
using MadPay724.Service.Site.Admin.Auth.Service;
using MadPay724.Services.Seed.Interface;
using MadPay724.Services.Seed.Service;
using MadPay724.Services.Site.Admin.Auth.Interface;
using MadPay724.Services.Site.Admin.Auth.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.Processors.Security;

namespace MadPay724.Presentation
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddControllers().AddNewtonsoftJson(opt =>
            //{
            //    opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            //});
            //services.AddControllers();
            services.AddMvc(opt => opt.EnableEndpointRouting = false);

            services.AddCors();
            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));

            services.AddAutoMapper(typeof(Startup));
            services.AddTransient<ISeedService, SeedService>();
            services.AddScoped<IUnitOfWork<MalpayDbContext>, UnitOfWork<MalpayDbContext>>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettingToken:Token").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            #region swagger
            services.AddOpenApiDocument(document =>
                {
                    document.DocumentName = "Site";
                    document.ApiGroupNames = new[] { "Site" };

                    document.PostProcess = d =>
                    {
                        d.Info.Title = "Hello World Site";
                        //d.Info.Contact = new OpenApiContact
                        //{
                        //    Name = "Ali Naghavi",
                        //    Email = string.Empty,
                        //    Url = "https://www.nuget.org/packages/NSwag.Generation.AspNetCore/"
                        //};
                        //d.Info.License = new OpenApiLicense
                        //{
                        //    Name = "Use under LICX",
                        //    Url = "https://www.nuget.org/packages/NSwag.Generation.AspNetCore/"
                        //};
                    };

                    document.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.ApiKey,
                        Name = "Authorization",
                        In = OpenApiSecurityApiKeyLocation.Header,
                        Description = "Type into the textbox: Bearer {your JWT token}."
                    });

                    document.OperationProcessors.Add(
                        new AspNetCoreOperationSecurityScopeProcessor("JWT"));
                    //      new OperationSecurityScopeProcessor("JWT"));
                });
            services.AddOpenApiDocument(document =>
            {
                document.DocumentName = "Api";
                document.ApiGroupNames = new[] { "Api" };
            });
            #endregion

            

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ISeedService seeder)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(builder =>
                {
                    builder.Run(async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            context.Response.AddAppError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message);
                        }
                    });
                });
                //app.UseHsts();
            }
            //app.UseHttpsRedirection();

            seeder.SeedUsers();

            //app.UseCors(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseCors(p => p.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader().AllowCredentials());

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(),@"Files")),
                RequestPath = new PathString("/Files")
            });


            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //});
            app.UseMvc();
        }
    }
}
