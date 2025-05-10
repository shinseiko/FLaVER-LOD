// ObjParser.cs
// Minimal .obj parser to extract vertices, normals, UVs, and faces for use with FLVER2

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

public static class ObjParser
{
    public class ObjMesh
    {
        public List<SoulsFormats.FLVER.Vertex> Vertices = new();
        public List<SoulsFormats.FLVER2.FaceSet> FaceSets = new();

        public SoulsFormats.FLVER2.Mesh.BoundingBoxes CalculateBoundingBox()
        {
            var min = new System.Numerics.Vector3(float.MaxValue);
            var max = new System.Numerics.Vector3(float.MinValue);

            foreach (var v in Vertices)
            {
                min = System.Numerics.Vector3.Min(min, v.Position);
                max = System.Numerics.Vector3.Max(max, v.Position);
            }

            return new SoulsFormats.FLVER2.Mesh.BoundingBoxes { Min = min, Max = max };
        }
    }

    public static List<ObjMesh> LoadOBJ(string path)
    {
        var positions = new List<System.Numerics.Vector3>();
        var normals = new List<System.Numerics.Vector3>();
        var uvs = new List<System.Numerics.Vector2>();
        var meshes = new List<ObjMesh> { new ObjMesh() };
        var vertexDict = new Dictionary<string, int>();

        using var reader = new StreamReader(path);
        string line;

        while ((line = reader.ReadLine()) != null)
        {
            line = line.Trim();
            if (line.StartsWith("#") || line.Length == 0) continue;

            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            switch (parts[0])
            {
                case "v":
                    positions.Add(ParseVec3(parts));
                    break;
                case "vn":
                    normals.Add(ParseVec3(parts));
                    break;
                case "vt":
                    uvs.Add(ParseVec2(parts));
                    break;
                case "f":
                    var faceVerts = new List<ushort>();
                    foreach (var part in parts[1..])
                    {
                        var indices = part.Split('/');
                        System.Numerics.Vector2 uv = indices.Length > 1 && indices[1] != "" ? uvs[int.Parse(indices[1]) - 1] : System.Numerics.Vector2.Zero;
                        var pos = positions[int.Parse(indices[0]) - 1];
                        var norm = indices.Length > 2 ? normals[int.Parse(indices[2]) - 1] : default;

                        var vert = new SoulsFormats.FLVER.Vertex();
                        vert.Position = pos;
                        vert.Normal = norm;
                        vert.UVs = new List<System.Numerics.Vector3> { new(uv.X, 1 - uv.Y, 0) };
                        vert.BoneWeights = new SoulsFormats.FLVER.VertexBoneWeights();
                        vert.Tangents = new List<System.Numerics.Vector4>();

                        int index = meshes[0].Vertices.Count;
                        vertexDict[part] = index;
                        meshes[0].Vertices.Add(vert);
                        faceVerts.Add((ushort)index);
                    }

                    for (int i = 1; i < faceVerts.Count - 1; i++)
                    {
                        meshes[0].FaceSets.Add(new SoulsFormats.FLVER2.FaceSet
                        {
                            Flags = SoulsFormats.FLVER2.FaceSet.FSFlags.None,
                            CullBackfaces = true,
                            TriangleStrip = false,
                            Indices = new List<int> { faceVerts[0], faceVerts[i], faceVerts[i + 1] }
                        });
                    }
                    break;
                case "o":
                case "g":
                    break;
            }
        }

        return meshes;
    }

    private static System.Numerics.Vector3 ParseVec3(string[] parts)
    {
        return new System.Numerics.Vector3(
            float.Parse(parts[1], CultureInfo.InvariantCulture),
            float.Parse(parts[2], CultureInfo.InvariantCulture),
            float.Parse(parts[3], CultureInfo.InvariantCulture));
    }

    private static System.Numerics.Vector2 ParseVec2(string[] parts)
    {
        return new System.Numerics.Vector2(
            float.Parse(parts[1], CultureInfo.InvariantCulture),
            float.Parse(parts[2], CultureInfo.InvariantCulture));
    }
}
