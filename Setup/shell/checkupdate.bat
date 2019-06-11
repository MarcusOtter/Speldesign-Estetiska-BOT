@ECHO OFF
SET UPSTREAM=%1
IF [%UPSTREAM%]==[] (
    ECHO The upstream parameter is missing.
    goto :end
)

git fetch origin

REM Saves the output (%%O) of "git rev-parse @" (the latest commit on head) to %LOCAL%
FOR /F %%O IN ('git rev-parse @') DO (SET LOCAL=%%O)

REM Saves the output (%%O) of "git rev-parse %UPSTREAM%" (latest commit on the upstream) to %REMOTE%
FOR /F %%O IN ('git rev-parse %UPSTREAM%') DO (SET REMOTE=%%O)

REM This statement assumes the remote is always ahead. Deploying should not be done on the developer machine.
IF %LOCAL%==%REMOTE% (
    ECHO Up to date
) ELSE (
    ECHO Outdated
)

:end
