rem Copyright (C) 2007-2008 Alexander M. Batishchev aka Godfather (abatishchev at gmail.com)

set PATH=%PATH%;%PROGRAMFILES%\WinRar;%PROGRAMFILES%\PuTTY

set CMD=A -afzip -o+ -s -ibck -t

set FILES=CHANGELOG.txt ConsoleStub.cs Core.cs Errors.cs ImportObject.cs LICENSE.txt ManualConsole.cs Reg2Run.csproj Reg2Run.sln Settings.cs Properties\AssemblyInfo.cs
set BINARY=bin\Release\reg2run.exe

set BATCH=batch.txt
del %BATCH%
echo cd uploads >> %BATCH%

set NAME=reg2run-2.0.2-source.zip
call winrar.exe %CMD% %NAME% %FILES%
echo put %NAME% >> %BATCH%

set NAME=reg2run-2.0.2-binary.zip
call winrar.exe -ep %CMD% %NAME% %BINARY%
echo put %NAME% >> %BATCH%

call psftp.exe -b %BATCH% abatishchev@web.sourceforge.net

del %BATCH%

set DIST=..\release

mkdir %DIST%
move /y reg2run-2.0.2-source.zip %DIST%
move /y reg2run-2.0.2-binary.zip %DIST%
