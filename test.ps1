./dotnet --version

./dotnet new globaljson --force

./dotnet build ./src/TestTool.csproj -o ./out

./dotnet ./out/TestTool.dll