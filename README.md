# ency-extension-template

Template for an [ENCY](https://encycam.com) extension that **publishes itself to the
[ENCY Extension Store](https://dmc.encycam.com/store) on every version tag**. Write code,
push a tag — the extension appears in the store. No files copied or uploaded by hand.

## Quick start

1. **Use this template** (GitHub button) → clone your new repo.
2. Rename the extension:
   ```powershell
   .\rename.ps1 -Name MyCoolExtension
   ```
3. Claim the extension name for this repository — **once, and no secret ends up in GitHub**:
   ```bash
   ency-extension-mcp login          # once per machine
   ency-extension-mcp claim MyCoolExtension owner/MyCoolExtension
   ```
   From then on every publish, the first one included, authenticates with the workflow's own GitHub
   OIDC token: nothing is stored in the repository and nothing expires.

   *Without the CLI:* add an `ENCY_STORE_TOKEN` secret (Settings → Secrets and variables → Actions)
   with a store API token. The first publish then registers this repository as the extension's
   trusted publisher and the secret can be deleted afterwards.
4. Write your code in `src/` (start at `Extension.cs`), fill `src/readme.md` (it becomes the
   store card README) and `description`/`author` in `src/package.info.json`.
5. Publish:
   ```bash
   git tag v0.1.0 && git push --tags
   ```
   The workflow builds, packs and publishes; the card link is in the job summary.

## Local build & try-out

```powershell
dotnet build src -c Release -t:PackReady   # flat, pack-ready output in src\bin\Release
```

(`-t:PackReady` = normal build + sweeps the SDK doc-xmls out of the output; see the csproj.)

To try the built extension in a local ENCY before publishing: install it via the store card
after a publish (new extensions are reachable by direct link right away), or — if you have the
ENCY pack CLI — pack `src\bin\Release` yourself and use file install. CI needs no packing tool:
the store backend packs the uploaded build output itself.

## Layout

| Path | Purpose |
|---|---|
| `src/Extension.cs` | your extension logic (`IExtensionUtility.Run`) |
| `src/ExtensionFactory.cs` | entry point ENCY looks for (`CAMAPI.ExtensionFactory`) — keep the class/namespace |
| `src/EncyExtension.settings.json` | declares the extensions of this dll for ENCY (ids must match the factory) |
| `src/package.info.json` | store metadata: packageId, version, `tags` (keep the `ency-extension` marker!), sdkVersion |
| `src/readme.md` | store card README |
| `.github/workflows/publish.yml` | tag → build → pack → publish |

## Rules worth knowing

- The `ency-extension` tag in `package.info.json` is the **store marker** — remove it and the
  catalog will not index your package (the publish step fails fast on this).
- The tag `vX.Y.Z` overrides the version in `package.info.json` at publish time.
- `sdkVersion` controls the "minimal ENCY version" hint on the card — bump it when you bump
  the `EncySoftware.CAMAPI.Sdk.Net` package.
- The publisher (token owner) becomes the extension owner in the store.
