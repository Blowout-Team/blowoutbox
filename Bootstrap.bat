@echo off

if not exist ".\engine\BlowoutTeamSoft\Compilation.Module\bin\BCompiler.exe" dotnet build .\engine\BlowoutTeamSoft\Compilation.Module\BlowoutTeamSoft.Compilation.Module.csproj --configuration Blowout_Source2_Debug 

.\engine\BlowoutTeamSoft\Compilation.Module\bin\BCompiler.exe precompile

REM For the time being, we will leave an indication only of the 'BlowoutTeamSoft.*' projects and assemblies. The features of the compiler are currently not used in the Sandbox. For quick compilation, we are currently using BlowoutTeamSoft.
.\engine\BlowoutTeamSoft\Compilation.Module\bin\BCompiler.exe compilation --compile-filter "BlowoutTeamSoft.*" --out-directory "BGenerated" --rel-proj-src true --expose-errors true

dotnet run --project .\engine\Tools\SboxBuild\SboxBuild.csproj -- build --config Developer

.\engine\BlowoutTeamSoft\Compilation.Module\bin\BCompiler.exe post_compilation --compile-filter "BlowoutTeamSoft.*" --root-directory "game/bin/managed" --compile-method "target-dir"

dotnet run --project .\engine\Tools\SboxBuild\SboxBuild.csproj -- build-shaders
dotnet run --project .\engine\Tools\SboxBuild\SboxBuild.csproj -- build-content