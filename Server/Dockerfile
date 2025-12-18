FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

# Copy solution and project files first for layer caching
COPY ["Server.sln", "./"]
COPY ["Api/Api.csproj", "Api/"]
COPY ["Dataaccess/", "DataAccess/"]
COPY ["Utils/Utils.csproj", "Utils/"]
COPY ["Test/Test.csproj", "Test/"]

# Restore dependencies with the target runtime
RUN dotnet restore "Api/Api.csproj" --runtime linux-musl-x64

# Copy the rest of the source code (this will overwrite/merge)
COPY ["Api/", "Api/"]
COPY ["Utils/", "Utils/"]

# Build and publish
WORKDIR /src/Api
RUN dotnet publish "Api.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    --runtime linux-musl-x64 \
    --self-contained true \
    /p:PublishSingleFile=true

FROM mcr.microsoft.com/dotnet/runtime-deps:9.0-alpine

# Install timezone data for Alpine Linux
RUN apk add --no-cache tzdata

# Set timezone environment variable
ENV TZ=Europe/Copenhagen

WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

ENTRYPOINT ["./Api"]