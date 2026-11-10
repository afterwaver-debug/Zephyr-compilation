$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot

Write-Host "Building Zephyr for Windows x64..." -ForegroundColor Magenta
dotnet publish .\Zephyr.csproj -c Release -r win-x64 --self-contained true `
  -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true `
  -p:EnableCompressionInSingleFile=true

$exe = Join-Path $PSScriptRoot "bin\Release\net8.0-windows\win-x64\publish\Zephyr.exe"
if (Test-Path $exe) {
    Write-Host "`nBuild complete: $exe" -ForegroundColor Green
    Start-Process explorer.exe (Split-Path $exe)
} else {
    throw "Build finished but Zephyr.exe was not found at the expected path."
}
