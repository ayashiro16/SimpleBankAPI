using AccountContext = SimpleBankAPI.AccountContext;
using Microsoft.EntityFrameworkCore;
using SimpleBankAPI.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
services.AddSingleton<ISavableCollection, AccountContext>();

builder.Services.AddControllers();
builder.Services.AddDbContext<AccountContext>(opt =>
    opt.UseInMemoryDatabase("Accounts"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();