# Use the official .NET SDK as the build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the project and restore dependencies
COPY *.sln ./
COPY UoBAttendanceCodeDatabase.Server/*.fsproj ./UoBAttendanceCodeDatabase.Server/
RUN dotnet restore

# Copy the rest of the project files and build
COPY . ./
RUN dotnet publish -c Release -o out

# Use the official .NET runtime as the base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy the built files from the build image
COPY --from=build /app/out .

# Make sure SQLite database has persistence
VOLUME /app/db
RUN mkdir -p /app/db

# Set the environment for ASP.NET Core
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

# Run the app
ENTRYPOINT ["dotnet", "UoBAttendanceCodeDatabase.Server.dll"]
