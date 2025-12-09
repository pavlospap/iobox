using EasyNetQ;

using IOBox;
using IOBox.SqlServer;
using IOBox.SqlServer.Demo;
using IOBox.SqlServer.Demo.Persistence;
using IOBox.SqlServer.Demo.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddIOBox(builder.Configuration);

builder.Services.AddIOBoxSqlServer(builder.Configuration);

builder.Services.AddSingleton<IDbContext, DbContext>();

builder.Services.AddTransient<IDbMigrator, DbMigrator>();

builder.Services.AddSingleton<IMembersRepository, MembersRepository>();

builder.Services.AddSingleton<IMessageProcessor, MessageProcessor>();

builder.Services.AddHostedService<MessageListener>();

builder.Services.AddSingleton(RabbitHutch.CreateBus(
    builder.Configuration.GetValue<string>("RabbitMQ:ConnectionString")));

var app = builder.Build();

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseIOBox(builder.Configuration);

app.AddMembersEndpoints();

using var scope = app.Services.CreateScope();

var dbMigrator = scope.ServiceProvider.GetRequiredService<IDbMigrator>();

dbMigrator.MigrateDb();

app.Run();
