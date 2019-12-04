FROM node:alpine AS build
WORKDIR /build
COPY package.json .
RUN npm install
COPY . .
RUN npm run ng build web -- --output-path=dist/web

FROM nginx:alpine
RUN rm -rf /usr/share/nginx/html/*
COPY nginx/default.conf /etc/nginx/conf.d/
COPY --from=build /build/dist/web /usr/share/nginx/html
CMD ["nginx", "-g", "daemon off;"]