#!/bin/bash

echo "::: Unity License Activation :::"
/opt/unity/Editor/Unity -logFile - -batchmode -nographics -quit -manualLicenseFile /github/workspace/license.ulf

echo "::: Unity Solution Generation :::"
cd .project
/opt/unity/Editor/Unity -logFile - -batchmode -nographics -quit -executeMethod Solution.Sync

echo "::: DotNet Installation :::"
cd ..
wget https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
apt-get update
apt-get install -y dotnet-sdk-7.0
dotnet tool update -g docfx --version 2.67.5

echo "::: Documentation Generation :::"
~/.dotnet/tools/docfx .docfx/docfx.json
