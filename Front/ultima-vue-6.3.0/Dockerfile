# Builder
FROM node:lts-alpine AS build-stage

WORKDIR /app
COPY package*.json .
RUN npm install

COPY . .
RUN npm run build

# production stage
FROM nginx:stable-alpine AS production-stage

COPY --from=build-stage /app/dist /usr/share/nginx/html

# Replace Nginx config
RUN rm /etc/nginx/conf.d/default.conf
COPY <<EOF /etc/nginx/conf.d/default.conf
server {
    listen 80;
    server_name localhost;
    
    location / {
        root /usr/share/nginx/html;
        index index.html;
        try_files \$uri \$uri/ /index.html;
    }

    location ~* \.(js|jpg|png|css)$ {
        root /usr/share/nginx/html;
    }
}
EOF

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]