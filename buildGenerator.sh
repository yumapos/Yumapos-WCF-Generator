#!/usr/bin/env bash
set -euo pipefail

script_dir="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"

dotnet build "$script_dir/WCF-Generator/WCF-Generator.csproj" -c Debug
