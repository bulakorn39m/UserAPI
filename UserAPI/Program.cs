using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.Json;
using UserAPI;
using UserAPI.Models;
using UserAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/* --- Load data to in-memory storage ---*/
builder.Services.AddSingleton((Func<IServiceProvider, ConcurrentDictionary<long, UserDo>>)(_ =>
{
    var inMemoryStore = new ConcurrentDictionary<long, UserDo>();
    using (var client = new HttpClient())
    {
        var response = client.GetAsync("https://jsonplaceholder.typicode.com/users").Result;
        response.EnsureSuccessStatusCode();

        var jsonString = response.Content.ReadAsStringAsync().Result;
        var users = JsonSerializer.Deserialize<List<UserDo>>(jsonString, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (users != null)
        {
            foreach (var user in users)
            {
                if (user.Id != null)
                {
                    inMemoryStore.TryAdd(user.Id.Value, new UserDo
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Username = user.Username,
                        Email = user.Email,
                        Address = user.Address,
                        Phone = user.Phone,
                        Website = user.Website,
                        Company = user.Company,
                    });
                }
            }
        }
    }
    return inMemoryStore;
}));

/* --- Services ---*/
StartupService.Register(builder);

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
