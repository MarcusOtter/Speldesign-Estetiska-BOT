#!/bin/sh
SCRIPT_PATH=$(dirname `which $0`)
cd ${SCRIPT_PATH}/..
dotnet publish ../../../SpeldesignBotCore.csproj -c Release
clear
dotnet SpeldesignBotCore.dll
