FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

ADD --chmod=644 https://truststore.pki.rds.amazonaws.com/global/global-bundle.pem /cert/global-bundle.pem

WORKDIR /cert/

RUN cat global-bundle.pem|awk 'split_after==1{n++;split_after=0} /-----END CERTIFICATE-----/ {split_after=1} {print > "cert" n ""}' ;\
    for CERT in /cert/cert*; do mv $CERT /usr/local/share/ca-certificates/aws-rds-ca-$(basename $CERT).crt; done ;\
    update-ca-certificates

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

ARG ART_USER
ARG ART_PASS
ARG ART_URL

RUN dotnet nuget add source --name crossknowledge/phoenix $ART_URL --username $ART_USER --password $ART_PASS --store-password-in-clear-text
RUN mkdir Reporting.API
COPY ./Reporting.API/Reporting.API.csproj ./Reporting.API/
RUN mkdir Reporting.Domain
COPY ./Reporting.Domain/Reporting.Domain.csproj ./Reporting.Domain/
RUN mkdir Reporting.Infrastructure
COPY ./Reporting.Infrastructure/Reporting.Infrastructure.csproj ./Reporting.Infrastructure/
RUN mkdir Reporting.Infrastructure.Interface
COPY ./Reporting.Infrastructure.Interface/Reporting.Infrastructure.Interface.csproj ./Reporting.Infrastructure.Interface/
RUN mkdir Reporting.Services
COPY ./Reporting.Services/Reporting.Services.csproj ./Reporting.Services/
COPY . .
RUN dotnet restore "Reporting.API/Reporting.API.csproj"
WORKDIR "/src/Reporting.API"
RUN dotnet build "Reporting.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Reporting.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Reporting.API.dll"]
