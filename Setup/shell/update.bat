@ECHO OFF
SET UPSTREAM=%1
IF [%UPSTREAM%]==[] (
    ECHO The upstream parameter is missing.
    goto :end
)

git merge %UPSTREAM%
dotnet publish -c Release > nul
ECHO Updated
:end
