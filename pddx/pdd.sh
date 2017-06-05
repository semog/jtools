#!/bin/bash
function changedir()
{
	local cdscript="$HOME/.local/share/powertools/popdir.sh"
	if [ -f $cdscript ]; then
		. $cdscript
		rm $cdscript
	fi
}

mono `which pddx.exe` $*
if [ $? -eq 1 ]; then
	changedir
fi
