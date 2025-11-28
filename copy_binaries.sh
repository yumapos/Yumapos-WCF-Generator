SCRIPT_DIR=$( cd -- "$( dirname -- "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )

for f in "$SCRIPT_DIR/_WCF-Generator-binaries/*.*" ;
    do
    if [[ $f != "$SCRIPT_DIR/_WCF-Generator-binaries/WCF-Generator.exe.config" ]];
        then rm -f $f
    fi
done

cp $SCRIPT_DIR/WCF-Generator/bin/Debug/net8.0/win-x64/*.dll $SCRIPT_DIR/_WCF-Generator-binaries
cp $SCRIPT_DIR/WCF-Generator/bin/Debug/net8.0/win-x64/*.json $SCRIPT_DIR/_WCF-Generator-binaries
cp $SCRIPT_DIR/WCF-Generator/bin/Debug/net8.0/win-x64/WCF-Generator.exe $SCRIPT_DIR/_WCF-Generator-binaries
