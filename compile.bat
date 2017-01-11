cd /d %~dp0
call vsvar32.bat
devenv ./RemoteTestHarness.sln /rebuild debug
copy /y VendingMachine\bin\Debug\VendingMachine.dll .\Client\ClientStorageLocation
copy /y TestDriverVendingMachine2\bin\Debug\TestDriverVendingMachine2.dll .\Client\ClientStorageLocation
copy /y TestDriverVendingMachine3\bin\Debug\TestDriverVendingMachine3.dll .\Client\ClientStorageLocation
copy /y TestDriverVendingMachine\bin\Debug\TestDriverVendingMachine.dll .\Client\ClientStorageLocation
copy /y TestDriverVendingMachine4\bin\Debug\TestDriverVendingMachine4.dll .\Client\ClientStorageLocation

