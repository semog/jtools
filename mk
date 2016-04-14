#!/bin/bash

buildaction=Build
buildconfig=Release

while [[ $# > 0 ]]
do
key="$1"

case $key in
    [Dd]ebug)
    buildconfig=Debug
    ;;
    [Rr]elease)
    buildconfig=Release
    ;;
    [Cc]lean)
    buildaction=Clean
    ;;
    [Bb]uild)
    buildaction=Build
    ;;
    *)
            # unknown option
    ;;
esac
shift
done

mdtool build -t:$buildaction -c:$buildconfig jtools.sln

