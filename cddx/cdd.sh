#!/bin/bash
function changedir()
{
	local cdscript="$HOME/.local/share/powertools/changedir.sh"
	if [ -f $cdscript ]; then
		. $cdscript
		rm $cdscript
	fi
}

cddx.exe $*
if [ $? -eq 1 ]; then
	changedir
fi
