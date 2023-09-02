FROM mcr.microsoft.com/dotnet/sdk:6.0 AS buildEnv
WORKDIR /App    

COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /App
COPY --from=buildEnv /App/out .
ENTRYPOINT [ "dotnet", "publisher.dll" ]