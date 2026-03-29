@echo off
setlocal

set "PROJECT=%~dp0Aimoro.App\Aimoro.App.csproj"
set "OUTPUT=%~dp0artifacts\publish\win-x64"

dotnet publish "%PROJECT%" ^
  -c Release ^
  -r win-x64 ^
  --self-contained true ^
  /p:PublishSingleFile=true ^
  /p:IncludeNativeLibrariesForSelfExtract=true ^
  /p:DebugType=None ^
  /p:DebugSymbols=false ^
  -o "%OUTPUT%"

if errorlevel 1 exit /b 1

echo Published Aimoro to %OUTPUT%
