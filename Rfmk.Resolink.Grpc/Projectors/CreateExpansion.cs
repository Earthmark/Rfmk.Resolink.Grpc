namespace Rfmk.Resolink.Grpc.Projectors;

public class CreateExpansion : IBatchProjectorLayer
{
    public void Project(BatchRequest request)
    {
        var src = request.Mutations.ToList();
        request.Mutations.Clear();
        foreach (var mutation in src)
        {
            request.Mutations.Add(mutation.MutationCase switch
            {
                BatchMutation.MutationOneofCase.AddSlot => ExpandAddSlot(mutation.DebugId, mutation.AddSlot),
                _ => [mutation]
            });
        }
    }

    private static IEnumerable<BatchMutation> ExpandAddSlot(string debugId, AddSlotRequest request)
    {
        var i = 0;
        foreach (var mutation in ExpandCreateSlots(request.Data))
        {
            mutation.DebugId = $"{debugId}/Create-tree expansion {i}";
            yield return mutation;
            i++;
        }
    }

    private static IEnumerable<BatchMutation> ExpandCreateSlots(Slot toCreate)
    {
        foreach (var createSlot in CreateSlotTree(toCreate, null)) yield return createSlot;
        foreach (var createComponent in CreateComponentTree(toCreate)) yield return createComponent;
        foreach (var updateComponent in UpdateComponentTree(toCreate)) yield return updateComponent;
    }

    private static IEnumerable<BatchMutation> CreateSlotTree(Slot toCreate, string? parent)
    {
        var parentRef = toCreate.Parent ?? new Reference();
        if (!string.IsNullOrEmpty(parent))
        {
            parentRef.TargetId = parent;
        }

        if ((toCreate.Children.Count > 0 || toCreate.Components.Count > 0)
            && string.IsNullOrEmpty(toCreate.Id))
        {
            toCreate.Id = Guid.NewGuid().ToString("N");
        }

        yield return new BatchMutation
        {
            AddSlot = new AddSlotRequest
            {
                Data = new Slot
                {
                    Id = toCreate.Id,
                    Parent = parentRef,
                    Name = toCreate.Name,
                    Tag = toCreate.Tag,
                    Position = toCreate.Position,
                    Rotation = toCreate.Rotation,
                    Scale = toCreate.Scale,
                    IsActive = toCreate.IsActive,
                    IsPersistent = toCreate.IsPersistent,
                    OrderOffset = toCreate.OrderOffset,
                }
            }
        };

        foreach (var child in toCreate.Children)
        {
            foreach (var expanded in CreateSlotTree(child, toCreate.Id)) yield return expanded;
        }
    }

    private static IEnumerable<BatchMutation> CreateComponentTree(Slot toCreate)
    {
        if ((toCreate.Children.Count > 0 || toCreate.Components.Count > 0)
            && string.IsNullOrEmpty(toCreate.Id))
        {
            toCreate.Id = Guid.NewGuid().ToString("N");
        }

        foreach (var component in toCreate.Components)
        {
            if (component.Members.Count > 0 && string.IsNullOrEmpty(component.Id))
            {
                component.Id = Guid.NewGuid().ToString("N");
            }

            yield return new BatchMutation
            {
                AddComponent = new AddComponentRequest
                {
                    ContainerSlotId = toCreate.Id,
                    Data = new Component
                    {
                        Id = component.Id,
                        ComponentType = component.ComponentType,
                    }
                }
            };
        }

        foreach (var child in toCreate.Children)
        {
            foreach (var expanded in CreateComponentTree(child)) yield return expanded;
        }
    }

    private static IEnumerable<BatchMutation> UpdateComponentTree(Slot toCreate)
    {
        foreach (var component in toCreate.Components)
        {
            if (component.Members.Count == 0)
            {
                continue;
            }

            yield return new BatchMutation
            {
                UpdateComponent = new UpdateComponentRequest
                {
                    Data = component
                }
            };
        }

        foreach (var child in toCreate.Children)
        {
            foreach (var expanded in UpdateComponentTree(child)) yield return expanded;
        }
    }
}
