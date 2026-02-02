namespace Rfmk.Resolink.Grpc.Bridge.Converters;

public static partial class Convert
{
    public static GetSessionResponse ToProto(this ResoniteLink.SessionData self) => new()
    {
        ResoniteVersion = self.ResoniteVersion,
        ResoniteLinkVersion = self.ResoniteLinkVersion,
        UniqueSessionId = self.UniqueSessionId,
    };

    public static ResoniteLink.GetSlot ToModel(this GetSlotRequest self) => new()
    {
        SlotID = self.SlotId,
        Depth = self.Depth,
        IncludeComponentData = self.IncludeComponentData
    };

    public static ResoniteLink.AddSlot ToModel(this AddSlotRequest self) => new()
    {
        Data = self.Data.ToModel(),
    };

    public static ResoniteLink.UpdateSlot ToModel(this UpdateSlotRequest self) => new()
    {
        Data = self.Data.ToModel(),
    };

    public static ResoniteLink.RemoveSlot ToModel(this DeleteSlotRequest self) => new()
    {
        SlotID = self.SlotId,
    };

    public static ResoniteLink.GetComponent ToModel(this GetComponentRequest self) => new()
    {
        ComponentID = self.ComponentId,
    };

    public static ResoniteLink.AddComponent ToModel(this AddComponentRequest self) => new()
    {
        ContainerSlotId = self.ContainerSlotId,
        Data = self.Data.ToModel(),
    };

    public static ResoniteLink.UpdateComponent ToModel(this UpdateComponentRequest self) => new()
    {
        Data = self.Data.ToModel(),
    };

    public static ResoniteLink.RemoveComponent ToModel(this DeleteComponentRequest self) => new()
    {
        ComponentID = self.ComponentId,
    };

    public static ResoniteLink.ImportTexture2DFile ToModelTexture(this ImportFileRequest self) => new()
    {
        FilePath = self.FilePath,
    };

    public static ResoniteLink.ImportAudioClipFile ToModelAudio(this ImportFileRequest self) => new()
    {
        FilePath = self.FilePath,
    };

    public static Slot ToProto(this ResoniteLink.Slot self) => new()
    {
        Id = self.ID,
        IsReferenceOnly = self.IsReferenceOnly,
        Name = self.Name?.ToProto(),
        Tag = self.Tag?.ToProto(),
        OrderOffset = self.OrderOffset?.ToProto(),
        Parent = self.Parent?.ToProto(),
        Position = self.Position?.ToProto(),
        Rotation = self.Rotation?.ToProto(),
        Scale = self.Scale?.ToProto(),
        IsActive = self.IsActive?.ToProto(),
        IsPersistent = self.IsPersistent?.ToProto(),
        Children = { self.Children?.Select(ToProto) ?? [] },
        Components = { self.Components?.Select(ToProto) ?? [] }
    };

    public static ResoniteLink.Slot ToModel(this Slot self) => new()
    {
        ID = self.Id,
        IsReferenceOnly = self.IsReferenceOnly,
        Name = self.Name?.ToModel(),
        Tag = self.Tag?.ToModel(),
        OrderOffset = self.OrderOffset?.ToModel(),
        Parent = self.Parent?.ToModel(),
        Position = self.Position?.ToModel(),
        Rotation = self.Rotation?.ToModel(),
        Scale = self.Scale?.ToModel(),
        IsActive = self.IsActive?.ToModel(),
        IsPersistent = self.IsPersistent?.ToModel(),
        Children = self.Children.Count > 0 ? self.Children?.Select(ToModel).ToList() : null,
        Components = self.Components.Count > 0 ? self.Components?.Select(ToModel).ToList() : null
    };

    public static Component ToProto(this ResoniteLink.Component self) => new()
    {
        Id = self.ID,
        IsReferenceOnly = self.IsReferenceOnly,
        ComponentType = self.ComponentType,
        Members = { self.Members?.Select(kvp => (kvp.Key, kvp.Value.ToProto())).ToDictionary() ?? [] }
    };

    public static ResoniteLink.Component ToModel(this Component self) => new()
    {
        ID = self.Id,
        IsReferenceOnly = self.IsReferenceOnly,
        ComponentType = self.ComponentType,
        Members = self.Members.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToModel()),
    };

    public static ResoniteLink.Message? ToModel(this BatchMutation self)
    {
        return self.MutationCase switch
        {
            BatchMutation.MutationOneofCase.None => null,
            BatchMutation.MutationOneofCase.AddSlot => self.AddSlot.ToModel(),
            BatchMutation.MutationOneofCase.UpdateSlot => self.UpdateSlot.ToModel(),
            BatchMutation.MutationOneofCase.DeleteSlot => self.DeleteSlot.ToModel(),
            BatchMutation.MutationOneofCase.AddComponent => self.AddComponent.ToModel(),
            BatchMutation.MutationOneofCase.UpdateComponent => self.UpdateComponent.ToModel(),
            BatchMutation.MutationOneofCase.DeleteComponent => self.DeleteComponent.ToModel(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public static ResoniteLink.Message? ToModel(this BatchQuery self)
    {
        return self.QueryCase switch
        {
            BatchQuery.QueryOneofCase.None => null,
            BatchQuery.QueryOneofCase.GetSlot => self.GetSlot.ToModel(),
            BatchQuery.QueryOneofCase.GetComponent => self.GetComponent.ToModel(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public static BatchQueryResponse ToProto(this ResoniteLink.Response self)
    {
        return self switch
        {
            ResoniteLink.ComponentData msg => new BatchQueryResponse
            {
                Component = msg.Data.ToProto(),
            },
            ResoniteLink.SlotData msg => new BatchQueryResponse
            {
                Slot = msg.Data.ToProto()
            },
            _ => throw new ArgumentOutOfRangeException(nameof(self))
        };
    }
}
