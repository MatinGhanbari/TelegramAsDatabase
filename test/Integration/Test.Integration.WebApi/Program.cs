using TelegramAsDatabase.ConfigurationServices;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddTDB(sc =>
{
    sc.Config.ApiKey = "7770450595:AAFtIQGYGHPD-CnqSoWkgQAJSYP8nnVYSSs";
    sc.Config.ChannelId = "-1002305327746";

    sc.Config.RetryPolicies.RetryCount = 2;
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
