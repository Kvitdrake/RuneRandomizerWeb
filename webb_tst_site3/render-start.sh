#!/bin/bash
export PATH="$HOME/.dotnet:$PATH"
echo "===== Запуск приложения ====="
cd publish
dotnet webb_tst_site3.dll --urls "http://0.0.0.0:$PORT"