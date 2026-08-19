# Frontend
FROM node:20-alpine AS frontend-builder
WORKDIR /src/frontend

ARG VITE_API_URL
ENV VITE_API_URL=${VITE_API_URL}

COPY frontend/package*.json ./
RUN npm ci

COPY frontend/ .
RUN npm run build

# Backend
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS backend-builder
WORKDIR /backend

ARG GITHUB_USERNAME
ARG GITHUB_PASSWORD

ENV GITHUB_USERNAME=${GITHUB_USERNAME}
ENV GITHUB_PASSWORD=${GITHUB_PASSWORD}

COPY backend/ .

RUN dotnet restore src/MealPlanner.Web/MealPlanner.Web.csproj

COPY --from=frontend-builder /src/frontend/dist ./src/MealPlanner.Web/wwwroot

RUN dotnet publish src/MealPlanner.Web/MealPlanner.Web.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=backend-builder /app/publish .

EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "MealPlanner.Web.dll"]
