#!/bin/bash

# https://stackoverflow.com/a/821419/1663648
set -e

echo "START build.sh"
echo "BUILD_NUMBER: $BUILD_NUMBER"

# https://stackoverflow.com/a/25119904/1663648
if [ "$GITHUB_ACTIONS" = "true" ]; then configuration="Release"; else configuration="Debug"; fi

slns=(
	"TixFactory.Logging/TixFactory.Logging.sln"
	"TixFactory.Operations/TixFactory.Operations.sln"
	"TixFactory.CookieJar/TixFactory.CookieJar.sln"
	"TixFactory.Http/TixFactory.Http.sln"
	"TixFactory.Firebase/TixFactory.Firebase.sln"
	"TixFactory.Configuration/TixFactory.Configuration.sln"
	"TixFactory.Queueing/TixFactory.Queueing.sln"
)

echo "Building ${#slns[@]} solutions (configuration: $configuration)..."

# https://stackoverflow.com/a/18898718/1663648
for sln in "${slns[@]}"
do
    echo "Building $sln..."
	dotnet build ./../$sln -p:Version=2.0.$BUILD_NUMBER --configuration $configuration
done

echo "END build.sh"
