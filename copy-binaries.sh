for f in "_WCF-Generator-binaries/*.*" ;
    do
    echo $f
    if [[ $f != "_WCF-Generator-binaries/WCF-Generator.exe.config" ]];
        then rm -f $f
    fi
done

cp WCF-Generator/bin/Debug/net8.0/*.dll _WCF-Generator-binaries
cp WCF-Generator/bin/Debug/net8.0/*.json _WCF-Generator-binaries
cp WCF-Generator/bin/Debug/net8.0/WCF-Generator.exe _WCF-Generator-binaries
