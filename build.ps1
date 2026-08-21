$ErrorActionPreference = "Stop"
dotnet publish .\RealWorldLauncher\RealWorldLauncher.csproj `
  -c Release -r win-x64 --self-contained true `
  -p:PublishSingleFile=true `
  -p:IncludeNativeLibrariesForSelfExtract=true `
  -o .\publish
Write-Host "Готово: .\publish\RealWorldLauncher.exe"