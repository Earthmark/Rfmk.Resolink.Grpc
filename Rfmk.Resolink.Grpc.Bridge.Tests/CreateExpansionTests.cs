namespace Rfmk.Resolink.Grpc.Bridge.Tests;

public class CreateExpansionTests
{
    [Fact]
    public Task TestCreate()
    {
        var toCreate = new Slot
        {
            Name = new FieldString { Value = "Taco" },
            Children =
            {
                new Slot { Name = new FieldString { Value = "Burrito" } },
                new Slot { Name = new FieldString { Value = "Quesadilla" } },
                new Slot
                {
                    Name = new FieldString { Value = "Hot Dog" },
                    Components =
                    {
                        new Component
                        {
                            Members =
                            {
                                ["Mystery"] = new Member { Sbyte = new FieldSByte { Value = 3 } },
                                ["Secret Mystery"] = new Member { Long = new FieldLong { Value = 31222 } },
                                ["Third Mystery"] = new Member { Bool = new FieldBool { Value = true } }
                            }
                        }
                    }
                },
            },
            Components =
            {
                new Component { Id = "Something" },
                new Component
                {
                    Id = "Something Else",
                    Members = { ["Other Mystery"] = new Member { Float = new FieldFloat { Value = 3.14f } } }
                },
                new Component { Id = "Something Else Again" }
            }
        };

        var changes = new BatchRequest { Mutations = { CreateExpansion.ExpandCreateSlots(toCreate) } };

        return Verify(changes);
    }
}
