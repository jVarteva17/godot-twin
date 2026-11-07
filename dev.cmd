@echo off
REM Team command line. See tools/dev.cs.
cd /d "%~dp0" && dotnet run tools/dev.cs -- %*
