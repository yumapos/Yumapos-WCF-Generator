#!/usr/bin/env bash

SCRIPT_DIR=$( cd -- "$( dirname -- "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )

dotnet "$SCRIPT_DIR/_WCF-Generator-binaries/WCF-Generator.dll" "$SCRIPT_DIR/WCF-Generator.exe.config"
