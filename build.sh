#!/bin/sh

set -e

msbuild -t:build,test -restore -Property:Configuration=Release

rm -rf GameData/KontrolSystem/Plugins
mkdir -p GameData/KontrolSystem/Plugins

for module in Parsing TO2 KSPRuntime Plugin
do
    cp bin/Release/KontrolSystem$module.dll GameData/KontrolSystem/Plugins
done