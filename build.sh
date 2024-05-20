#!/usr/bin/env bash
set -e

if [[ x$1 == x'clean' ]]; then
    [[ -d nupkgs ]] && find nupkgs -iname "*.nupkg" -delete
    find src -iname "*.csproj" -exec dotnet clean "{}" -v m \;
    find src -iname "*.csproj" -exec echo dotnet clean "{}" -v m \;

elif [[ x$1 == x'pack' ]]; then
    [[ x$2 == x'--debug' ]] && DEBUG="-c Debug"
    find src -iname "*.csproj" -exec echo dotnet pack "{}" -o nupkgs $DEBUG -v m \;
    find src -iname "*.csproj" -exec dotnet pack "{}" -o nupkgs $DEBUG -v m \;
    # dotnet pack docs/sample/4-sample-plugin/SamplePlugin.csproj -o nupkgs $DEBUG -v m

elif [[ x$1 == x'push' ]]; then
    find nupkgs -iname "*.nupkg" -exec dotnet nuget push -s http://localhost:8000/v3/index.json "{}" \;

else
    echo 'Available command:'
    echo '  clean, pack, push'
fi
