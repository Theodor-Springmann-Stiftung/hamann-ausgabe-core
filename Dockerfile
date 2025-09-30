# Build frontend assets
FROM node:18 AS frontend
WORKDIR /app/HaWeb
COPY HaWeb/package*.json ./
RUN npm install
COPY HaWeb/ ./
RUN npm run build

# Build .NET application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy project files and restore dependencies
COPY HaDocumentV6/HaDocumentV6.csproj ./HaDocumentV6/
COPY HaXMLReaderV6/HaXMLReaderV6.csproj ./HaXMLReaderV6/
COPY HaWeb/HaWeb.csproj ./HaWeb/
RUN dotnet restore HaWeb/HaWeb.csproj

# Copy all source files
COPY HaDocumentV6/ ./HaDocumentV6/
COPY HaXMLReaderV6/ ./HaXMLReaderV6/
COPY HaWeb/ ./HaWeb/

# Copy built frontend assets (overwrites the source wwwroot/dist)
COPY --from=frontend /app/HaWeb/wwwroot/dist/ ./HaWeb/wwwroot/dist/

# Build application
WORKDIR /app/HaWeb
RUN dotnet publish -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app

# Install git for LibGit2Sharp
RUN apt-get update && apt-get install -y git && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

# Create data directory
RUN mkdir -p /app/data

# Expose ports for HTTP, HTTPS
EXPOSE 5000
EXPOSE 5001

ENV ASPNETCORE_URLS="http://+:5000;https://+:5001"
ENV DOTNET_ENVIRONMENT="Production"
ENV FileStoragePath="/app/data"

CMD ["dotnet", "HaWeb.dll"]