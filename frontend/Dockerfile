FROM node:22-bookworm

WORKDIR /app

COPY package.json package-lock.json ./
RUN npm ci && cp package-lock.json node_modules/.install-stamp

COPY docker-entrypoint.sh /usr/local/bin/docker-entrypoint.sh
RUN chmod +x /usr/local/bin/docker-entrypoint.sh

ENV API_PROXY_TARGET=http://api:21000

EXPOSE 20000

ENTRYPOINT ["docker-entrypoint.sh"]
