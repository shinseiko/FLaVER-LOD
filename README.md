# FLaVER-LOD

**FLVER Level-of-Detail injector for Elden Ring.**  
This CLI tool lets you import decimated `.obj` geometry as LOD meshes into Elden Ring `.flver` weapon models, preserving materials and vertex layout.

---

## ✅ Features

- 🔁 Injects mesh geometry from `.obj` into FLVER2 files
- 🧠 Supports `v`, `vn`, `vt`, and `f` lines (positions, normals, UVs)
- ⚙ Replaces `Vertices` and `FaceSets` in FLVER mesh slots
- 📦 Rebuilds bounding boxes automatically
- 🔍 Logs mesh count, layout info, and warnings
- 🧪 Compatible with SoulsFormats `.flver` parser

---

## 🔧 Usage

```bash
FLaVER-LOD.exe -in <input.flver> -obj <mesh.obj> -out <output.flver>
```

### Example:

```bash
FLaVER-LOD.exe -in example/wp_a_0800.flver -obj example/wp_a_0800_l.obj -out wp_a_0800_l.flver
```

---

## 📁 Input Requirements

### FLVER
- Must be a valid Elden Ring FLVER2 model file
- Mesh/material layout must match decimated model

### OBJ
- Must contain:
  - `v`  → vertex positions
  - `vn` → vertex normals
  - `vt` → UVs (1 set, UV0 only)
  - `f`  → triangle faces using `v/vt/vn` indices
- One-to-one mesh mapping is assumed (e.g., 29 meshes → 29 FLVER slots)

---

## 🚧 Limitations

- Only handles triangle meshes (no quads)
- UV1, vertex colors, tangents not yet supported
- Materials and bones are **preserved**, not editable
- No support for animated FLVERs (yet)

---

## 🧱 Build Instructions

- Requires [.NET 6 SDK](https://dotnet.microsoft.com/download)
- Requires [`SoulsFormats.dll`](https://github.com/JKAnderson/SoulsFormats)
  - Drop it in `src/` or next to `FLaVER-LOD.exe`
- Build with:
  ```bash
  dotnet build
  ```

Or publish a self-contained binary:
```bash
dotnet publish -r win-x64 -c Release
```

---

## 📜 License

MIT — see [LICENSE](LICENSE).

---

## 🔗 Credits

- FLVER handling: [SoulsFormats](https://github.com/JKAnderson/SoulsFormats) by JKAnderson
- Built by Roku (祿)
