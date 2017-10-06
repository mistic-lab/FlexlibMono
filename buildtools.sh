#!/bin/bash
if [ -d "tools" ]; then
  rm -rf tools
fi

mkdir tools

msbuild /p:Configuration=Release /t:Clean src/FlexlibMono.sln
msbuild /p:Configuration=Release /t:Build src/FlexlibMono.sln
cp src/HB9FXQ.Queryutil/bin/Release/*.dll tools/
cp src/HB9FXQ.Queryutil/bin/Release/*.exe tools/
