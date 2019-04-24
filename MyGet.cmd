pushd %~dp0

call buildscripts\build-native.cmd x64 Release skiptests
call buildscripts\build-managed.cmd x64 Release skiptests
call buildscripts\build-packages.cmd x64 Release skiptests

popd
