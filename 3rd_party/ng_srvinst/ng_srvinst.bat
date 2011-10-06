@echo off
if "%1"=="remove" goto RemoveSrv
ng_srv.exe install
goto Ex1
:RemoveSrv
ng_srv.exe uninstall
:Ex1
