#!/bin/sh

set -e

msbuild -t:build -restore -Property:Configuration=Release

mono $HOME/.nuget/packages/nunit.consolerunner/3.10.0/tools/nunit3-console.exe --inprocess bin/Release/KontrolSystemParsing-Test.dll

mono $HOME/.nuget/packages/nunit.consolerunner/3.10.0/tools/nunit3-console.exe --inprocess bin/Release/KontrolSystemTO2-Test.dll

mono $HOME/.nuget/packages/nunit.consolerunner/3.10.0/tools/nunit3-console.exe --inprocess bin/Release/KontrolSystemKSPRuntime-Test.dll

rm -rf GameData/KontrolSystem/Plugins
mkdir -p GameData/KontrolSystem/Plugins

for module in Parsing TO2 KSPRuntime Plugin
do
    cp bin/Release/KontrolSystem$module.dll GameData/KontrolSystem/Plugins
done