# PREREQUISITES
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
RUN apt update
RUN apt install openssh-server nodejs npm -y

# CLONE & SETUP 
RUN git clone https://github.com/Theodor-Springmann-Stiftung/hamann-ausgabe-core.git
RUN mkdir /data/
RUN mkdir /data/hamann/
RUN mkdir /data/xml/
RUN git clone https://github.com/Theodor-Springmann-Stiftung/hamann-xml.git /data/xml/

# COMPILE & PUBLISH
WORKDIR hamann-ausgabe-core/HaWeb/
RUN dotnet restore 
RUN npm install
RUN npm run css_build
RUN dotnet publish --no-restore -o /app

# RUN
WORKDIR /app
RUN export DOTNET_ENVIRONMENT="Docker"
RUN dotnet HaWeb.dll
EXPOSE 5000
