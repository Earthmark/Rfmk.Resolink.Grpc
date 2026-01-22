using ResoniteLink;
using Rfmk.Resolink.Grpc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddOptions<BridgeOptions>()
    .Bind(builder.Configuration.GetSection("Bridge"))
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddSingleton<LinkInterface>();
builder.Services.AddHostedService<StartupConnection>();

var app = builder.Build();

app.MapGrpcService<ResolinkService>();

app.Run();
