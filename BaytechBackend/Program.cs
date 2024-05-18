using BaytechBackend;
using BaytechBackend.Entities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<SharedDB>();
builder.Services.AddDbContext<BaytechDbContext>(options => options.UseNpgsql("User ID= ubuntu_db_admin;Password=admin;Host=3.73.248.105;Port=5432;Database=BaytechDB;"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();

        });
});
builder.Services.AddIdentity<User, Role>().AddEntityFrameworkStores<BaytechDbContext>();
builder.Services.AddSingleton<IDictionary<string, UserConnection>>(opts => new Dictionary<string, UserConnection>());
builder.Services.AddSignalR();
builder.Services.AddScoped<BaytechService>();

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowSpecificOrigins");

app.MapHub<ChatHub>("/chat");
app.MapHub<ChatHubb>("/chathub");

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapHub<ChatHub>("/chat");
//});


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

