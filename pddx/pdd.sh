#!/bin/bash
pddx $*
if [ $? -eq 1]; then
	local cdscript="$HOME/.local/share/powertools/popdir.sh"
	if [ -f $cdscript ]; then
		. $cdscript
		rm $cdscript
	fi
fi
