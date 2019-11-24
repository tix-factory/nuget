printenv
dotnet build ./../TixFactory.CookieJar/TixFactory.CookieJar.sln -p:Version=2.0.$GITHUB_ACTION --configuration Release
dotnet build ./../TixFactory.Http/TixFactory.Http.sln -p:Version=2.0.$GITHUB_ACTION --configuration Release
