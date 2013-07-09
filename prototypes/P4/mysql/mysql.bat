@echo off
start mysql\bin\mysqld.exe
echo MySQL server is started
echo Please type close to shutdown the MySQL server
:input
set INPUT=
set /P INPUT=Type input: %=%
if "%INPUT%"=="close" start mysql\bin\mysqladmin.exe -u root shutdown
if "%INPUT%"=="close" exit
goto input