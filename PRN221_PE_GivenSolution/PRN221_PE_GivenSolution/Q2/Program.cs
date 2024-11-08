using Q2.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Đăng ký các dịch vụ
builder.Services.AddRazorPages();
builder.Services.AddSession();
builder.Services.AddMemoryCache();
builder.Services.AddSignalR();
builder.Services.AddRouting();

// Nếu bạn có DbContext để làm việc với cơ sở dữ liệu, thêm vào đây
// builder.Services.AddDbContext<MyDbContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Sử dụng Static Files trước các middleware khác
app.UseStaticFiles();

// Sử dụng Session, nên đặt trước khi sử dụng Razor Pages
app.UseSession();

// Áp dụng HTTPS redirection
app.UseHttpsRedirection();

// Thiết lập Routing và SignalR hub trước khi chạy ứng dụng
app.UseRouting();
app.MapRazorPages();
app.MapHub<ProductsHub>("/productHubs");

// Nếu có xác thực, thêm app.UseAuthentication(); ở đây
// Nếu có phân quyền, thêm app.UseAuthorization(); ở đây

// Endpoint cơ bản, nếu cần có thể bỏ qua hoặc thay thế
app.MapGet("/", () => "Hello World!");

app.Run();
