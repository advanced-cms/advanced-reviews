@ECHO OFF
SETLOCAL

SET PATH=.\.ci\tools\;%PATH%

REM Set Release or Debug configuration.
IF "%1"=="Release" (set CONFIGURATION=Release) ELSE (set CONFIGURATION=Debug)
ECHO Testing in %CONFIGURATION%

ECHO Running c# tests
CALL dotnet test -c %CONFIGURATION%
IF %errorlevel% NEQ 0 EXIT /B %errorlevel%

EXIT /B %ERRORLEVEL%
