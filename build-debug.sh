#!/bin/sh

set -e

rm -f bin/Debug/KontrolSystemPlugin.dll

export RELOAD_VERSION="$(date -u +"%Y%m%d%H%M%S")"

msbuild /Property:Configuration=Debug

mono packages/NUnit.ConsoleRunner.3.10.0/tools/nunit3-console.exe --inprocess bin/Debug/KontrolSystemParsing-Test.dll

mono packages/NUnit.ConsoleRunner.3.10.0/tools/nunit3-console.exe --inprocess bin/Debug/KontrolSystemTO2-Test.dll

# mono packages/NUnit.ConsoleRunner.3.10.0/tools/nunit3-console.exe --inprocess bin/Debug/KontrolSystemKSPRuntime-Test.dll

rm -rf GameData/KontrolSystem/Plugins
mkdir -p GameData/KontrolSystem/Plugins

for module in Parsing TO2 KSPRuntime
do
    cp bin/Debug/KontrolSystem$module.dll GameData/KontrolSystem/Plugins
done

rm -f bin/Debug/KontrolSystemPlugin.dll
ln -s $PWD/bin/Debug/KontrolSystemPlugin_$RELOAD_VERSION.dll bin/Debug/KontrolSystemPlugin.dll
