SET TMPBAT=publish.temp.bat
SET CURRENT_PATH=%CD%
SubWCRev "%CD%" publish.in %TMPBAT%
call %TMPBAT%
del "%CURRENT_PATH%\%TMPBAT%"