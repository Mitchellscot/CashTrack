# Dockerfile
# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:7.0-jammy AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY src/*.csproj .
RUN dotnet restore --use-current-runtime  

# install node
RUN apt-get update && apt-get install -y \
    software-properties-common \
    npm
RUN npm install npm@8.6.0 -g && \
    npm install n -g && \
    n v18.12.1

# copy everything else and build app
COPY src/. .
# remove the install stamp so 'npm install' can be rerun in container, if it exists
RUN rm -f /source/src/node_modules/.install-stamp
RUN dotnet publish -c Release -o /app --use-current-runtime --self-contained false --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "CashTrack.dll", "--environment Development"]