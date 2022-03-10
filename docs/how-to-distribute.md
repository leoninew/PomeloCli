[TOC]

PomeloCli 分为宿主与插件两个部分，各自发布和管理。

## 编译

使用 dotnet 编译

```bash
dotnet clean PomeloCli.sln
dotnet restore PomeloCli.sln # --configfile nuget.config
dotnet build PomeloCli.sln --no-restore
```

### 打包

打包宿主

```bash
dotnet pack PomeloCli.sln -o nupkgs -v m --no-build --no-restore
# dotnet pack PomeloCli.sln -c Release -o nupkgs -v m --no-build --no-restore
```

打包插件

```bash
VERSION=$(grep -oP '(?<=<Version>).+(?=</Version>)' common.props)
find . -iwholename '*Plugin/PomeloCli.*.nuspec' -exec nuget pack {} -OutputDirectory nupkgs -Version $VERSION \;
```

## 发布

发布宿主和插件

```bash
VERSION=$(grep -oP '(?<=<Version>).+(?=</Version>)' common.props)
find nupkgs -iwholename "*.$VERSION.nupkg" -exec dotnet nuget push -s http://localhost:5555/v3/index.json -k NUGET-SERVER-API-KEY {} --skip-duplicate \;
```

> 目前插件使用 nuspec 打包，以获取插件所需要的全量文件

## 管理

### 使用 nupkg 安装或更新宿主

安装使用 `dotnet tool install` 命令

```bash
dotnet tool install PomeloCli.DemoApp -g --add-source nupkgs
# and for beta version
dotnet tool install PomeloCli.DemoApp -g --add-source nupkgs --version $VERSION
```

更新使用 `dotnet tool update` 命令

```bash
dotnet tool update PomeloCli.DemoApp -g --add-source nupkgs
# for beta version
dotnet tool update PomeloCli.DemoApp -g --add-source nupkgs --version $VERSION
```

如果安装或更新遇到了缓存问题，可以使用以下命令清理，且同样对下文的插件管理有效

```bash
dotnet nuget locals http-cache --clear
```

### 使用 nuget 源安装或更新宿主

安装使用 `dotnet tool install` 命令

```bash
dotnet tool install PomeloCli.DemoApp -g --add-source http://localhost:5555/v3/index.json
# for beta version
dotnet tool install PomeloCli.DemoApp -g --add-source http://localhost:5555/v3/index.json --version $VERSION
```

更新使用 `dotnet tool update` 命令

```bash
dotnet tool update PomeloCli.DemoApp -g --add-source http://localhost:5555/v3/index.json
# for beta version
dotnet tool update PomeloCli.DemoApp -g --add-source http://localhost:5555/v3/index.json --version $VERSION
```

### 卸载宿主

卸载使用 `dotnet tool uninstall` 命令

```bash
dotnet tool uninstall PomeloCli.DemoApp -g
```

### 使用 nuget 源安装或更新插件

安装或更新均使用 `pomelo-cli plugin install` 命令，目前插件只支持 nuget 源

```bash
pomelo-cli plugin install PomeloCli.DemoPlugin -s http://localhost:5555/v3/index.json --version $VERSION
```

### 卸载插件

卸载使用 `pomelo-cli plugin uninstall` 命令，示例

```bash
pomelo-cli plugin uninstall PomeloCli.DemoPlugin
```