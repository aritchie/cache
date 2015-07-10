@echo off
del *.nupkg
nuget pack Acr.Cache.nuspec
nuget pack Acr.Cache.Sqlite.nuspec
pause