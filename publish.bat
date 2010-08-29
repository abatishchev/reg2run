set TMPBAT=publish.temp.bat
set CURRENT_PATH=%CD%
SubWCRev "%CD%" publish.in %TMPBAT%
call %TMPBAT%
del "%CURRENT_PATH%\%TMPBAT%"