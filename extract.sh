#!/bin/bash

if [ -d "data/root" ] ; then
    rm -rf data/root
fi

mono Bokuract/Bokuract/bin/Debug/Bokuract.exe data/
