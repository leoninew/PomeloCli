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
    if [[ x$2 == x'' ]]; then
        find . -iwholename '*Plugin/DevCloud.Cli.*.nuspec' -exec nuget pack {} -OutputDirectory nupkgs -Version $VERSION \;
    else
        nuget pack $2 -OutputDirectory nupkgs -Version $VERSION
    fi    
elif [[ x$1 == x'upload' ]]; then
    echo ================pack upload=========================
    VERSION=$(grep -oP '(?<=<Version>).+(?=</Version>)' common.props)
    if [[ x$2 == x'' ]]; then
        find nupkgs -iwholename "*.$VERSION.nupkg" -exec dotnet nuget push -s http://localhost:5555/v3/index.json -k NUGET-SERVER-API-KEY {} \;
    else
        dotnet nuget push -k NUGET-SERVER-API-KEY -s http://localhost:5555/v3/index.json $2
    fi
fi