using SignalR.Test.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR().AddMessagePackProtocol();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// use this only for test
app.UseCors(
    (opt) =>
    {
        opt
            .AllowAnyHeader()
            .WithMethods("GET", "POST")
            .AllowCredentials()
            .SetIsOriginAllowed(origin => new Uri(origin).IsLoopback);
            // .WithExposedHeaders("Accept-Encoding");
    }
);

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHub<TestHub>("/testHub");

app.Run();
