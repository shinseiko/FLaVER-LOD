// Program.cs
// FLaVER-LOD main entry point

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

            string flverIn = "", objPath = "", flverOut = "";

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

            if (string.IsNullOrWhiteSpace(flverIn) || string.IsNullOrWhiteSpace(objPath))
            {
                Console.WriteLine("Missing input files.");
                return;
            }

			var flver = SoulsFile<FLVER2>.Read(flverIn);
            if (flver == null)
            {
                Console.WriteLine("Failed to read FLVER2 from file: " + flverIn);
                return;
            }
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
            Console.WriteLine($"Saved LOD FLVER: {flverOut}");
        }
    }
}
