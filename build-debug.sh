#!/bin/sh

set -e

rm -rf GameData/KontrolSystem/Plugins

msbuild -t:build,test -restore -Property:Configuration=Debug
