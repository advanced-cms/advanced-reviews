@ECHO OFF
SETLOCAL

SET CONFIGURATION=Debug

IF "%2"=="Release" (SET CONFIGURATION=Release)

@REM CALL build.cmd Release
powershell ./build/pack.ps1  -version %1 -configuration %CONFIGURATION%

EXIT /B %errorlevel%
