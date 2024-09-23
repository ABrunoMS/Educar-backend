# Define the version of .NET and the project name as build arguments
ARG VERSION=8.0

FROM mcr.microsoft.com/dotnet/sdk:$VERSION AS build

WORKDIR /app

COPY ["src/Web/Web.csproj", "src/Web/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]

ENV DOTNET_SYSTEM_NET_HTTP_USESOCKETSHTTPHANDLER=0

COPY Directory.Build.props ./
COPY Directory.Packages.props ./

RUN dotnet restore "src/Web/Web.csproj"
COPY . .
WORKDIR "/app/src/Web"
RUN dotnet build "Web.csproj" -c Release -o /app/build

FROM build AS publish

RUN dotnet publish "Web.csproj" \
  --runtime linux-x64 \
  --self-contained true \
  /p:PublishSingleFile=true \
  -c Release \
  -o /app/publish
  
FROM mcr.microsoft.com/dotnet/runtime-deps:$VERSION

# Install necessary libraries
# RUN apk add --no-cache icu-libs

WORKDIR /app

# Copy published files
COPY --from=publish /app/publish .

# Set environment variables
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    DOTNET_RUNNING_IN_CONTAINER=true \
    ASPNETCORE_URLS=http://+:8080

# Expose port 8080
EXPOSE 8080

# Adjust OpenSSL security level
RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf

# Disable .NET diagnostics
ENV DOTNET_EnableDiagnostics=0

# Use the project name dynamically for the entrypoint
ENTRYPOINT ["./Educar.Backend.Web", "--urls", "http://0.0.0.0:8080"]
