rem ugly build script
@echo off

if "%2" == "run" goto run
if "%1" == "third" goto third
if "%1" == "core" goto core
if "%1" == "text" goto text
if "%1" == "ui" goto ui
if "%1" == "fb" goto fb  
echo Error: not enough arguments.
echo Usage:
echo   build target [action]
echo   target: "third | core | text | ui | fb"
echo   action: "build (default), clean, init, run"
goto end

:third
cd third-party\symbian\expat-2.0.1\symbian
bldmake bldfiles
call abld clean
call abld build winscw udeb
cd ..\..\..\.. 


cd third-party\symbian\SQLiteS60\SQLite60\group
bldmake bldfiles
call abld clean
call abld build winscw udeb
cd ..\..\..\..\..
goto end

:core
cd zlibrary\core
if "%2" == "init" bldmake bldfiles
if "%2" == "clean" abld clean
call abld build winscw udeb
cd ..\..
goto end

:text
cd zlibrary\text
if "%2" == "init" bldmake bldfiles
if "%2" == "clean" abld clean
call abld build winscw udeb
cd ..\..
goto end

:ui
cd zlibrary\ui
if "%2" == "init" bldmake bldfiles
if "%2" == "clean" abld clean
call abld build winscw udeb
cd ..\..     
goto end

:fb
cd fbreader
if "%2" == "init" bldmake bldfiles
if "%2" == "clean" abld clean
call abld build winscw udeb
cd ..
goto end

:run
C:\S60\devices\S60_5th_Edition_SDK_v1.0\epoc32\release\winscw\udeb\fbreader
goto end

:end
 