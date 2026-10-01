#!/usr/bin/env bash
# Fails if the Linux libmagic for <rid> needs a shared library outside that runtime's allowed list, or
# a glibc symbol version newer than GLIBC_BASELINE. musl doesn't version its symbols, so only the
# library list is checked there. readelf reads any ELF architecture, so the arm64 builds can be
# checked on an x64 runner.
# Usage: check-linux-deps.sh <rid> <lib>
set -euo pipefail

# The oldest glibc of any distro on the supported-OS list of the oldest .NET release we target
# (RHEL 8 for both .NET 8 and .NET 10). The glibc builds use a manylinux_2_28 container to match.
# Raise it only when that distro drops off the list.
GLIBC_BASELINE=2.28

rid=$1 lib=$2
case $rid in
  linux-x64) max_glibc=$GLIBC_BASELINE allowed=(libc.so.6 libm.so.6 libz.so.1) ;;
  linux-arm64) max_glibc=$GLIBC_BASELINE allowed=(libc.so.6 libm.so.6 libz.so.1 ld-linux-aarch64.so.1) ;;
  linux-musl-x64) max_glibc='' allowed=(libc.musl-x86_64.so.1 libz.so.1) ;;
  linux-musl-arm64) max_glibc='' allowed=(libc.musl-aarch64.so.1 libz.so.1) ;;
  *)
    echo "::error::Unknown runtime $rid"
    exit 1
    ;;
esac
status=0

needed=$(readelf -d "$lib" | sed -n 's/.*(NEEDED).*\[\(.*\)\]/\1/p')
if [ -z "$needed" ]; then
  echo "::error::No NEEDED entries read from $lib"
  exit 1
fi
echo "$lib needs: $(tr '\n' ' ' <<<"$needed")"

for name in $needed; do
  if [[ ! " ${allowed[*]} " == *" $name "* ]]; then
    echo "::error::$lib needs $name, which isn't in the $rid allowed list: ${allowed[*]}"
    status=1
  fi
done

if [ -n "$max_glibc" ]; then
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
