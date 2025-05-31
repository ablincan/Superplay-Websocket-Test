FROM node:alpine

WORKDIR /usr/src/app

COPY . .

RUN npm install -g @angular/cli

RUN npm install

RUN npm run docker

CMD ["ng", "serve", "--host", "0.0.0.0"]
