# Build stage
FROM node:22-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm ci

COPY . .
RUN npm run build:prod

# The output will be in dist/price-tracker/browser
# In deployment, we copy these files to the nginx container
