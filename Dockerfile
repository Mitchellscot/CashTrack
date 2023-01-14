# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:7.0-jammy AS build
WORKDIR /dist
EXPOSE 5000
# copy csproj and restore as distinct layers
COPY src/*.csproj .
RUN dotnet restore --use-current-runtime  

# install node
ENV NODE_VERSION 18.13.0
ENV NODE_DOWNLOAD_SHA dc68e229425b941eeae0b1d59c66c680b56fd536d0ad2311e3fb009bd83661e4
ENV NODE_DOWNLOAD_URL https://nodejs.org/dist/v$NODE_VERSION/node-v$NODE_VERSION-linux-arm64.tar.gz
RUN wget "$NODE_DOWNLOAD_URL" -O nodejs.tar.gz \
       && echo "$NODE_DOWNLOAD_SHA  nodejs.tar.gz" | sha256sum -c - \
       && tar -xzf "nodejs.tar.gz" -C /usr/local --strip-components=1 \
       && rm nodejs.tar.gz \
       && ln -s /usr/local/bin/node /usr/local/bin/nodejs \
       && curl -sL https://deb.nodesource.com/setup_18.x |  bash - \
       && apt update \
       && apt-get install -y nodejs

# copy everything else and build app
ADD src/. .
RUN dotnet ef migrations add Init --project src/CashTrack.csproj -o src/Data/Migrations -- seed
RUN dotnet ef database update --project ./src/CashTrack.csproj
RUN dotnet publish -c Release -o /app --use-current-runtime --self-contained false --no-restore
COPY src/cashtrack.db /app

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "CashTrack.dll", "--environment=Docker"]
