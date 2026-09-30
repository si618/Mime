#!/usr/bin/env bash
# Copies every DLL that <dll> imports, directly or transitively, from a MinGW bin directory into
# <out-dir>. System DLLs (KERNEL32, msvcrt, api-ms-win-*) aren't in <bin-dir>, so they're skipped.
# Usage: collect-mingw-dlls.sh <objdump> <dll> <bin-dir> <out-dir>
set -euo pipefail

objdump=$1 dll=$2 bin=$3 out=$4

imports() {
  "$objdump" -p "$1" | awk '/DLL Name:/ { print $3 }'
}

# Every PE DLL imports something; an empty list means objdump's output wasn't understood.
if [ -z "$(imports "$dll")" ]; then
  echo "::error::No imports read from $dll with $objdump"
  exit 1
fi

queue=("$dll")
while [ ${#queue[@]} -gt 0 ]; do
  current=${queue[0]}
  queue=("${queue[@]:1}")
  while read -r name; do
    if [ -f "$bin/$name" ] && [ ! -f "$out/$name" ]; then
      cp "$bin/$name" "$out/"
      echo "$(basename "$current") -> $name"
      queue+=("$out/$name")
    fi
  done < <(imports "$current")
done
