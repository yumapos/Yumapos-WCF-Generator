FOR %%f IN (_WCF-Generator-binaries\*.*) DO IF NOT %%f==_WCF-Generator-binaries\WCF-Generator.exe.config DEL /Q %%f
for /d %%x in (_WCF-Generator-binaries\*) do @rd /s /q "%%x"
xcopy WCF-Generator\bin\Debug\*.dll _WCF-Generator-binaries /s/Y
copy WCF-Generator\bin\Debug\WCF-Generator.exe _WCF-Generator-binaries
