using Microsoft.Extensions.Options;
using Rfmk.Resolink.Grpc;
using Rfmk.Resolink.Grpc.Bridge;
using Rfmk.Resolink.Grpc.Bridge.Connection;
using Rfmk.Resolink.Grpc.Projectors;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddOptions<BridgeOptions>()
    .Bind(builder.Configuration.GetSection("Bridge"))
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddSingleton<IValidateOptions<BridgeOptions>, BridgeOptionsValidator>();
builder.Services.AddTransient<ILinkConnection, WsLinkConnection>();
builder.Services.AddTransient<IBatchProjector, BatchProjector>();
builder.Services.AddTransient<WsLinkConnection>();
builder.Services.AddSingleton<WsAdapter>();
builder.Services.AddHostedService<StartupConnection>();

var app = builder.Build();

app.MapGrpcService<ResolinkService>();

app.Run();
