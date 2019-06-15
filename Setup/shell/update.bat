@ECHO OFF
SET UPSTREAM=%1
IF [%UPSTREAM%]==[] (
    ECHO The upstream parameter is missing.
    goto :end
)

git merge %UPSTREAM%
cd ..\
dotnet publish ..\..\..\SpeldesignBotCore.csproj -c Release

ECHO Updated
:end
