@echo off
cddx %*
if errorlevel 1 (
	if exist "%localappdata%\powertools\changedir.cmd" (
		"%localappdata%\powertools\changedir.cmd"
		del /q "%localappdata%\powertools\changedir.cmd" > nul
	)
)
