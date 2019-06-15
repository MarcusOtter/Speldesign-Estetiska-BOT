#!/bin/sh
UPSTREAM=$1
if [ -z "$UPSTREAM" ]; then
    echo "The upstream parameter is missing."
    exit
fi

git merge $UPSTREAM

SCRIPT_PATH=$(dirname `which $0`)
cd ${SCRIPT_PATH}/..
dotnet publish ../../../SpeldesignBotCore.csproj -c Release

echo "Updated"
