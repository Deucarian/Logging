# Deucarian Logging Agent Notes

Package ID: `com.deucarian.logging`
Repository: `Deucarian/Logging`

Follow the canonical Deucarian governance docs in [Package Registry](https://github.com/Deucarian/Package-Registry/blob/main/ARCHITECTURE.md), especially capability ownership and dependency rules.

## Ownership

This package owns:

- Logging facade, categories, sinks, and Unity console forwarding.

Registered capabilities:
- `logging`

This package must not own:

- Telemetry, diagnostics UI, upload pipelines, or unrelated editor chrome.

## Dependencies

Allowed dependency shape:

- May depend on Editor for editor-facing logging tooling only.

Required dependencies and why:

- `com.deucarian.editor`: shared editor shell/resources.

Optional/version-defined dependencies:

- None.

Architecture exceptions:

- Direct Unity Debug calls are allowed only in `Runtime/UnityConsoleLogSink.cs` and `DeucarianLog.ReportSinkFailure`.

## Policies

- Logging: This package owns direct Unity console forwarding; keep direct Debug calls inside approved sink/fallback points.
- Common: Do not add Common unless production runtime code directly uses its approved primitive.
- Editor UI: Editor dependency is for editor support only; do not expand into general editor shell ownership.
- Diagnostics: Do not become Diagnostics or telemetry.
- Testing: Tests should assert facade/sink behavior without adding new direct Debug policy exceptions.

## Validation

Run the shared validator before committing:

```powershell
python C:/Repositories/Package-Registry/Tools/deucarian_package_validator.py --registry-root C:/Repositories/Package-Registry --repository-root . --config deucarian-package.json
```

Also run existing repository tests when changing code or asmdefs. Documentation-only updates should still run `git diff --check`.

## Codex Guidance

- Inspect current files before changing anything.
- Work on `develop`; do not edit or merge `main` unless the task is promotion-only.
- Do not edit `Library/PackageCache`.
- Do not guess package versions or dependency versions.
- Do not add package dependencies casually; update asmdefs, `package.json`, `deucarian-package.json`, Package Registry, and fallback catalogs together when a dependency is truly required.
- Do not create local copies of shared helpers.
- Keep commits focused and report exactly what changed and what was validated.

## Before Adding Code

- Confirm the change fits this package's ownership boundary.
- Reuse existing local patterns and helpers.
- Avoid broad refactors without audit support.
- Preserve runtime/editor behavior unless the task explicitly asks to change it.

## Before Adding A Dependency

- Is the capability already owned by that package?
- Is it used by production code, editor code, sample code, or tests?
- Does the asmdef reference match `package.json`?
- Does `deucarian-package.json` need updating?
- Does Package Registry need updating?
- Does Package Installer fallback catalog need updating?
- Does Bootstrap fallback catalog need updating?
- Are exact versions propagated without guessing?

## Before Adding A Helper

- Is this package the capability owner?
- Is this behavior repeated in at least three production packages?
- Is there an existing owner package?
- Should this remain local?
- Has the audit been updated?

## Debug And Unity Object Lifetime

- This package owns the approved Unity console sink/fallback. Keep direct Unity Debug calls limited to the configured approved files.
- Do not copy Common lifetime helpers. Add Common only if production code directly owns transient Unity object cleanup.
- Test fixture teardown may use `DestroyImmediate` directly.
