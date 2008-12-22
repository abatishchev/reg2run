#!/bin/bash
# Copyright (C) 2007-2008 Alexander M. Batishchev aka Godfather (abatishchev at gmail.com)

source='CHANGELOG.txt ConsoleStub.cs Core.cs Errors.cs ImportObject.cs LICENSE.txt ManualConsole.cs Reg2Run.csproj Reg2Run.sln Settings.cs Properties/AssemblyInfo.cs'

release='bin/Release'
binary='reg2run.exe'

batch='batch'
rm -f $batch
echo 'cd uploads' >> $batch

name='reg2run-2.0.2-source.tar.bz'
tar cfjv $name $source
echo 'put '$name >> $batch

name='reg2run-2.0.2-binary.tar.bz'
tar cfjv $name -C $release $binary
echo 'put '$name >> $batch

sftp -b $batch abatishchev@frs.sourceforge.net

rm $batch

dist='../release/2.0.2'

mkdir -p $dist
mv -f reg2run-2.0.2-source.tar.bz $dist
mv -f reg2run-2.0.2-binary.tar.bz $dist
