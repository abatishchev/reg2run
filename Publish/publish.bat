SET TMPBAT=publish.temp.bat
SubWCRev "%CD%" publish.in %TMPBAT%
call %TMPBAT%
del %TMPBAT%