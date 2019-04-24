pushd %~dp0

call build.cmd x64 Release skiptests
call buildscripts\build-packages.cmd x64 Release skiptests

popd
