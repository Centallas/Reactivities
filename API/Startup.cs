using API.Extensions;
using API.Middleware;
using API.SignalR;
using Application.Activities;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace API
{
    public class Startup
    {
        public IConfiguration _config { get; }
        public Startup(IConfiguration config)
        {
            _config = config;

        }



        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers(opt =>
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                opt.Filters.Add(new AuthorizeFilter(policy));
            })
                .AddFluentValidation(config =>
            {
                config.RegisterValidatorsFromAssemblyContaining<Create>();
            });
            services.AddApplicationServices(_config);
            services.AddIdentityServices(_config);


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseXContentTypeOptions();
            app.UseReferrerPolicy(opt => opt.NoReferrer());
       
            app.UseXXssProtection(opt => opt.EnabledWithBlockMode());
            app.UseXfo(opt => opt.Deny());
            app.UseCsp(opt => opt                
                .BlockAllMixedContent()
                .StyleSources(s => s.Self().CustomSources(
                    "https://fonts.googleapis.com",
                    "sha256-yChqzBduCCi4o4xdbXRXh4U/t1rP4UUUMJt+rB+ylUI="                   
                ))
                .FontSources(s => s.Self().CustomSources(
                    "https://fonts.gstatic.com", "data:"
                ))
                .FormActions(s => s.Self())
                .FrameAncestors(s => s.Self())
                .ImageSources(s => s.Self().CustomSources(
                    "https://res.cloudinary.com",
                    "https://www.facebook.com",                   
                    "https://platform-lookaside.fbsbx.com/",
                    "data:"
                    ))
                .ScriptSources(s => s.Self()
                    .CustomSources(
                      /*"sha256-wVRXWUEfZlb+w20/SgFvOUbjKFu6/dc5mzuPLloyk4c=",*/
                        "sha256-wVRXWUEfZlb+w20/SgFvOUbjKFu6/dc5mzuPLloyk4c=",
                        "https://connect.facebook.net",
                        "sha256-JNYmorSMh6CRwKRsAP+707Grq/2GGFA7qLGk4vEmhdE="
                        //,
                        //"sha256-3x3EykMfFJtFd84iFKuZG0MoGAo5XdRfl3rq3r//ydA="
                       
                    ))
                
            );
            
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
            }
            else 
            {
                app.Use(async (context, next)=> 
                {
                   context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000");  
                   //to call the next piece of middleware after we add our Header
                   await next.Invoke();
                });
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/chat");
                endpoints.MapFallbackToController("Index", "Fallback");
            });
        }
    }
}
