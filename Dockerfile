# Dockerfile
# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:7.0-jammy AS build
WORKDIR /dist

# copy csproj and restore as distinct layers
COPY src/*.csproj .
RUN dotnet restore --use-current-runtime  

# install node
RUN apt-get update && apt-get install -y \
    software-properties-common \
    npm \
    apt-utils
RUN npm install npm@8.6.0 -g && \
    npm install n -g && \
    n v18.13.0

# copy everything else and build app
COPY src/. .
RUN dotnet publish -c Release -o /app --use-current-runtime --self-contained false --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "CashTrack.dll", "--environment Development"]