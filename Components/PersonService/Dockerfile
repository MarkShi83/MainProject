# Stage 1
FROM microsoft/dotnet:2.2-sdk AS builder
ARG version
WORKDIR /src

COPY ./src .

WORKDIR /src/PersonService
RUN dotnet publish --output /app/ --configuration Release -p:AssemblyVersion=$version

# Stage 2
FROM microsoft/dotnet:2.2-aspnetcore-runtime

WORKDIR /app
COPY --from=builder /app .

ENV ASPNETCORE_URLS "http://0.0.0.0:5000/"

ENV TZ=Australia/Sydney
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

ENTRYPOINT ["dotnet", "PersonService.dll"]
