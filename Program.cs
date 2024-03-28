using Microsoft.EntityFrameworkCore;
using server.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using server.Hubs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

var jwtIssuer = builder.Configuration.GetSection("Jwt:Issuer").Get<string>();
var jwtKey = builder.Configuration.GetSection("Jwt:Key").Get<string>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
 .AddJwtBearer(options =>
 {
     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateIssuer = true,
         ValidateAudience = true,
         ValidateLifetime = true,
         ValidateIssuerSigningKey = true,
         ValidIssuer = jwtIssuer,
         ValidAudience = jwtIssuer,
         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
     };
 });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var connectionString = builder.Configuration.GetConnectionString("ServerDbConnectionString");
builder.Services.AddDbContext<ServerDbContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
builder.Services.AddSignalR();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();

app.UseHttpsRedirection();

app.UseCors(policy=>policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseAuthentication();


app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<NotificationHub>("connectedUsersCount");
    //endpoints.MapControllers();
});
app.MapControllers();

app.Run();




