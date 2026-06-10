#!/bin/bash

cd src/client || return
yarn install --immutable
sed -i 's/development/production/g' vite.config.ts
yarn build
sed -i 's/production/development/g' vite.config.ts
cd ../..
