@echo off
dotnet build .\engine\BlowoutTeamSoft\Compilation.Module\BlowoutTeamSoft.Compilation.Module.csproj --configuration Blowout_Source2_Debug 

.\engine\BlowoutTeamSoft\Compilation.Module\bin\BCompiler.exe %*