@ECHO OFF

REM Set Release or Debug configuration.
IF "%1"=="Release" (set CONFIGURATION=Release) ELSE (set CONFIGURATION=Debug)
ECHO Building in %CONFIGURATION%

REM Build the C# solution.
CALL dotnet build -c %CONFIGURATION% /p:CheckEolTargetFramework=false
IF %errorlevel% NEQ 0 EXIT /B %errorlevel%

REM Build client side
CD src\Advanced.CMS.ApprovalReviews\React
CALL yarn build
IF %errorlevel% NEQ 0 EXIT /B %errorlevel%
CD ..\..\..\
