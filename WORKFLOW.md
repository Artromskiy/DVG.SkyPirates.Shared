# SkyPirates.Shared workflow

```bash
dotnet restore SkyPirates.Shared.csproj
dotnet build SkyPirates.Shared.csproj -c Release --no-restore \
  --disable-build-servers -m:1 /p:UseSharedCompilation=false
git diff --check
```
