@echo off
if exist "%localappdata%\powertools\popdir.cmd"  (
	del /q "%localappdata%\powertools\popdir.cmd" > nul
)
pddx %*
if not errorlevel 1 (
	return
)
if not exist "%localappdata%\powertools\popdir.cmd"  (
	return
)
"%localappdata%\powertools\popdir.cmd"
