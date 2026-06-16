# Stage 1: Build the React frontend
FROM node:22-alpine AS frontend-build
WORKDIR /src/newsportal-ui
COPY newsportal-ui/package*.json ./
RUN npm ci
COPY newsportal-ui/ ./
RUN npm run build

# Stage 2: Build the .NET backend
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS backend-build
# Trust corporate proxy CA (Zscaler)
COPY zscaler-root-ca.crt /usr/local/share/ca-certificates/zscaler-root-ca.crt
RUN update-ca-certificates
WORKDIR /src
COPY NewsPortal.sln ./
COPY NewsPortal/NewsPortal.csproj NewsPortal/
COPY NewsPortal.BLL/NewsPortal.BLL.csproj NewsPortal.BLL/
COPY NewsPOrtal.DAL/NewsPortal.DAL.csproj NewsPOrtal.DAL/
COPY NewsPortal.Common/NewsPortal.Common.csproj NewsPortal.Common/
RUN dotnet restore NewsPortal/NewsPortal.csproj
COPY NewsPortal/ NewsPortal/
COPY NewsPortal.BLL/ NewsPortal.BLL/
COPY NewsPOrtal.DAL/ NewsPOrtal.DAL/
COPY NewsPortal.Common/ NewsPortal.Common/
RUN dotnet publish NewsPortal/NewsPortal.csproj -c Release -o /app/publish

# Stage 3: Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=backend-build /app/publish .
COPY --from=frontend-build /src/newsportal-ui/dist ./wwwroot
ENTRYPOINT ["dotnet", "NewsPortal.dll"]
