@ECHO OFF
cd ..\
dotnet publish ..\..\..\SpeldesignBotCore.csproj -c Release
cls
dotnet SpeldesignBotCore.dll
