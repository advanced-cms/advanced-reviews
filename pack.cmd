@ECHO OFF
SETLOCAL

SET CONFIGURATION=Debug

IF "%2"=="Release" (SET CONFIGURATION=Release)

powershell ./build/pack.ps1  -version %1 -configuration %CONFIGURATION%

EXIT /B %errorlevel%
