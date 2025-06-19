#!/bin/bash
echo "===== Установка .NET 8 ====="
# Принудительная установка .NET
curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --version 8.0.100
export PATH="$HOME/.dotnet:$PATH"

echo "===== Пути проекта ====="
pwd
ls -la

echo "===== Сборка .NET приложения ====="
dotnet restore
dotnet publish -c Release -o ../publish