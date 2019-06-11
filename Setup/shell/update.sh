#!/bin/sh
UPSTREAM=$1
if [ -z "$UPSTREAM" ]; then
    echo "The upstream parameter is missing."
    exit
fi

git merge $UPSTREAM
dotnet publish -c Release > /dev/null
echo "Updated"
