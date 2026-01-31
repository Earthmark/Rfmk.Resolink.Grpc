using Google.Protobuf.WellKnownTypes;

namespace Rfmk.Resolink.Grpc.Bridge.Converters;

public static partial class Convert
{
    // These help the codegen, cause some of these special cases are rough.

    private static int ToProto(this sbyte self) => self;
    private static int ToProto(this short self) => self;
    private static int ToProto(this int self) => self;
    private static long ToProto(this long self) => self;

    private static uint ToProto(this byte self) => self;
    private static uint ToProto(this ushort self) => self;
    private static uint ToProto(this uint self) => self;
    private static ulong ToProto(this ulong self) => self;

    private static float ToProto(this float self) => self;
    private static double ToProto(this double self) => self;

    private static bool ToProto(this bool self) => self;
    private static uint ToProto(this char self) => self;

    private static string ToProto(this string self) => self;

    private static string ToProto(this Uri self) => self.ToString();
    private static Uri ToModel(this string self) => new(self);

    private static Timestamp ToProto(this DateTime self) => Timestamp.FromDateTime(self);
    private static DateTime ToModel(this Timestamp self) => self.ToDateTime();

    private static Duration ToProto(this TimeSpan self) => Duration.FromTimeSpan(self);
    private static TimeSpan ToModel(this Duration self) => self.ToTimeSpan();

    private static Color ToProto(this ResoniteLink.color self) => new()
    {
        R = self.r,
        G = self.g,
        B = self.b,
        A = self.a,
    };

    private static ResoniteLink.color ToModel(this Color self) => new()
    {
        r = self.R,
        g = self.G,
        b = self.B,
        a = self.A,
    };

    private static ColorX ToProto(this ResoniteLink.colorX self) => new()
    {
        R = self.r,
        G = self.g,
        B = self.b,
        A = self.a,
        Profile = self.Profile,
    };

    private static ResoniteLink.colorX ToModel(this ColorX self) => new()
    {
        r = self.R,
        g = self.G,
        b = self.B,
        a = self.A,
        Profile = self.Profile,
    };

    private static Color32 ToProto(this ResoniteLink.color32 self) => new()
    {
        R = self.r,
        G = self.g,
        B = self.b,
        A = self.a,
    };

    private static ResoniteLink.color32 ToModel(this Color32 self) => new()
    {
        r = (byte)self.R,
        g = (byte)self.G,
        b = (byte)self.B,
        a = (byte)self.A,
    };

    private const decimal NanoFactor = 1_000_000_000;

    private static Decimal ToProto(this decimal self)
    {
        var units = decimal.ToInt64(self);
        var nanos = decimal.ToInt32((self - units) * NanoFactor);
        return new Decimal
        {
            Units = units,
            Nanos = nanos,
        };
    }

    private static decimal ToModel(this Decimal self) => self.Units + self.Nanos / NanoFactor;
}