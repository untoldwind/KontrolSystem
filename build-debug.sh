#!/bin/sh

set -e

rm -f bin/Debug/KontrolSystemPlugin.dll

export RELOAD_VERSION="$(date -u +"%Y%m%d%H%M%S")"

msbuild -t:build,test -restore -Property:Configuration=Debug

rm -rf GameData/KontrolSystem/Plugins
mkdir -p GameData/KontrolSystem/Plugins

for module in Parsing TO2 KSPRuntime
do
    cp bin/Debug/KontrolSystem$module.dll GameData/KontrolSystem/Plugins
done

rm -f bin/Debug/KontrolSystemPlugin.dll
ln -s $PWD/bin/Debug/KontrolSystemPlugin_$RELOAD_VERSION.dll bin/Debug/KontrolSystemPlugin.dll
