using Microsoft.Extensions.Options;
using Rfmk.Resolink.Grpc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddOptions<BridgeOptions>()
    .Bind(builder.Configuration.GetSection("Bridge"))
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddSingleton<IValidateOptions<BridgeOptions>, BridgeOptionsValidator>();
builder.Services.AddScoped<BatchAdaptor>();
builder.Services.AddSingleton<WsAdapter>();
builder.Services.AddHostedService<StartupConnection>();

var app = builder.Build();

app.MapGrpcService<ResolinkService>();

app.Run();
