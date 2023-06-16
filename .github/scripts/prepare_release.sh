#!/bin/bash

# Fail on first error.
set -e

releaseLevel="$1"

oldVersion="$(node -pe 'require("./package.json").version')"
npx commit-and-tag-version --release-as "$releaseLevel" --skip.commit=true --skip.changelog=true --skip.tag=true
newVersion="$(node -pe 'require("./package.json").version')"

sed -i -e "s/<VersionPrefix>$oldVersion<\/VersionPrefix>/<VersionPrefix>$newVersion<\/VersionPrefix>/" ./packages/*/src/*.csproj

npx conventional-changelog-cli -p angular -i CHANGELOG.md -s
