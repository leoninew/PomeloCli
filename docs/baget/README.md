- [Run BaGet on Docker](https://loic-sharma.github.io/BaGet/installation/docker/)

## Run BaGet

```bash
$ docker run --rm -it -p 8000:80 --env-file .env -v "$PWD/data:/var/baget" loicsharma/baget:latest
$ docker run --rm --name baget -p 8000:80 --env-file .env -v "$PWD/data:/var/baget" loicsharma/baget:latest
$ docker run -d --name baget -p 8000:80 --env-file .env -v "$PWD/data:/var/baget" loicsharma/baget:latest
```

## Publish packages

```bash
$ dotnet nuget push -s http://localhost:8000/v3/index.json -k NUGET-SERVER-API-KEY package.1.0.0.nupkg
```
