#!/usr/bin/env bash
set -e

if [[ x$1 == x'clear' ]]; then
    dotnet nuget locals http-cache --clear

elif [[ x$1 == x'build' ]]; then
    echo ================build=========================
    dotnet clean PomeloCli.sln -v m
    if [[ x$2 == x'' ]]; then
        dotnet restore PomeloCli.sln # --configfile nuget.config
        dotnet build PomeloCli.sln --no-restore -v m
    else
        dotnet restore $2 # --configfile nuget.config
        [[ -f $2 ]] && [[ $2 == *.csproj ]] && dotnet build $2 --no-restore -v m
    fi
    
elif [[ x$1 == x'pack' ]]; then
    echo ================pack=========================
    VERSION=$(grep -oP '(?<=<Version>).+(?=</Version>)' common.props)
    if [[ x$2 == x'' ]]; then
        dotnet pack PomeloCli.sln -o nupkgs -v m --no-build --no-restore
        find . -iwholename '*Plugin/PomeloCli.*.nuspec' -exec nuget pack {} -OutputDirectory nupkgs -Version $VERSION \;
    else
        [[ -f $2 ]] && [[ $2 == *.csproj ]] && dotnet pack $2 -o nupkgs -v m --no-build --no-restore
        [[ -f $2 ]] && [[ $2 == *.nuspec ]] && nuget pack $2 -OutputDirectory nupkgs -Version $VERSION
    fi

elif [[ x$1 == x'push' ]]; then
    echo ================push=========================
    VERSION=$(grep -oP '(?<=<Version>).+(?=</Version>)' common.props)
    if [[ x$2 == x'' ]]; then
        find nupkgs -iwholename "*.$VERSION.nupkg" -exec dotnet nuget push {} -s http://localhost:5555/v3/index.json -k NUGET-SERVER-API-KEY --skip-duplicate \;
    else
        [[ -f $2 ]] && dotnet nuget push $2 -s http://localhost:5555/v3/index.json  -k NUGET-SERVER-API-KEY --skip-duplicate
    fi

else
    echo command "$1" not support
fi