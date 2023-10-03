#!/bin/bash

# Fail on first error.
set -e

releaseLevel="$1"

oldVersion="$(node -pe 'require("./package.json").version')"

# If no release level is specified, let standard-version handle versioning
if [ -z "$releaseLevel" ] 
then
  npx commit-and-tag-version --skip.commit=true --skip.changelog=true --skip.tag=true
else
  npx commit-and-tag-version --release-as "$releaseLevel" --skip.commit=true --skip.changelog=true --skip.tag=true
fi

newVersion="$(node -pe 'require("./package.json").version')"

sed -i -e "s/<VersionPrefix>$oldVersion<\/VersionPrefix>/<VersionPrefix>$newVersion<\/VersionPrefix>/" ./packages/*/src/*.csproj

npx conventional-changelog-cli -p angular -i CHANGELOG.md -s
