$ErrorActionPreference = "Stop"

# clean
dotnet clean DiscUtils.sln --configuration Release

# build
dotnet build DiscUtils.sln --configuration Release

# pack
dotnet pack DiscUtils.sln --configuration Release -o '.packages'
