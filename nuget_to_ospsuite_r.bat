@echo off
REM Packs PK-Sim as local NuGet packages into OSPSuite.Core\nuget_repo,
REM updates the PKSim.R version in OSPSuite-R\DependencyManager.csproj,
REM and builds the DependencyManager to copy DLLs into OSPSuite-R\inst\lib.
REM
REM Prerequisites:
REM   - Build PKSim.sln in Debug configuration before running this script.
REM   - Unload .NET assemblies in Positron/R to avoid locked DLL errors.
cls
call rake create_local_nuget_r
