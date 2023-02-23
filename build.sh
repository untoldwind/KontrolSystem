#!/bin/sh

set -e

rm -rf GameData/KontrolSystem/Plugins

msbuild -t:build -restore -Property:Configuration=Release
msbuild -t:test -Property:Configuration=Release
