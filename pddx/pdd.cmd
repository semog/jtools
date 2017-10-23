@echo off
if exist "%localappdata%\powertools\popdir.cmd"  (
	del /q "%localappdata%\powertools\popdir.cmd" > nul
)
pddx %*
if not errorlevel 1 (
	goto :eof
)
if not exist "%localappdata%\powertools\popdir.cmd"  (
	goto :eof
)
"%localappdata%\powertools\popdir.cmd"
