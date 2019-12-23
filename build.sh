#!/bin/sh

set -e

msbuild /Property:Configuration=Release

mono packages/NUnit.ConsoleRunner.3.10.0/tools/nunit3-console.exe --inprocess bin/Release/KontrolSystemParsing-Test.dll

mono packages/NUnit.ConsoleRunner.3.10.0/tools/nunit3-console.exe --inprocess --workers=1 bin/Release/KontrolSystemTO2-Test.dll

mono packages/NUnit.ConsoleRunner.3.10.0/tools/nunit3-console.exe --inprocess bin/Release/KontrolSystemKSPRuntime-Test.dll

rm -rf GameData/KontrolSystem/Plugins
mkdir -p GameData/KontrolSystem/Plugins

for module in Parsing TO2 KSPRuntime Plugin
do
    cp bin/Release/KontrolSystem$module.dll GameData/KontrolSystem/Plugins
done