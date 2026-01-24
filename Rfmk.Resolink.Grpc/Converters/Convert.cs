namespace Rfmk.Resolink.Grpc.Converters;

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
        Children = self.Children?.Select(ToModel).ToList(),
        Components = self.Components?.Select(ToModel).ToList()
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
}
