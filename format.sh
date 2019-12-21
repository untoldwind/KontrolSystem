#!/bin/sh

# astyle -N -A2 -R "Parsing/*.cs"
# astyle -N -A2 -R "Parsing-Test/*.cs"
# astyle -N -A2 -R "TO2/*.cs"
# astyle -N -A2 -R "TO2-Test/*.cs"
# astyle -N -A2 -R "KSPRuntime/*.cs"
# astyle -N -A2 -R "KSPRuntime-Test/*.cs"
# astyle -N -A2 -R "Plugin/*.cs"
# for i in $(find . -name "*.orig")
# do
#     rm $i
# done

~/.dotnet/tools/dotnet-format
