# PREREQUISITES
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
RUN apt update
RUN apt install openssh-server nodejs npm -y

# CLONE & SETUP 
COPY . . 
RUN mkdir /data/
RUN mkdir /data/hamann/
RUN mkdir /data/xml/
RUN git clone https://github.com/Theodor-Springmann-Stiftung/hamann-xml.git /data/xml/

# COMPILE & PUBLISH
WORKDIR HaWeb/
RUN dotnet restore 
RUN npm install
RUN npm run css_build
RUN dotnet publish --no-restore -o /app

# RUN
WORKDIR /app
RUN DOTNET_ENVIRONMENT=Docker dotnet HaWeb.dll
EXPOSE 5000
