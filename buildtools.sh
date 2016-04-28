#!/bin/bash
mkdir tools

if [ -d "tools" ]; then
  rm -rf tools
fi

mkdir tools

xbuild /p:Configuration=Release /t:Clean src/FlexlibMono.sln
xbuild /p:Configuration=Release /t:Build src/FlexlibMono.sln
cp src/HB9FXQ.Queryutil/bin/Release/*.dll tools/
cp src/HB9FXQ.Queryutil/bin/Release/*.exe tools/
