namespace Rfmk.Resolink.Grpc.Converters;

public static partial class Convert
{
    public static AssetResponse ToProto(this ResoniteLink.AssetData self)
    {
        return new AssetResponse
        {
            AssetUrl = self.AssetURL.ToProto(),
        };
    }

    public static ResoniteLink.ImportTexture2DRawData ToModel(this Texture self)
    {
        return new ResoniteLink.ImportTexture2DRawData
        {
            RawBinaryPayload = self.Data.ToByteArray(),
            ColorProfile = self.ColorProfile,
            Height = self.Height,
            Width = self.Width,
        };
    }

    public static ResoniteLink.ImportMeshJSON ToModel(this MeshJson self)
    {
        return new ResoniteLink.ImportMeshJSON
        {
            Vertices = self.Vertices.Select(ToModel).ToList(),
            Submeshes = self.Submeshes.Select(ToModel).ToList(),
            Bones = self.Bones.Select(ToModel).ToList(),
            BlendShapes = self.BlendShapes.Select(ToModel).ToList(),
        };
    }

    private static ResoniteLink.Vertex ToModel(this Vertex self)
    {
        return new ResoniteLink.Vertex
        {
            Position = self.Position.ToModel(),
            Normal = self.Normal.ToModel(),
            Tangent = self.Tangent.ToModel(),
            Color = self.Color.ToModel(),
            UVs = self.Uvs.Select(ToModel).ToList(),
            BoneWeights = self.BoneWeights.Select(ToModel).ToList(),
        };
    }

    private static ResoniteLink.UV_Coordinate ToModel(this UVCoordinate self)
    {
        return self.CoordCase switch
        {
            UVCoordinate.CoordOneofCase.Uv2D => new ResoniteLink.UV2D_Coordinate
            {
                uv = self.Uv2D.ToModel(),
            },
            UVCoordinate.CoordOneofCase.Uv3D => new ResoniteLink.UV3D_Coordinate
            {
                uv = self.Uv3D.ToModel(),
            },
            UVCoordinate.CoordOneofCase.Uv4D => new ResoniteLink.UV4D_Coordinate
            {
                uv = self.Uv4D.ToModel(),
            },
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private static ResoniteLink.BoneWeight ToModel(this BoneWeight self)
    {
        return new ResoniteLink.BoneWeight
        {
            BoneIndex = self.BoneIndex,
            Weight = self.Weight,
        };
    }

    private static ResoniteLink.Submesh ToModel(this Submesh self)
    {
        return self.MeshKindCase switch
        {
            Submesh.MeshKindOneofCase.Points => new ResoniteLink.PointSubmesh
            {
                VertexIndicies = self.Points.VertexIndices.ToList(),
            },
            Submesh.MeshKindOneofCase.Trangles => new ResoniteLink.TriangleSubmesh
            {
                Triangles = self.Trangles.Trangles.Select(ToModel).ToList(),
            },
            Submesh.MeshKindOneofCase.TranglesFlat => new ResoniteLink.TriangleSubmeshFlat
            {
                VertexIndicies = self.TranglesFlat.VertexIndices.ToList(),
            },
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private static ResoniteLink.Triangle ToModel(this Triangle self)
    {
        return new ResoniteLink.Triangle
        {
            Vertex0Index = self.Vertex0Index,
            Vertex1Index = self.Vertex1Index,
            Vertex2Index = self.Vertex2Index,
        };
    }

    private static ResoniteLink.Bone ToModel(this Bone self)
    {
        return new ResoniteLink.Bone
        {
            Name = self.Name,
            BindPose = self.BindPose.ToModel(),
        };
    }

    private static ResoniteLink.BlendShape ToModel(this BlendShape self)
    {
        return new ResoniteLink.BlendShape
        {
            Name = self.Name,
            Frames = self.Frames.Select(ToModel).ToList(),
        };
    }

    private static ResoniteLink.BlendshapeFrame ToModel(this BlendShapeFrame self)
    {
        return new ResoniteLink.BlendshapeFrame
        {
            Position = self.Position,
            PositionDeltas = self.PositionDeltas.Select(ToModel).ToList(),
            NormalDeltas = self.NormalDeltas.Select(ToModel).ToList(),
            TangentDeltas = self.TangentDeltas.Select(ToModel).ToList(),
        };
    }

    public static ResoniteLink.ImportMeshRawData ToModel(this MeshRaw self)
    {
        return new ResoniteLink.ImportMeshRawData
        {
            RawBinaryPayload = self.Data.ToByteArray(),
            VertexCount = self.VertexCount,
            HasNormals = self.HasNormals,
            HasTangents = self.HasTangents,
            HasColors = self.HasColors,
            BoneWeightCount = self.BoneWeightCount,
            UV_Channel_Dimensions = self.UvChannelDimensions.ToList(),
            Submeshes = self.Submeshes.Select(ToModel).ToList(),
            BlendShapes = self.Blendshapes.Select(ToModel).ToList(),
            Bones = self.Bones.Select(ToModel).ToList(),
        };
    }

    private static ResoniteLink.SubmeshRawData ToModel(this RawSubmesh self)
    {
        return self.SubmeshCase switch
        {
            RawSubmesh.SubmeshOneofCase.Point => new ResoniteLink.PointSubmeshRawData
            {
                PointCount = self.Point.PointCount,
            },
            RawSubmesh.SubmeshOneofCase.Trangle => new ResoniteLink.TriangleSubmeshRawData
            {
                TriangleCount = self.Trangle.TriangleCount,
            },
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private static ResoniteLink.BlendShapeRawData ToModel(this RawBlendshape self)
    {
        return new ResoniteLink.BlendShapeRawData
        {
            Name = self.Name,
            HasNormalDeltas = self.HasNormalDeltas,
            HasTangentDeltas = self.HasTangentDeltas,
            Frames = self.Frames.Select(ToModel).ToList(),
        };
    }

    private static ResoniteLink.BlendShapeFrameRawData ToModel(this RawBlendshapeFrame self)
    {
        return new ResoniteLink.BlendShapeFrameRawData
        {
            Position = self.Position,
        };
    }

    public static ResoniteLink.ImportAudioClipRawData ToModel(this RawAudioClip self)
    {
        return new ResoniteLink.ImportAudioClipRawData
        {
            RawBinaryPayload = self.Data.ToByteArray(),
            SampleCount = self.SampleCount,
            SampleRate = self.SampleRate,
            ChannelCount = self.ChannelCount,
        };
    }
}
