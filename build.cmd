REM Set Release or Debug configuration.
IF "%1"=="Release" (set CONFIGURATION=Release) ELSE (set CONFIGURATION=Debug)
ECHO Building in %CONFIGURATION%

REM Build the C# solution.
CALL dotnet build -c %CONFIGURATION%
IF %errorlevel% NEQ 0 EXIT /B %errorlevel%

REM Build webpack.
IF "%1"=="Release" (set WEBPACK_CONFIGURATION=build) ELSE (set WEBPACK_CONFIGURATION=build:debug)
CALL npm run %WEBPACK_CONFIGURATION% --prefix ./src/ui