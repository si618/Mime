#!/usr/bin/env bash
# Compares the TargetFrameworks in Directory.Build.props with the .NET releases index and opens an
# issue for each framework the README "Requirements" rule says to add or drop:
#   - add every supported LTS
#   - drop a target once its release reaches end of support
#   - drop an STS target once a newer LTS is supported (it's replaced by that LTS)
# A new STS is only logged: targeting one depends on whether the library needs an API it adds.
# An issue isn't reopened once closed, so closing one without acting on it silences it.
#
# Environment:
#   DRY_RUN=1       print the issues instead of creating them
#   TODAY           override today's date (YYYY-MM-DD), for testing
#   RELEASES_INDEX  local file to read instead of downloading the index
set -euo pipefail

index_url=https://raw.githubusercontent.com/dotnet/core/main/release-notes/releases-index.json
today=${TODAY:-$(date -u +%F)}

if [ -n "${RELEASES_INDEX:-}" ]; then
  index=$(cat "$RELEASES_INDEX")
else
  index=$(curl -fsSL --retry 3 "$index_url")
fi

# Supported means GA (active or maintenance) and not past its end-of-support date, in case the index
# hasn't flipped the phase yet.
channels=$(jq -c --arg today "$today" '
  [.["releases-index"][]
   | {version: .["channel-version"], type: .["release-type"], phase: .["support-phase"], eol: .["eol-date"]}
   | .supported = ((.phase == "active" or .phase == "maintenance") and (.eol == null or .eol > $today))]
' <<<"$index")
if [ "$(jq 'map(select(.supported)) | length' <<<"$channels")" -eq 0 ]; then
  echo "::error::No supported channels read from the releases index"
  exit 1
fi

targets=$(sed -n 's:.*<TargetFrameworks>\(.*\)</TargetFrameworks>.*:\1:p' Directory.Build.props | tr ';' ' ')
if [ -z "$targets" ]; then
  echo "::error::No TargetFrameworks read from Directory.Build.props"
  exit 1
fi
echo "Targets: $targets"
echo "Supported: $(jq -r 'map(select(.supported) | "\(.version) (\(.type))") | join(", ")' <<<"$channels")"

channel() { jq -c --arg v "$1" 'map(select(.version == $v)) | first // empty' <<<"$channels"; }
targeted() { [[ " $targets " == *" net$1 "* ]]; }

# Opens an issue unless one (open or closed) already has <key> in its title, e.g. #62 for "drop net8.0".
open_issue() {
  local key=$1 title=$2 body=$3
  local existing
  existing=$(gh issue list --state all --search "\"$key\" in:title" --json number,title \
    --jq "map(select(.title | ascii_downcase | contains(\"$(tr '[:upper:]' '[:lower:]' <<<"$key")\"))) | .[0].number // empty")
  if [ -n "$existing" ]; then
    echo "Skipping \"$title\": #$existing already covers it"
  elif [ "${DRY_RUN:-}" = 1 ]; then
    printf 'Would open: %s\n\n%s\n\n' "$title" "$body"
  else
    gh issue create --title "$title" --body "$body"
  fi
}

# shellcheck disable=SC2016 # backticks are Markdown, not command substitution
checklist='- [ ] `Directory.Build.props`: update `TargetFrameworks`.
- [ ] Workflows: update the `dotnet-version` runtime lists in `ci.yml`, `pack.yml` and `build-libmagic.yml`, including the Alpine runtime unpack and the x86 step.
- [ ] Code: add or remove `#if NETx_0_OR_GREATER` branches, then rebuild with `-warnaserror`.
- [ ] Linux baseline: re-check the oldest glibc distro on the [supported-OS list](https://github.com/dotnet/core/tree/main/release-notes) of the oldest .NET still targeted. Update `GLIBC_BASELINE` in `.github/scripts/check-linux-deps.sh`, the `manylinux` build images, the `ubuntu-22.04` test runners and the Renovate rule that pins them if it changed.
- [ ] README "Requirements" and release steps: update the listed target frameworks.
- [ ] `renovate.json`: review the `dotnet-sdk` rule description.
- [ ] Release: dropping a target framework is a breaking change, so it needs a new major version. Adding one is a minor.'

mapfile -t lts < <(jq -r 'map(select(.supported and .type == "lts")) | .[].version' <<<"$channels")

for version in "${lts[@]}"; do
  if ! targeted "$version"; then
    open_issue "add net$version" "Add net$version: .NET ${version%.0} LTS is released" \
"The [.NET releases index]($index_url) lists .NET $version as a supported LTS, and the README \"Requirements\" rule targets every supported LTS. Current targets: \`$targets\`.

$checklist"
  fi
done

for target in $targets; do
  version=${target#net}
  info=$(channel "$version")
  if [ -z "$info" ]; then
    echo "::warning::$target isn't in the releases index"
    continue
  fi
  if [ "$(jq -r .supported <<<"$info")" != true ]; then
    eol=$(jq -r '.eol // "an unknown date"' <<<"$info")
    open_issue "drop $target" "Drop $target: .NET ${version%.0} reached end of support on $eol" \
"The [.NET releases index]($index_url) shows .NET $version reached end of support on $eol, and the README \"Requirements\" rule drops a target once it does. Current targets: \`$targets\`.

$checklist"
  elif [ "$(jq -r .type <<<"$info")" = sts ]; then
    newer=$(jq -r --arg v "$version" 'map(select(.supported and .type == "lts" and (.version | tonumber) > ($v | tonumber))) | .[0].version // empty' <<<"$channels")
    if [ -n "$newer" ]; then
      open_issue "drop $target" "Drop $target: replaced by .NET ${newer%.0} LTS" \
"$target is an STS target, and .NET $newer LTS is now supported. The README \"Requirements\" rule replaces an STS target with the next LTS once that ships, even though the STS is still supported. Current targets: \`$targets\`.

$checklist"
    fi
  fi
done

jq -r 'map(select(.supported and .type == "sts")) | .[].version' <<<"$channels" | while read -r version; do
  if ! targeted "$version"; then
    echo "::notice::.NET $version (STS) is supported but not targeted. Add net$version only if the library needs an API it adds."
  fi
done
