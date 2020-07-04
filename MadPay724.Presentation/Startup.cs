using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using AutoMapper;
using MadPay724.Common.Helpers;
using MadPay724.Common.Helpers.Helpers;
using MadPay724.Common.Helpers.Interface;
using MadPay724.Common.Helpers.MediaTypes;
using MadPay724.Data.DatabaseContext;
using MadPay724.Presentation.Helpers.Filters;
using MadPay724.Repository.Infrastructure;
using MadPay724.Service.Site.Admin.Auth.Service;
using MadPay724.Services.Seed.Interface;
using MadPay724.Services.Seed.Service;
using MadPay724.Services.Site.Admin.Auth.Interface;
using MadPay724.Services.Site.Users.Interface;
using MadPay724.Services.Site.Users.Service;
using MadPay724.Services.Upload.Interface;
using MadPay724.Services.Upload.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.ResponseCompression;
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
        private readonly int? _httpsPort;
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;

            if (env.IsDevelopment())
            {
                var lunchJsonConig = new ConfigurationBuilder()
                   .SetBasePath(env.ContentRootPath)
                   .AddJsonFile("Properties\\launchSettings.json")
                   .Build();

                _httpsPort = lunchJsonConig.GetValue<int>("iisSettings:iisExpress:sslPort");
            }
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddControllers().AddNewtonsoftJson(opt =>
            //{
            //    opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            //});
            //services.AddControllers();

            services.AddMvc(config =>
            {
                config.EnableEndpointRouting = false;
                //conig.ReturnHttpNotAcceptable = true; // agar accept k dar heder bud eshtebah bud error bde
                //conig.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter()); // baraye poshtibai az xml dar proje - ersale xms to user
                //conig.InputFormatters.Add(new XmlSerializerInputFormatter(conig));  // baraye in k betvanad darkhaste ra ba content-type xml befrstad

                #region baraye in ke content type header request ra be delkhah avaz konim
                //var jsonFormatter = conig.OutputFormatters.OfType<SystemTextJsonOutputFormatter>().Single();
                //conig.OutputFormatters.Remove(jsonFormatter);
                //conig.OutputFormatters.Add(new IonOutputFormatter(jsonFormatter));
                #endregion

                #region config https for project
                config.SslPort = _httpsPort;
                config.Filters.Add(typeof(RequireHttpsAttribute));
                #endregion

                config.Filters.Add(typeof(LinkRewritingFilter)); // afzoodan filter baraye sakhte link dar reponse (ION)

                #region tarife Cache , arguman hayash - estefade : balaye action => [ResponseCache(CacheProfileName = "static")]
                //config.CacheProfiles.Add("static", new CacheProfile
                //{
                //    Duration = 80000
                //}); 
                #endregion

            });

            #region baraye in ke http aslan kar nakonad va browser ra majbor konim site ra ba https baz konad ba khode mvc
            // heder zir ra be header hye site ezafr mikonad : Strict-Transport-Security
            // bad az in kar site ba http nabayad baz shavad
            services.AddHsts(opt =>
                {
                    opt.MaxAge = TimeSpan.FromDays(180);
                    opt.IncludeSubDomains = true;
                    opt.Preload = true;
                    //opt.ExcludedHosts //  yek seri address ha ra tabdi be https nakon
                });
            #endregion

            //services.AddResponseCompression(opt=>opt.Providers.Add<GzipCompressionProvider>());
            services.AddResponseCaching();  // vaghti be in sorat ([ResponseCache(Duration = 60)]) estefade mikonim ham dar client va ham dar server Cach mishavad

            #region all inputs lower case
            //services.AddRouting(opt=>
            //{
            //    opt.LowercaseUrls = true;   // baraye lower case kardane horoof input - default pascal
            //}); 
            #endregion

            #region Versioning
            //services.AddApiVersioning(opt =>
            //{
            //    opt.ApiVersionReader = new MediaTypeApiVersionReader();
            //    opt.AssumeDefaultVersionWhenUnspecified = true;
            //    opt.ReportApiVersions = true;
            //    opt.DefaultApiVersion = new ApiVersion(1, 0);
            //    opt.ApiVersionSelector = new CurrentImplementationApiVersionSelector(opt);
            //}); 
            #endregion


            services.AddCors();
            //services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));

            services.AddAutoMapper(typeof(Startup));
            services.AddTransient<ISeedService, SeedService>();
            services.AddScoped<IUnitOfWork<MalpayDbContext>, UnitOfWork<MalpayDbContext>>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUploadService, UploadService>();
            services.AddScoped<IUtilities, Utilities>();

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
                    document.DocumentName = "v1_Site_Admin";
                    document.ApiGroupNames = new[] { "v1_Site_Admin" };

                    document.PostProcess = d =>
                    {
                        d.Info.Title = "MadPay724 Api doces";
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

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //services.AddScoped<LogFilter>();
            services.AddScoped<UserCheckIdFilter>();
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

                #region baraye in ke http aslan kar nakonad va browser ra majbor konim site ra ba https baz konad ba package NWebsec.AspNetCore.Middleware
                // chn cashable hast behtar ast dar halate prodication estefade shavad
                //app.UseHsts(opt =>
                //    {
                //        opt.MaxAge(days: 180);
                //        opt.IncludeSubdomains();
                //        opt.Preload();
                //    }); 
                #endregion
                app.UseHsts();

            }
            app.UseHttpsRedirection();

            //app.UseResponseCompression();
            app.UseResponseCaching();

            seeder.SeedUsers();

            //app.UseCors(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseCors(p => p.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader().AllowCredentials());

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseOpenApi();
            app.UseSwaggerUi3();

            //app.UseStaticFiles(new StaticFileOptions()
            //{
            //    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot")),
            //    RequestPath = new PathString("/wwwroot")
            //});
            app.UseStaticFiles(new StaticFileOptions()
            {
                RequestPath = new PathString("/wwwroot")
            });

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //});
            app.UseMvc();
        }
    }
}
