using IpczApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Registrace služeb pro MVC
builder.Services.AddControllersWithViews();

// Registrace vlastních služeb
builder.Services.AddSingleton<TriageService>();
builder.Services.AddSingleton<DatabaseService>();
builder.Services.AddSingleton<PdfService>();
builder.Services.AddTransient<EmailService>();
builder.Services.AddHttpClient<GeminiService>();

var app = builder.Build();

// Konfigurace HTTP req pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Triage}/{action=Index}/{id?}");

app.Run();
