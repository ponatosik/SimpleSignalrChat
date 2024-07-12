using Microsoft.EntityFrameworkCore;
using SimpleSignalrChat.BusinessLogic.Events;
using SimpleSignalrChat.BusinessLogic.Events.Implementation;
using SimpleSignalrChat.BusinessLogic.Events.Interfaces;
using SimpleSignalrChat.BusinessLogic.Services;
using SimpleSignalrChat.BusinessLogic.Services.Interfaces;
using SimpleSignalrChat.DataAccess;
using SimpleSignalrChat.DataAccess.Repositories;
using SimpleSignalrChat.DataAccess.Repositories.Interfaces;
using SimpleSignalrChat.Presentaion.Rest.ExceptionHandling;
using SimpleSignalrChat.Presentaion.SignalR;
using SimpleSignalrChat.Presentaion.SignalR.EventHandlers;
using SimpleSignalrChat.Presentaion.SignalR.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSignalR();
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
builder.Services.AddScoped<IEventHandler<ChatDeletedEvent>, ChatDeletedEventHandler>();
builder.Services.AddScoped<IEventHandler<MessageCreatedEvent>, MessageCreatedEventHandler>();
builder.Services.AddScoped<IEventHandler<MessageDeletedEvent>, MessageDeletedEventHandler>();

builder.Services.AddSingleton<IHubGroupsManager<ChatHub>, HubGroupsManager<ChatHub>>();

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
app.MapHub<ChatHub>("/chat");

app.Run();
