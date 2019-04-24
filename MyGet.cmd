pushd %~dp0

choco install cmake --installargs 'ADD_CMAKE_TO_PATH=User ALLUSERS=0'

call buildscripts\build-native.cmd x64 Release skiptests
call buildscripts\build-managed.cmd x64 Release skiptests
call buildscripts\build-packages.cmd x64 Release skiptests

popd
