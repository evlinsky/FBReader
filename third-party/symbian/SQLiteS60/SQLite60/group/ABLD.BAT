@ECHO OFF

REM Bldmake-generated batch file - ABLD.BAT
REM ** DO NOT EDIT **

perl -S ABLD.PL "\project\src\fbreader-symbian\third-party\symbian\SQLiteS60\SQLite60\group\\" %1 %2 %3 %4 %5 %6 %7 %8 %9
if errorlevel==1 goto CheckPerl
goto End

:CheckPerl
perl -v >NUL
if errorlevel==1 echo Is Perl, version 5.003_07 or later, installed?
goto End

:End
