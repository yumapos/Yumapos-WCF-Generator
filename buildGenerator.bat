@echo off
setlocal

pushd "%~dp0" || exit /b 1

dotnet build "WCF-Generator\WCF-Generator.csproj" -c Debug

set "EXIT_CODE=%ERRORLEVEL%"
popd
exit /b %EXIT_CODE%
