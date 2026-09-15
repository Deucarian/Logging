# Third-party notices

This notice describes the dependency and distribution inventory for `com.deucarian.logging` `1.0.1`. It does not replace the repository's [MIT license](LICENSE.md), and it does not grant rights to software supplied separately.

## Review basis

The reviewed baseline is `origin/main` commit `abf576b112f86762c775436fb3e477d61e8e9501`. Its `npm pack --dry-run` inventory contained 72 package files. The tracked and packed inventories were checked for common vendor/third-party directories, compiled binaries and archives, Git submodules, Git LFS pointers, separate license markers, and media/font assets.

That inventory identified no files marked or located as vendored third-party source, no compiled binary/archive candidates, no submodules, no LFS pointers, and no media/font asset candidates.

## Deucarian dependencies (not third-party)

| Package | Version | Relationship | License |
|---|---:|---|---|
| `com.deucarian.editor` | `1.0.0` | Direct package dependency, resolved separately by Unity Package Manager | [MIT](https://github.com/Deucarian/Editor/blob/main/LICENSE.md) |

No non-Deucarian package dependency is declared by the reviewed manifest.

## Host platform

The manifest requires Unity `2021.3`. Unity is a host platform and is not included in this package. Use of Unity is governed by the applicable [Unity Editor Software Terms](https://unity.com/legal/editor-terms-of-service/software).

Re-run the inventory and update this notice whenever dependencies or distributed content change.
