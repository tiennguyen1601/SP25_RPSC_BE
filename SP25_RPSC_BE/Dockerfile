# Use .NET SDK to build the app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

# Copy the project files
COPY . /source

WORKDIR /source/SP25_RPSC.Controllers

ARG TARGETARCH

# Build the application
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish -a ${TARGETARCH/amd64/x64} --use-current-runtime --self-contained false -o /app

# Create a final stage for running the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final

# Install fonts and fontconfig
RUN apk update && apk add --no-cache \
    ttf-dejavu \
    fontconfig \
    && fc-cache -fv  # Install the fonts and refresh the font cache

# Install ICU libraries for globalization support
RUN apk add icu-libs

# Cài đặt tzdata để có thể thiết lập múi giờ
RUN apk add --no-cache tzdata

# Thiết lập múi giờ
ENV TZ=Asia/Ho_Chi_Minh
RUN cp /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

WORKDIR /app

# Copy everything needed to run the app from the build stage
COPY --from=build /app .

# Switch to a non-privileged user
USER $APP_UID

# Set the entrypoint for the container
ENTRYPOINT ["dotnet", "SP25_RPSC.Controllers.dll"]
