#!/bin/bash

# https://stackoverflow.com/a/821419/1663648
set -e

echo "START test.sh"
echo "BUILD_NUMBER: $BUILD_NUMBER"
start=`date +%s`

if [ "$GITHUB_ACTIONS" = "true" ]
then
	configuration="Release"
else
	configuration="Debug"
fi

dotnet msbuild "./build.proj" -t:Test -maxcpucount:64 -p:Configuration=$configuration

end=`date +%s`
runtime=$((end-start))
echo "END test.sh (runtime: $runtime seconds)"
