
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["School-Admission-System-Backend.csproj", "./"]
RUN dotnet restore "School-Admission-System-Backend.csproj"

COPY . .
RUN dotnet publish "School-Admission-System-Backend.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM build AS test
RUN dotnet restore "SchoolAdmission.Tests/SchoolAdmission.Tests.csproj"
RUN dotnet build "SchoolAdmission.Tests/SchoolAdmission.Tests.csproj" \
    -c Release \
    --no-restore
ENTRYPOINT ["dotnet", "test", "SchoolAdmission.Tests/SchoolAdmission.Tests.csproj", \
    "-c", "Release", \
    "--no-build", \
    "--logger", "console;verbosity=normal"]

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final
WORKDIR /app

RUN addgroup -S appgroup && adduser -S appuser -G appgroup
USER appuser

ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_RUNNING_IN_CONTAINER=true

EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "School-Admission-System-Backend.dll"]
