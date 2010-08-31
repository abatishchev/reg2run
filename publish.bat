rem Copyright (C) 2007-2010 Alexander M. Batishchev (abatishchev at gmail.com)

set TMPBAT=publish.temp.bat
set CURRENT_PATH=%CD%
SubWCRev "%CD%" publish.in %TMPBAT%
call %TMPBAT%
del "%CURRENT_PATH%\%TMPBAT%"
