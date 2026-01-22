namespace Rfmk.Resolink.Grpc.Converters;

public static partial class Convert
{
    private static Empty ToProto(this ResoniteLink.EmptyElement self)
    {
        return new Empty
        {
            Id = self.ID,
        };
    }
    
    private static ResoniteLink.EmptyElement ToModel(this Empty self)
    {
        return new ResoniteLink.EmptyElement
        {
            ID = self.Id,
            
        };
    }
    
    private static Reference ToProto(this ResoniteLink.Reference self)
    {
        return new Reference
        {
            Id = self.ID,
            TargetId =  self.TargetID,
            TargetType =  self.TargetType,
        };
    }
    
    private static ResoniteLink.Reference ToModel(this Reference self)
    {
        return new ResoniteLink.Reference
        {
            ID = self.Id,
            TargetID =  self.TargetId,
            TargetType =  self.TargetType,
            
        };
    }
    
    private static SyncList ToProto(this ResoniteLink.SyncList self)
    {
        return new SyncList
        {
            Id = self.ID,
            Elements = { self.Elements.Select(ToProto) }
        };
    }
    
    private static ResoniteLink.SyncList ToModel(this SyncList self)
    {
        return new ResoniteLink.SyncList
        {
            ID = self.Id,
            Elements = self.Elements.Select(v => v.ToModel()).ToList()
        };
    }
    
    private static SyncObject ToProto(this ResoniteLink.SyncObject self)
    {
        return new SyncObject
        {
            Id = self.ID,
            Members = { self.Members.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToProto()) }
        };
    }
    
    private static ResoniteLink.SyncObject ToModel(this SyncObject self)
    {
        return new ResoniteLink.SyncObject
        {
            ID = self.Id,
            Members = self.Members.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToModel()),
        };
    }
    
    private static FieldEnum ToProto(this ResoniteLink.Field_Enum self)
    {
        return new FieldEnum
        {
            Id = self.ID,
            Value = self.Value,
            EnumType = self.EnumType,
        };
    }
    
    private static ResoniteLink.Field_Enum ToModel(this FieldEnum self)
    {
        return new ResoniteLink.Field_Enum
        {
            ID = self.Id,
            Value = self.Value,
            EnumType = self.EnumType,
        };
    }
    
    private static FieldNullableEnum ToProto(this ResoniteLink.Field_Nullable_Enum self)
    {
        return new FieldNullableEnum
        {
            Id = self.ID,
            Value = self.Value,
            EnumType = self.EnumType,
        };
    }
    
    private static ResoniteLink.Field_Nullable_Enum ToModel(this FieldNullableEnum self)
    {
        return new ResoniteLink.Field_Nullable_Enum
        {
            ID = self.Id,
            Value = self.Value,
            EnumType = self.EnumType,
        };
    }
}
