#!/bin/sh
set -e

# Get current version
VERSION=`date --iso-8601`

cd bin/Release

# Create hashes of all the files (except the hash files themselves)
rm -f sha256sums.txt sha512sums.txt blake3sums.txt
SHA256SUMS=`sha256sum *`
SHA512SUMS=`sha512sum *`
BLAKE3SUMS=`b3sum *`
echo "$SHA256SUMS" > sha256sums.txt
echo "$SHA512SUMS" > sha512sums.txt
echo "$BLAKE3SUMS" > blake3sums.txt

# Create args to be passed to tea for uploading all the files
FILES_ARG=`find ./ -type f | awk 1 ORS=' -a '`
FILES_ARG="-a ${FILES_ARG%????}"

# Create a gitea release draft
tea release create --draft --target main \
  --tag v$VERSION --title v$VERSION \
  $FILES_ARG

cd ../..
