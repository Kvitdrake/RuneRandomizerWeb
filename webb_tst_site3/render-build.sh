#!/bin/bash
set -e  # Прерывать при ошибках

echo "===== Установка .NET 8 ====="
# Устанавливаем .NET в системную папку
curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --version 8.0.100 --install-dir /opt/render/.dotnet
export DOTNET_ROOT=/opt/render/.dotnet
export PATH=$PATH:$DOTNET_ROOT

echo "===== Пути проекта ====="
pwd
ls -la

echo "===== Переход в папку проекта ====="

echo "===== Восстановление зависимостей ====="
dotnet restore

echo "===== Сборка приложения ====="
dotnet publish -c Release -o /opt/render/publish -r linux-x64 --self-contained true