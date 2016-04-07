#!/bin/bash
cddx.exe $*
if [ $? -eq 1 ]; then
	local cdscript="$HOME/.local/share/powertools/changedir.sh"
	if [ -f $cdscript ]; then
		. $cdscript
		rm $cdscript
	fi
fi
