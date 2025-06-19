#!/bin/bash
export DOTNET_ROOT=/opt/render/.dotnet
export PATH=$PATH:$DOTNET_ROOT

echo "===== Проверка окружения ====="
dotnet --info
ls -la /opt/render

echo "===== Запуск приложения ====="
cd /opt/render/publish
dotnet webb_tst_site3.dll --urls "http://0.0.0.0:$PORT"