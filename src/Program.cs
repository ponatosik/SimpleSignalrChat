using Microsoft.EntityFrameworkCore;
using SimpleSignalrChat;
using SimpleSignalrChat.BusinessLogic.Events;
using SimpleSignalrChat.BusinessLogic.Events.Implementation;
using SimpleSignalrChat.BusinessLogic.Events.Interfaces;
using SimpleSignalrChat.BusinessLogic.Services;
using SimpleSignalrChat.BusinessLogic.Services.Interfaces;
using SimpleSignalrChat.DataAccess;
using SimpleSignalrChat.DataAccess.Repositories;
using SimpleSignalrChat.DataAccess.Repositories.Interfaces;
using SimpleSignalrChat.Presentaion.Rest.ExceptionHandling;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ChatContext>(options => options.UseInMemoryDatabase("ChatDb"));

builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IApiErrorMapper, ApiErrorMapper>();

builder.Services.AddScoped<IEventPublisher, EventPublisher>();


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
