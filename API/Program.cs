using Application.Activities;

var builder = WebApplication.CreateBuilder(args);

//add service to container

builder.Services.AddControllers(opt =>
{
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    opt.Filters.Add(new AuthorizeFilter(policy));
})
    .AddFluentValidation(config =>
{
    config.RegisterValidatorsFromAssemblyContaining<Create>();
});
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

// Configure http request pipeline

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.UseXContentTypeOptions();
app.UseReferrerPolicy(opt => opt.NoReferrer());

app.UseXXssProtection(opt => opt.EnabledWithBlockMode());
app.UseXfo(opt => opt.Deny());
app.UseCsp(opt => opt
    .BlockAllMixedContent()
    .StyleSources(s => s.Self().CustomSources(
        "https://fonts.googleapis.com",
        "sha256-yChqzBduCCi4o4xdbXRXh4U/t1rP4UUUMJt+rB+ylUI=",
        "sha256-rg9vBVGb4HCmLX9JXBEBSDVopOqpHsM1jQE1yCa1b64="       
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
        "blob:"
        ))
    .ScriptSources(s => s.Self()
        .CustomSources(
            /*"sha256-wVRXWUEfZlb+w20/SgFvOUbjKFu6/dc5mzuPLloyk4c=",*/
            "sha256-wVRXWUEfZlb+w20/SgFvOUbjKFu6/dc5mzuPLloyk4c=",           
            "https://connect.facebook.net",
            "sha256-JNYmorSMh6CRwKRsAP+707Grq/2GGFA7qLGk4vEmhdE=",            
            "sha256-+XNgLi9xFQyMCQemsP2he+ScPFFAOzyqA3x4Z4I2WI4="
        //,
        //"sha256-3x3EykMfFJtFd84iFKuZG0MoGAo5XdRfl3rq3r//ydA="

        ))

);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
}
else
{
    app.Use(async (context, next) =>
    {
        context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000");
        //to call the next piece of middleware after we add our Header
        await next.Invoke();
    });
}

//app.UseHttpsRedirection();       

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();
app.MapHub<ChatHub>("/chat");
app.MapFallbackToController("Index", "Fallback");


// Necessary to Disposed Application
// host any services that we create inside this particular method
// Then be Disposed
using var scope = app.Services.CreateScope();

var services = scope.ServiceProvider;

try
{
    var context = services.GetRequiredService<DataContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    await context.Database.MigrateAsync();
    await Seed.SeedData(context, userManager);
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occured during migration");
}

await app.RunAsync();


