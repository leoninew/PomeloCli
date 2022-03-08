#!/usr/bin/env bash
set -e

if [[ x$1 == x'compile' ]]; then
    echo ================compile=========================
    dotnet clean PomeloCli.sln -v m
    dotnet restore PomeloCli.sln # --configfile nuget.config
    dotnet build PomeloCli.sln --no-restore -v m

elif [[ x$1 == x'pack' ]]; then
    echo ================pack host=========================
    dotnet pack PomeloCli.sln -o nupkgs -v m --no-build --no-restore    
    echo ================pack plugin=========================
    VERSION=$(grep -oP '(?<=<Version>).+(?=</Version>)' common.props)
    find . -iwholename '*Plugin/PomeloCli.*.nuspec' -exec nuget pack {} -OutputDirectory nupkgs -Version $VERSION \;

elif [[ x$1 == x'upload' ]]; then
    echo ================pack upload=========================
    VERSION=$(grep -oP '(?<=<Version>).+(?=</Version>)' common.props)
    find nupkgs -iwholename "*.$VERSION.nupkg" -exec dotnet nuget push -s http://localhost:5555/v3/index.json -k NUGET-SERVER-API-KEY {} \;
    exit 0
fi