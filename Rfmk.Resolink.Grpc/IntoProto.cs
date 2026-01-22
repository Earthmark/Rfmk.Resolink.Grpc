using Rfmk.Resonite;

namespace Rfmk.Resolink.Grpc;

public static class IntoProto
{
    public static Slot ToProto(this ResoniteLink.Slot self)
    {
        return new Slot
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
    }

    private static Component ToProto(this ResoniteLink.Component self)
    {
        return new Component
        {
            Id = self.ID,
            IsReferenceOnly = self.IsReferenceOnly,
            ComponentType = self.ComponentType,
            Members = { self.Members.Select(kvp => (kvp.Key, kvp.Value.ToProto())).ToDictionary() }
        };
    }

    private static Member ToProto(this ResoniteLink.Member self)
    {
        return MemberMapper.Convert(self);
    }

    private static Reference ToProto(this ResoniteLink.Reference self)
    {
        return new Reference
        {
            Id = self.ID,
            TargetId = self.TargetID ?? "",
            TargetType = self.TargetType,
        };
    }

    private static FieldString ToProto(this ResoniteLink.Field_string self)
    {
        var str = new FieldString
        {
            Id = self.ID,
        };
        if (self.Value != null)
        {
            str.Value = self.Value;
        }

        return str;
    }

    private static FieldFloat3 ToProto(this ResoniteLink.Field_float3 self)
    {
        return new FieldFloat3
        {
            Id = self.ID,
            Value = self.Value.ToProto(),
        };
    }

    private static Float3 ToProto(this ResoniteLink.float3 self)
    {
        return new Float3
        {
            X = self.x,
            Y = self.y,
            Z = self.z,
        };
    }

    private static FieldFloatQ ToProto(this ResoniteLink.Field_floatQ self)
    {
        return new FieldFloatQ
        {
            Id = self.ID,
            Value = self.Value.ToProto(),
        };
    }

    private static FloatQ ToProto(this ResoniteLink.floatQ self)
    {
        return new FloatQ
        {
            X = self.x,
            Y = self.y,
            Z = self.z,
            W = self.w,
        };
    }

    private static FieldBool ToProto(this ResoniteLink.Field_bool self)
    {
        return new FieldBool
        {
            Id = self.ID,
            Value = self.Value,
        };
    }

    private static FieldInt ToProto(this ResoniteLink.Field_int self)
    {
        return new FieldInt
        {
            Id = self.ID,
            Value = self.Value,
        };
    }

    private static FieldLong ToProto(this ResoniteLink.Field_long self)
    {
        return new FieldLong
        {
            Id = self.ID,
            Value = self.Value,
        };
    }
}
