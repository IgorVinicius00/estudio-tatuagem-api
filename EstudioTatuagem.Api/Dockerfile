FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["EstudioTatuagem.Api/EstudioTatuagem.Api.csproj", "EstudioTatuagem.Api/"]
RUN dotnet restore "EstudioTatuagem.Api/EstudioTatuagem.Api.csproj"

COPY . .
WORKDIR "/src/EstudioTatuagem.Api"
RUN dotnet build "EstudioTatuagem.Api.csproj" -c Release -o /app/build
RUN dotnet publish "EstudioTatuagem.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=10000
EXPOSE 10000
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "EstudioTatuagem.Api.dll"]