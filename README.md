# Rfmk.Resolink.Grpc

[![NuGet version](https://img.shields.io/nuget/v/Rfmk.Resolink.Grpc.svg)](https://www.nuget.org/packages/Rfmk.Resolink.Grpc)

A [gRPC](https://grpc.io/) proxy for [ResoniteLink](https://github.com/Yellow-Dog-Man/ResoniteLink), exposing ResoniteLink with a strongly typed [GRPC](https://grpc.io/) and [protobuf](https://protobuf.dev/) API.

## Getting Started

### Perquisites:

[.NET-10](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) - Either SDK or ASP.NET, SDK is recommended.

### Installing

```bash
dotnet tool install --global Rfmk.Resolink.Grpc.Bridge
```

Updating if it's already installed
```bash
dotnet tool update --global Rfmk.Resolink.Grpc.Bridge
```

If updating from the pre-release, you'll need to uninstall Rfmk.Resolink.Grpc first; it's now a library.
```bash
dotnet tool uninstall --global Rfmk.Resolink.Grpc
```

### Starting the bridge

Start a Resonite session and enable ResoniteLink. Place the port in the URL below.

```bash
resolink-grpc --bridge:port={Resonite Link Port}
```

| Argument           | Description                                                                 |
|--------------------|-----------------------------------------------------------------------------|
| `--bridge:port`    | The ResoniteLink port opened in Resonite. *Ignored if hostUrl is provided.* |
| `--bridge:hostUrl` | A URL to the ResoniteLink port.                                             |
| `--port`           | The port for the GRPC endpoint to listen to.                                |

## Generating a client

GRPC and protobuf schemas are in the `Protos` folder, use the `protoc` [compiler](https://protobuf.dev/installation/) to generate a client.

The APIs are fairly normal rest-ish APIs and follow the same structure as [ResoniteLink](https://github.com/Yellow-Dog-Man/ResoniteLink).

A good starting query:
```textproto
GRPC {grpc endpoint}/rfmk.resolink.LinkService/GetSlot

{
  "slot_id": "Root",
  "depth": 2,
  "include_component_data": false
}
```

