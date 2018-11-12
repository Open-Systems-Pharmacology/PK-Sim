@echo off

sqldiff.exe --primarykey PKSimDB.sqlite ../PKSimDB.sqlite > PKSimDB_diff.txt

pause
