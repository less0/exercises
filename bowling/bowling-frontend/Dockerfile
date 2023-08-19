FROM node:latest as buildenv
WORKDIR /app
COPY . .
RUN npm install 
RUN npm run build --prod

FROM nginx:alpine
COPY --from=buildenv /app/dist/bowling-frontend /usr/share/nginx/html

