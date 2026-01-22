using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq.Expressions;
using Type = System.Type;

namespace Rfmk.Resolink.Grpc;

internal class MemberMapper
{
    private static readonly ConcurrentDictionary<Type, Func<ResoniteLink.Member, Resonite.Member>> Mappers = [];

    private readonly ConcurrentDictionary<Type, Type> PbTarget = [];
    private readonly ConcurrentDictionary<Type, Type> LinkTarget = [];

    static MemberMapper()
    {
        MakeFieldConverter(typeof(ResoniteLink.Field_int));
        
        Google.Protobuf.WellKnownTypes.

        AddMapper<ResoniteLink.EmptyElement>((v, o) => o.Empty = new Resonite.Empty { Id = v.ID });
        AddMapper<ResoniteLink.Reference>((v, o) => o.Reference = new Resonite.Reference { Id = v.ID, TargetId = v.TargetID, TargetType = v.TargetType });
        AddMapper<ResoniteLink.SyncList>((v, o) => o.SyncList = new Resonite.SyncList
        {
            Id = v.ID,
            Elements = { v.Elements?.Select(Convert) }
        });
        AddMapper<ResoniteLink.SyncObject>((v, o) => o.SyncObject = new Resonite.SyncObject
        {
            Id = v.ID,
            Members = { v.Members?.Select(kvp => (kvp.Key, Convert(kvp.Value))).ToDictionary() }
        });

        /*
        AddMapper<ResoniteLink.Field_string>((v, o) => o.String = new Resonite.FieldString { Id = v.ID, Value = v.Value });
        AddMapper<ResoniteLink.Field_Uri>((v, o) => o.Uri = new Resonite.FieldUri { Value = v.Value.ToString() });
        AddMapper<ResoniteLink.Field_byte>((v, o) => o.Byte = new Resonite.FieldByte { Value = v.Value });
        AddMapper<ResoniteLink.Field_ushort>((v, o) => o.Ushort = new Resonite.FieldUShort { Value = v.Value });
        AddMapper<ResoniteLink.Field_uint>((v, o) => o.Uint = new Resonite.FieldUInt { Value = v.Value });
        AddMapper<ResoniteLink.Field_ulong>((v, o) => o.Ulong = new Resonite.FieldULong { Value = v.Value });
        AddMapper<ResoniteLink.Field_sbyte>((v, o) => o.Sbyte = new Resonite.FieldSByte { Value = v.Value });
        AddMapper<ResoniteLink.Field_short>((v, o) => o.Short = new Resonite.FieldShort { Value = v.Value });
        AddMapper<ResoniteLink.Field_int>((v, o) => o.Int = new Resonite.FieldInt { Value = v.Value });
        AddMapper<ResoniteLink.Field_int2>((v, o) => o.Int2 = new Resonite.FieldInt2 { Value = v.Value });
        AddMapper<ResoniteLink.Field_int3>((v, o) => o.Int3 = new Resonite.FieldInt3 { Value = v.Value });
        AddMapper<ResoniteLink.Field_int4>((v, o) => o.Int4 = new Resonite.FieldInt4 { Value = v.Value });
        AddMapper<ResoniteLink.Field_long>((v, o) => o.Long = new Resonite.FieldLong { Value = v.Value });
        AddMapper<ResoniteLink.Field_float>((v, o) => o.Float = new Resonite.FieldFloat { Value = v.Value });
        AddMapper<ResoniteLink.Field_double>((v, o) => o.Double = new Resonite.FieldDouble { Value = v.Value });
        AddMapper<ResoniteLink.Field_bool>((v, o) => o.Bool = new Resonite.FieldBool { Value = v.Value });
        AddMapper<ResoniteLink.Field_float2>((v, o) => o.Float2 = new Resonite.FieldFloat2 { Value = v.Value });
        AddMapper<ResoniteLink.Field_float3>((v, o) => o.Float3 = new Resonite.FieldFloat3 { Value = v.Value });
        AddMapper<ResoniteLink.Array_float>((v, o) => o.Float3Vec = new Resonite.FieldFloat3Vec { Values = { v.Values } });
        AddMapper<ResoniteLink.Field_float4>((v, o) => o.Float4 = new Resonite.FieldFloat4 { Value = v.Value });
        AddMapper<ResoniteLink.Field_floatQ>((v, o) => o.FloatQ = new Resonite.FieldFloatQ { Value = v.Value });
        AddMapper<ResoniteLink.Array_floatQ>((v, o) => o.FloatQVec = new Resonite.FieldFloatQVec { Values = { v.Values } });
        AddMapper<ResoniteLink.Field_color>((v, o) => o.Color = new Resonite.FieldColor { Value = v.Value });
        AddMapper<ResoniteLink.Field_colorX>((v, o) => o.ColorX = new Resonite.FieldColorX { Value = v.Value });
        AddMapper<ResoniteLink.Field_color32>((v, o) => o.Color32 = new Resonite.FieldColor32 { Value = v.Value });

        AddMapper<ResoniteLink.Field_Nullable_int>((v, o) => o.NullInt = new Resonite.FieldInt { Value = v.Value });
        AddMapper<ResoniteLink.Field_Nullable_int2>((v, o) => o.NullInt2 = new Resonite.FieldInt2 { Value = v.Value });
        AddMapper<ResoniteLink.Field_Nullable_float>((v, o) => o.NullFloat = new Resonite.FieldFloat { Value = v.Value });
        AddMapper<ResoniteLink.Field_Nullable_bool>((v, o) => o.NullBool = new Resonite.FieldBool { Value = v.Value });
        AddMapper<ResoniteLink.Field_Nullable_float3>((v, o) => o.NullFloat3 = new Resonite.FieldFloat3 { Value = v.Value });
        AddMapper<ResoniteLink.Field_Nullable_floatQ>((v, o) => o.NullFloatQ = new Resonite.FieldFloatQ { Value = v.Value });
        AddMapper<ResoniteLink.Field_Nullable_colorX>((v, o) => o.NullColorX = new Resonite.FieldColorX { Value = v.Value });
        */

        var typesToMap = typeof(ResoniteLink.Member).Assembly.GetExportedTypes().Where(t =>
            t.IsAssignableTo(typeof(ResoniteLink.Member)) && t != typeof(ResoniteLink.Member)).ToList();

        foreach (var type in typesToMap)
        {
            Mappers.GetOrAdd(type, MakeConverter);
        }
    }

    private static void AddMapper<T>(Action<T, Resonite.Member> f) where T : ResoniteLink.Member
    {
        Mappers[typeof(T)] = m =>
        {
            Resonite.Member output = new();
            f((T)m, output);
            return output;
        };
    }

    public static Resonite.Member Convert(ResoniteLink.Member src)
    {
        return Mappers.GetOrAdd(src.GetType(), MakeConverter)(src);
    }

    private static Type GetTargetPbType(Type linkType)
    {
        return typeof(Resonite.FieldInt);
    }

    private static Func<ResoniteLink.Member, Resonite.Member> MakeFieldConverter(Type type)
    {
        Debug.Assert(type.IsAssignableTo(typeof(ResoniteLink.Member)));
        var srcFieldType = type.GetProperty(nameof(ResoniteLink.Field_int.Value)).PropertyType;

        var targetType = GetTargetPbType(type);
        var targetFieldType = type.GetProperty(nameof(Resonite.FieldInt.Value)).PropertyType;

        var input = Expression.Parameter(typeof(ResoniteLink.Member), "linkType");
        var cast = Expression.Variable(type, "castType");

        var lambda =
            Expression.Lambda<Func<ResoniteLink.Member, Resonite.Member>>(
                Expression.Block(
                    Expression.Assign(cast, Expression.Convert(input, type)),
                    Expression.MemberInit(Expression.New(typeof(Resonite.Member)),
                        Expression.Bind(typeof(Resonite.Member).GetProperty(nameof(Resonite.Member.Int)),
            Expression.MemberInit(Expression.New(targetType),
            Expression.Bind(
                targetType.GetMember(nameof(Resonite.FieldInt.Id))[0],
                Expression.Property(cast, type.GetProperty(nameof(ResoniteLink.Member.ID)))
            ),
            Expression.Bind(
                targetType.GetMember(nameof(Resonite.FieldInt.Value))[0],
                Expression.Property(cast, type.GetProperty(nameof(ResoniteLink.Field_int.Value)))
            ))))),
            "translatorFor" + type.Name,
            [input]
            ).Compile();

        return lambda;
    }

    private static Func<ResoniteLink.Member, Resonite.Member> MakeSyncArrayConverter(Type type)
    {
        Debug.Assert(type.IsAssignableTo(typeof(ResoniteLink.SyncArray)));
    }

    private static Func<ResoniteLink.Member, Resonite.Member> MakeConverter(Type type)
    {
        type.GetProperty(nameof(ResoniteLink.Field_int.Value));

        return (m) => new Resonite.Member();
    }
}
