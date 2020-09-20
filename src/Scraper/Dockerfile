FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out -p:PublishChromeDriver=true

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

USER root

# install chrome
RUN apt-get update -qqy \
  && apt-get -qqy install wget \
     gnupg2
ARG CHROME_VERSION="google-chrome-stable"
RUN wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub | apt-key add - \
  && echo "deb http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google-chrome.list \
  && apt-get update -qqy \
  && apt-get -qqy install \
    # ${CHROME_VERSION:-google-chrome-stable} \
    chromium-driver \
  && rm /etc/apt/sources.list.d/google-chrome.list \
  && rm -rf /var/lib/apt/lists/* /var/cache/apt/*

WORKDIR /app

COPY --from=build-env /app/out .

ENTRYPOINT ["dotnet", "coach_bags_selenium.dll"]