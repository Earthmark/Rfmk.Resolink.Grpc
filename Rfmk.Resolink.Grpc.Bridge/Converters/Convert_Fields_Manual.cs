namespace Rfmk.Resolink.Grpc.Bridge.Converters;

public static partial class Convert
{
    private static Empty ToProto(this ResoniteLink.EmptyElement self) => new()
    {
        Id = self.ID,
    };

    private static ResoniteLink.EmptyElement ToModel(this Empty self) => new()
    {
        ID = self.Id,
    };

    private static Reference ToProto(this ResoniteLink.Reference self) => new()
    {
        Id = self.ID,
        TargetId = self.TargetID,
        TargetType = self.TargetType,
    };

    private static ResoniteLink.Reference ToModel(this Reference self) => new()
    {
        ID = self.Id,
        TargetID = self.TargetId,
        TargetType = self.TargetType,

    };

    private static SyncList ToProto(this ResoniteLink.SyncList self) => new()
    {
        Id = self.ID,
        Elements = { self.Elements.Select(ToProto) }
    };

    private static ResoniteLink.SyncList ToModel(this SyncList self) => new()
    {
        ID = self.Id,
        Elements = self.Elements.Select(v => v.ToModel()).ToList()
    };

    private static SyncObject ToProto(this ResoniteLink.SyncObject self) => new()
    {
        Id = self.ID,
        Members = { self.Members.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToProto()) }
    };

    private static ResoniteLink.SyncObject ToModel(this SyncObject self) => new()
    {
        ID = self.Id,
        Members = self.Members.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToModel()),
    };

    private static FieldEnum ToProto(this ResoniteLink.Field_Enum self) => new()
    {
        Id = self.ID,
        Value = self.Value,
        EnumType = self.EnumType,
    };

    private static ResoniteLink.Field_Enum ToModel(this FieldEnum self) => new()
    {
        ID = self.Id,
        Value = self.Value,
        EnumType = self.EnumType,
    };

    private static FieldNullableEnum ToProto(this ResoniteLink.Field_Nullable_Enum self) => new()
    {
        Id = self.ID,
        Value = self.Value,
        EnumType = self.EnumType,
    };

    private static ResoniteLink.Field_Nullable_Enum ToModel(this FieldNullableEnum self) => new()
    {
        ID = self.Id,
        Value = self.Value,
        EnumType = self.EnumType,
    };
}
