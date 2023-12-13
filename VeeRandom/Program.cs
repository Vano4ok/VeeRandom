using VeeRandomGenerator;
using VeeRandom.Server.Components;
using VeeRandom.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.Configure<GeneratorSettings>(
            builder.Configuration.GetSection(nameof(GeneratorSettings)));

builder.Services.AddTransient<IGenerator, Generator>();

builder.Services.AddTransient<RSACryptoService>();
builder.Services.AddSingleton<DigitalSignatureService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
