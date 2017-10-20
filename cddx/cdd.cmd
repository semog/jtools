@echo off
if exist "%localappdata%\powertools\changedir.cmd" (
	del /q "%localappdata%\powertools\changedir.cmd" > nul
)
cddx %*
if not errorlevel 1 (
	return
)
if not exist "%localappdata%\powertools\changedir.cmd" (
	return
)
"%localappdata%\powertools\changedir.cmd"
