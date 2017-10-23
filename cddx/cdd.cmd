@echo off
if exist "%localappdata%\powertools\changedir.cmd" (
	del /q "%localappdata%\powertools\changedir.cmd" > nul
)
cddx %*
if not errorlevel 1 (
	goto :eof
)
if not exist "%localappdata%\powertools\changedir.cmd" (
	goto :eof
)
"%localappdata%\powertools\changedir.cmd"
