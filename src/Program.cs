// FLaVER-LOD.cs
// A console tool to inject new mesh geometry into a FLVER file using Souls-compatible .obj files.
// Usage:
// FLaVER-LOD.exe -in wp_a_0800.flver -obj wp_a_0800_l.obj -out wp_a_0800_l.flver

using System;
using System.IO;
using SoulsFormats;

namespace FLaVER_LOD
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 6)
            {
                Console.WriteLine("Usage: FLaVER-LOD -in <input.flver> -obj <mesh.obj> -out <output.flver>");
                return;
            }

            string flverIn = null, objPath = null, flverOut = null;

            for (int i = 0; i < args.Length; i += 2)
            {
                switch (args[i])
                {
                    case "-in":
                        flverIn = args[i + 1];
                        break;
                    case "-obj":
                        objPath = args[i + 1];
                        break;
                    case "-out":
                        flverOut = args[i + 1];
                        break;
                }
            }

            if (!File.Exists(flverIn) || !File.Exists(objPath))
            {
                Console.WriteLine("Missing input files.");
                return;
            }

            FLVER2 flver = FLVER2.Read(flverIn);
            Console.WriteLine($"Loaded FLVER: {flverIn} with {flver.Meshes.Count} meshes");

            var objMeshes = ObjParser.LoadOBJ(objPath);
            if (objMeshes.Count != flver.Meshes.Count)
            {
                Console.WriteLine($"[!] Mesh count mismatch: OBJ has {objMeshes.Count}, FLVER has {flver.Meshes.Count}");
                return;
            }

            for (int i = 0; i < flver.Meshes.Count; i++)
            {
                flver.Meshes[i].Vertices = objMeshes[i].Vertices;
                flver.Meshes[i].FaceSets = objMeshes[i].FaceSets;
                flver.Meshes[i].BoundingBox = objMeshes[i].CalculateBoundingBox();
            }

            flver.Write(flverOut);
            Console.WriteLine($"Saved FLVER with LOD: {flverOut}");
        }
    }
} 
