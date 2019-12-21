msbuild /Property:Configuration=Release
if %errorlevel% neq 0 exit /b %errorlevel%

packages\NUnit.ConsoleRunner.3.10.0\tools\nunit3-console.exe bin\Release\KontrolSystemParsing-Test.dll
if %errorlevel% neq 0 exit /b %errorlevel%

packages\NUnit.ConsoleRunner.3.10.0\tools\nunit3-console.exe bin\Release\KontrolSystemTO2-Test.dll
if %errorlevel% neq 0 exit /b %errorlevel%

packages\NUnit.ConsoleRunner.3.10.0\tools\nunit3-console.exe bin\Release\KontrolSystemKSPRuntime-Test.dll
if %errorlevel% neq 0 exit /b %errorlevel%

if exist GameData\KontrolSystem\Plugin del GameData\KontrolSystem\Plugins\*.dll
if not exist GameData\KontrolSystem\Plugins mkdir GameData\KontrolSystem\Plugins

copy bin\Release\KontrolSystemParsing.dll GameData\KontrolSystem\Plugins
copy bin\Release\KontrolSystemTO2.dll GameData\KontrolSystem\Plugins
copy bin\Release\KontrolSystemKSPRuntime.dll GameData\KontrolSystem\Plugins
copy bin\Release\KontrolSystemPlugin.dll GameData\KontrolSystem\Plugins
