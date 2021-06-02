#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["CloudXProductShop/CloudXProductShop.csproj", "CloudXProductShop/"]
RUN dotnet restore "CloudXProductShop/CloudXProductShop.csproj"
COPY . .
WORKDIR "/src/CloudXProductShop"
RUN dotnet build "CloudXProductShop.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CloudXProductShop.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV LANG=en_US.UTF-8 \
    LANGUAGE=en_US.UTF-8
ENTRYPOINT ["dotnet", "CloudXProductShop.dll"]