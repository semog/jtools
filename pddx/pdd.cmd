@echo off
pddx %*
if errorlevel 1 (
	if exist "%localappdata%\powertools\popdir.cmd"  (
		"%localappdata%\powertools\popdir.cmd"
		del /q "%localappdata%\powertools\popdir.cmd" > nul
	)
)
