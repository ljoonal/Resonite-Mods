#!/bin/sh
set -e

# Get current version
VERSION=`date --iso-8601`

FILES_ARG=`find bin/Release/ -type f | awk 1 ORS=' -a '`
FILES_ARG="-a ${FILES_ARG%????}"

# Create a gitea release draft
tea release create --draft --target main \
  --tag v$VERSION --title v$VERSION \
  $FILES_ARG
