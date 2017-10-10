#!/bin/bash
ScriptDir=$(dirname "$0")
ToolDir="$ScriptDir/tools"
SourceDir="$ScriptDir/src"
OutputDir="$SourceDir/HB9FXQ.Queryutil/bin/Release"

if [ -d $ToolDir ]; then
  rm -rf $ToolDir
fi

mkdir $ToolDir


msbuild /p:Configuration=Release /t:Clean $SourceDir/FlexlibMono.sln
msbuild /p:Configuration=Release /t:Build $SourceDir/FlexlibMono.sln
cp $OutputDir/*.dll $ToolDir
cp $OutputDir/*.exe $ToolDir

GrFlexLibDir="$ScriptDir/../gr-flex/python/FlexlibMono"
cp $OutputDir/*.dll $GrFlexLibDir
