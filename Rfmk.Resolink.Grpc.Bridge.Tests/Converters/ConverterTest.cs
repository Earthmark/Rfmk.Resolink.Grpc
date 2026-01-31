using Rfmk.Resolink.Grpc.Bridge.Converters;

namespace Rfmk.Resolink.Grpc.Tests.Converters;

public class ConverterTest
{
    [Fact]
    public void ConvertStructuredObject()
    {
        var source = new Slot
        {
            Name = new FieldString
            {
                Id = "Taco",
                Value = "Mysterious"
            },
            Tag = new FieldString
            {
                Id = "Tomato"
            },
            Children =
            {
                new Slot
                {
                    Id = "Child"
                }
            },
            Components =
            {
                new Component
                {
                    Id = "Something",
                    Members =
                    {
                        ["Mystery"] = new Member
                        {
                            Sbyte = new FieldSByte
                            {
                                Id = "Mystery Field",
                                Value = 3
                            }
                        }
                    }
                }
            }
        };

        var modelVersion = source.ToModel();
        var outputVersion = modelVersion.ToProto();

        Assert.Equivalent(source, outputVersion);
        Assert.Equal("Mystery Field", outputVersion.Components[0].Members["Mystery"].Sbyte.Id);
    }
}