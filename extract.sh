#!/bin/bash

if [ -d "GameData/root" ] ; then
    rm -rf GameData/root
fi

dotnet run -p Bokuract/Bokuract/Bokuract.csproj -- $PWD/GameData
