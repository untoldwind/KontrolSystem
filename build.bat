
if exist GameData\KontrolSystem\Plugin del GameData\KontrolSystem\Plugins\*.dll

msbuild /t:build,test /restore /Property:Configuration=Release
if %errorlevel% neq 0 exit /b %errorlevel%
