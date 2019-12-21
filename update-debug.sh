#!/bin/sh

set -e

rm -f bin/Debug/KontrolSystemPlugin.dll

export RELOAD_VERSION="$(date -u +"%Y%m%d%H%M%S")"

(cd Plugin; msbuild /Property:Configuration=Debug)

rm -f bin/Debug/KontrolSystemPlugin.dll
ln -s $PWD/bin/Debug/KontrolSystemPlugin_$RELOAD_VERSION.dll bin/Debug/KontrolSystemPlugin.dll
