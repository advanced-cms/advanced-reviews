@ECHO OFF
SETLOCAL

SET CONFIGURATION=Debug

IF "%1"=="Release" (SET CONFIGURATION=Release)

powershell ./build/pack.ps1 -configuration %CONFIGURATION%

EXIT /B %errorlevel%
