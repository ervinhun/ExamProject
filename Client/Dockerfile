FROM node:22-alpine AS build
WORKDIR /app
COPY package.json ./
RUN npm install
COPY . .
# Use empty string for production - all API calls will be relative paths handled by nginx
ENV VITE_API_HOST=""
ENV VITE_PLAYER="0"
ENV VITE_ADMIN="1"
ENV VITE_SUPERADMIN="2"
ENV VITE_ENVIRONMENT="production"
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf.template /etc/nginx/conf.d/default.conf.template
COPY docker-entrypoint.sh /docker-entrypoint.sh

RUN chmod +x /docker-entrypoint.sh

# Set default VITE_API_HOST if not provided
ENV VITE_API_HOST=http://localhost:5152

EXPOSE 8080
CMD ["/docker-entrypoint.sh"]