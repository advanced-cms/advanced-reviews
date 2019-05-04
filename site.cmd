CALL npm run build:widget --prefix react-components
CALL npm run build:external-review-widget --prefix react-components
CALL "C:\Program Files\IIS Express\iisexpress.exe" /path:%~dp0src\Alloy.Mvc.Template
