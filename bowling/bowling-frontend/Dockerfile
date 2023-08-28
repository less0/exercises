FROM node:latest as buildenv
WORKDIR /app
COPY . .
RUN npm install 
RUN npm run build --prod

FROM nginx:alpine
COPY ./nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=buildenv /app/dist/bowling-frontend /usr/share/nginx/html

