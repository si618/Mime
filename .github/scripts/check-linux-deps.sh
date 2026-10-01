#!/usr/bin/env bash
# Fails if a Linux <lib> needs a shared library outside <allowed...>, or a glibc symbol version newer
# than <max-glibc>. Pass "-" as <max-glibc> for musl builds, which don't version their symbols.
# readelf reads any ELF architecture, so the arm64 builds can be checked on an x64 runner.
# Usage: check-linux-deps.sh <lib> <max-glibc> <allowed...>
set -euo pipefail

lib=$1 max_glibc=$2
shift 2
allowed=("$@")
status=0

needed=$(readelf -d "$lib" | sed -n 's/.*(NEEDED).*\[\(.*\)\]/\1/p')
if [ -z "$needed" ]; then
  echo "::error::No NEEDED entries read from $lib"
  exit 1
fi
echo "$lib needs: $(tr '\n' ' ' <<<"$needed")"

for name in $needed; do
  if [[ ! " ${allowed[*]} " == *" $name "* ]]; then
    echo "::error::$lib needs $name, which isn't in the allowed list: ${allowed[*]}"
    status=1
  fi
done

if [ "$max_glibc" != "-" ]; then
  newest=$(readelf -V -W "$lib" | grep -o 'GLIBC_[0-9.]*' | sed 's/GLIBC_//' | sort -uV | tail -1)
  echo "$lib newest glibc symbol version: ${newest:-none}"
  if [ -n "$newest" ] && [ "$(printf '%s\n%s\n' "$max_glibc" "$newest" | sort -V | tail -1)" != "$max_glibc" ]; then
    echo "::error::$lib needs GLIBC_$newest, newer than the GLIBC_$max_glibc baseline. Symbols above it:"
    readelf --dyn-syms -W "$lib" | grep -o '[^ ]*@GLIBC_[0-9.]*' | sort -u | while read -r sym; do
      version=${sym##*@GLIBC_}
      if [ "$(printf '%s\n%s\n' "$max_glibc" "$version" | sort -V | tail -1)" != "$max_glibc" ]; then
        echo "  $sym"
      fi
    done
    status=1
  fi
fi

exit $status
