using Library.Api.Configurations;
using Library.Application;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApiConfig(builder.Configuration, builder.Environment);
builder.Services.ConfigApplication(builder.Configuration);
builder.Services.AddAuthConfig(builder.Configuration);
builder.Services.AddSwaggerConfig();

var app = builder.Build();
app.UseApiConfig();
app.UseAuthConfig();
app.UseSwaggerConfig();
app.MapControllers();
app.Run();