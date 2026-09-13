#!/bin/zsh
cd "${0:A:h}/../backend/multiplayer" || exit 1
export PATH="/opt/homebrew/bin:/usr/local/bin:$PATH"
if [[ ! -d node_modules/ws ]]; then
  npm ci || exit 1
fi
exec node server.mjs
