#!/bin/bash
export DOTNET_ROOT=$HOME/.dotnet
export PATH=$PATH:$DOTNET_ROOT

echo "===== Проверка окружения ====="
dotnet --info
ls -la $HOME

cd $HOME/bin/Release/net8.0
dotnet webb_tst_site3.dll --urls "http://0.0.0.0:$PORT"