using Microsoft.Extensions.Options;
using Rfmk.Resolink.Grpc;
using Rfmk.Resolink.Grpc.Link;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddOptions<BridgeOptions>()
    .Bind(builder.Configuration.GetSection("Bridge"))
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddSingleton<IValidateOptions<BridgeOptions>, BridgeOptionsValidator>();
builder.Services.AddSingleton<Connection>();
builder.Services.AddHostedService<StartupConnection>(); 

var app = builder.Build();

app.MapGrpcService<ResolinkService>();

app.Run();
