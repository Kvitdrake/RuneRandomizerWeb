#!/bin/bash
echo "----- Building .NET application -----"
cd ./webb_tst_site3
dotnet publish -c Release -o ../publish