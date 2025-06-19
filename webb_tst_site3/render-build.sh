#!/bin/bash
echo "----- Restoring dependencies -----"
dotnet restore
echo "----- Publishing application -----"
dotnet publish -c Release -o ./publish --no-restore