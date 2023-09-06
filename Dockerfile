# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:7.0-jammy AS build
WORKDIR /dist
EXPOSE 5000
# copy csproj and restore as distinct layers
COPY src/*.csproj .
RUN dotnet restore --use-current-runtime  

# install node
ENV NODE_MAJOR 18
ENV NODE_DOWNLOAD_SHA dc68e229425b941eeae0b1d59c66c680b56fd536d0ad2311e3fb009bd83661e4
ENV NODE_DOWNLOAD_URL https://nodejs.org/dist/v$NODE_VERSION/node-v$NODE_VERSION-linux-arm64.tar.gz
RUN apt-get update \
       && apt-get install -y ca-certificates curl gnupg \
       && mkdir -p /etc/apt/keyrings \
       && curl -fsSL https://deb.nodesource.com/gpgkey/nodesource-repo.gpg.key | sudo gpg --dearmor -o /etc/apt/keyrings/nodesource.gpg 
       && echo "deb [signed-by=/etc/apt/keyrings/nodesource.gpg] https://deb.nodesource.com/node_$NODE_MAJOR.x jammy main" | sudo tee /etc/apt/sources.list.d/nodesource.list \
       && apt-get update \
       && apt-get install -y nodejs

# copy everything else and build app
ADD src/. .
RUN dotnet publish -c Release -o /app --use-current-runtime --self-contained false --no-restore
COPY src/Data/cashtrack.db /app/Data/cashtrack.db

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "CashTrack.dll", "--environment=Docker"]
