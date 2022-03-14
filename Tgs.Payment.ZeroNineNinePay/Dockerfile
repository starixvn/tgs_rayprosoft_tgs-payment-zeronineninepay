#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BuildConfiguration=Release
RUN --mount=type=secret,id=nuget,dst=/pat cp /pat ~/.nuget/NuGet/NuGet.Config
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.Payment.ZeroNineNinePay/Tgs.Payment.ZeroNineNinePay.csproj", "Tgs.Payment.ZeroNineNinePay/"]
COPY ["ZeroNineNinePay.Core/ZeroNineNinePay.Core.csproj", "ZeroNineNinePay.Core/"]
COPY ["Tgs.Payment.ZeroNineNinePay.Client/Tgs.Payment.ZeroNineNinePay.Client.csproj", "Tgs.Payment.ZeroNineNinePay.Client/"]
RUN dotnet restore "Tgs.Payment.ZeroNineNinePay/Tgs.Payment.ZeroNineNinePay.csproj"
COPY . .
WORKDIR "/src/Tgs.Payment.ZeroNineNinePay"
RUN dotnet build "Tgs.Payment.ZeroNineNinePay.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "Tgs.Payment.ZeroNineNinePay.csproj" -c $BuildConfiguration -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.Payment.ZeroNineNinePay.dll"]