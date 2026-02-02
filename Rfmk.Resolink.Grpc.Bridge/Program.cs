using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Rfmk.Resolink.Grpc;
using Rfmk.Resolink.Grpc.Bridge;
using Rfmk.Resolink.Grpc.Bridge.Connection;
using Rfmk.Resolink.Grpc.Projectors;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc().AddJsonTranscoding();

builder.Services.AddGrpcSwagger();
builder.Services.AddSwaggerGen(c =>
{
    c.MapType<Google.Protobuf.WellKnownTypes.Empty>(() => new OpenApiSchema { Type = "object", });
    c.SwaggerDoc("v1", new() { Title = "Resonite Link GRPC Bridge", Version = "v1" });
});

builder.Services.AddOptions<BridgeOptions>()
    .Bind(builder.Configuration.GetSection("Bridge"))
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddOptions<LinkServiceOptions>()
    .Bind(builder.Configuration.GetSection("ResoniteLink"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddSingleton<IValidateOptions<BridgeOptions>, BridgeOptionsValidator>();
builder.Services.AddTransient<ILinkConnection, WsLinkConnection>();
builder.Services.AddTransient<IBatchProjector, BatchProjector>();
builder.Services.AddTransient<WsLinkConnection>();
builder.Services.AddSingleton<WsAdapter>();
builder.Services.AddHostedService<StartupConnection>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Resonite Link GRPC Bridge"));

app.UseGrpcWeb();
app.MapGrpcService<ResolinkService>().EnableGrpcWeb();

app.Run();
