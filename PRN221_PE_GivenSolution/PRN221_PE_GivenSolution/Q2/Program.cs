
using Q2.Hubs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddSession();
builder.Services.AddMemoryCache();
builder.Services.AddSignalR();

var app = builder.Build();
app.UseStaticFiles();
app.UseSession();
app.MapRazorPages();
app.UseHttpsRedirection();
app.UseRouting();
app.MapGet("/", () => "Hello World!");
app.MapHub<ProductsHub>("/productHubs");
app.Run();
