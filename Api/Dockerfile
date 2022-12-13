FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app
EXPOSE 80

COPY ["Api/*.csproj", "Api/"]
COPY ["Api.Models/*.csproj", "Api.Models/"]
COPY ["Api.Services/*.csproj", "./Api.Services/"]
COPY ["Api.UseCases/*.csproj", "./Api.UseCases/"]

COPY . ./
RUN dotnet restore "Api/Api.csproj"
RUN dotnet publish "Api/Api.csproj" -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .

ENTRYPOINT ["dotnet", "Api.dll"]