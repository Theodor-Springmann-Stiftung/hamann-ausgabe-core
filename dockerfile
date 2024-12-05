# PREREQUISITES
FROM ubuntu

COPY . /source

RUN apt update
RUN apt upgrade -y
RUN apt install git wget curl libicu-dev -y

RUN wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
RUN chmod +x dotnet-install.sh
RUN ./dotnet-install.sh --channel 6.0
ENV DOTNET_DIR=/root/.dotnet

RUN wget https://raw.githubusercontent.com/nvm-sh/nvm/v0.40.0/install.sh -O install.sh
RUN chmod +x install.sh
RUN ./install.sh

ENV NODE_VERSION=22.12.0
ENV NVM_DIR=/root/.nvm

ENV PATH="/root/.dotnet:${PATH}"
ENV PATH="/root/.nvm:${PATH}"

RUN . "$NVM_DIR/nvm.sh" && nvm install ${NODE_VERSION}
RUN . "$NVM_DIR/nvm.sh" && nvm use v${NODE_VERSION}
RUN . "$NVM_DIR/nvm.sh" && nvm alias default v${NODE_VERSION}
ENV PATH="/root/.nvm/versions/node/v${NODE_VERSION}/bin/:${PATH}"

WORKDIR /source/HaWeb
RUN npm install
RUN "$DOTNET_DIR/dotnet" restore
RUN "$DOTNET_DIR/dotnet" publish -c Release -o /app

WORKDIR /app
CMD ["dotnet", "HaWeb.dll"]

# RUN apt update
# RUN apt install openssh-server nodejs npm -y
#
# # CLONE & SETUP 
# COPY . . 
# RUN mkdir /data/
# RUN mkdir /data/hamann/
# RUN mkdir /data/xml/
# RUN git clone https://github.com/Theodor-Springmann-Stiftung/hamann-xml.git /data/xml/
#
# # COMPILE & PUBLISH
# WORKDIR HaWeb/
# RUN dotnet restore 
# RUN npm install
# RUN npm run css_build
# RUN dotnet publish --no-restore -o /app
#
# # RUN
# WORKDIR /app
# RUN DOTNET_ENVIRONMENT=Docker dotnet HaWeb.dll
# EXPOSE 5000
