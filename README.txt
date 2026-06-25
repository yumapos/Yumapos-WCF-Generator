1. For debug generation - run in DEBUG WCF-Generator, it run with default configuration (edit App.config)

2. Build explicitly with buildGenerator.bat (Windows) or buildGenerator.sh (Linux).
   Every Debug build (including IDE builds) automatically publishes portable binaries
   into _WCF-Generator-binaries without rebuilding during publish.

3. Run generation.bat or generation.sh to use the published binaries without rebuilding.
   Both use dotnet WCF-Generator.dll with WCF-Generator.exe.config.
   Keep the complete published directory, including Roslyn BuildHost dependencies.
