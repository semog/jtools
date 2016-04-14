@echo off
setlocal

set buildconfig=%1
if "%buildconfig%"=="" (
	set buildconfig=release
)

rem Publish CDD files
call :pubfile cddx\bin\%buildconfig%\cddx.exe
call :pubfile cddx\cdd.cmd
call :pubfile cddx\cdd.ps1

rem Publish PDD files
call :pubfile pddx\bin\%buildconfig%\pddx.exe
call :pubfile pddx\pdd.cmd
call :pubfile pddx\pdd.ps1

rem Publish SDD files
call :pubfile sdd\bin\%buildconfig%\sdd.exe

goto :eof

:pubfile
if not exist "%1" (
	echo ERROR: File %1 does not exist.
	exit
)

echo Copying %1...
copy %1 C:\Utils > nul
goto :eof
