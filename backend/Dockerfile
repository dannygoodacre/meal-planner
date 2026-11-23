# Build from the repo root: docker build -f backend/Dockerfile --build-arg VERSION=x.y.z .

FROM node:24-alpine AS frontend-builder
WORKDIR /src/frontend

COPY frontend/package*.json ./
RUN npm ci

COPY frontend/ .

ARG VERSION=0.0.0-dev
ENV VITE_APP_VERSION=$VERSION
RUN npm run build

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS backend-builder
WORKDIR /src

COPY backend/src/MealPlanner.Application/MealPlanner.Application.csproj src/MealPlanner.Application/
COPY backend/src/MealPlanner.Data/MealPlanner.Data.csproj src/MealPlanner.Data/
COPY backend/src/MealPlanner.Web/MealPlanner.Web.csproj src/MealPlanner.Web/
RUN dotnet restore src/MealPlanner.Web/MealPlanner.Web.csproj

COPY backend/src/ src/

ARG VERSION=0.0.0-dev
RUN dotnet publish src/MealPlanner.Web/MealPlanner.Web.csproj -c Release -o /app/publish --no-restore -p:UseAppHost=false -p:Version=${VERSION}

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=backend-builder /app/publish .
COPY --from=frontend-builder /src/frontend/dist ./wwwroot

USER $APP_UID
EXPOSE 8080

ENTRYPOINT ["dotnet", "MealPlanner.Web.dll"]
