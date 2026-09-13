FROM node:24-alpine
WORKDIR /app
COPY backend/multiplayer/package*.json ./
RUN npm ci --omit=dev
COPY backend/multiplayer/server.mjs ./
COPY Builds/Web/ ./web/
ENV PORT=8090 WEB_ROOT=/app/web
USER node
EXPOSE 8090
CMD ["node", "server.mjs"]
