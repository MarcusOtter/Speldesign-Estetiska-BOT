#!/bin/sh
UPSTREAM=$1
if [ -z "$UPSTREAM" ]; then
    echo "The upstream parameter is missing."
    exit
fi

git fetch origin

LOCAL=`git rev-parse @`
REMOTE=`git rev-parse $UPSTREAM`

# This statement assumes the remote is always ahead. Deploying should not be done on the developer machine.
if [ $LOCAL = $REMOTE ]; then
    echo "Up to date"
else
    echo "Outdated"
fi
