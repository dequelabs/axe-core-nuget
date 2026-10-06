#!/bin/bash

# Fail on first error.
set -e

releaseLevel="$1"

pnpm install --frozen-lockfile

oldVersion="$(node -pe 'require("./package.json").version')"

# If no release level is specified, let commit-and-tag-version handle versioning
if [ -z "$releaseLevel" ] 
then
  pnpm exec commit-and-tag-version --skip.commit=true --skip.changelog=true --skip.tag=true
else
  pnpm exec commit-and-tag-version --release-as "$releaseLevel" --skip.commit=true --skip.changelog=true --skip.tag=true
fi

newVersion="$(node -pe 'require("./package.json").version')"

sed -i -e "s/<VersionPrefix>$oldVersion<\/VersionPrefix>/<VersionPrefix>$newVersion<\/VersionPrefix>/" ./packages/*/src/*.csproj

pnpm exec conventional-changelog -p angular -i CHANGELOG.md -s
