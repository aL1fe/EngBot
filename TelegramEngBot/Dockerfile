# Определение базового образа для сборки
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env 
WORKDIR /app

COPY . .
RUN ls -la
RUN cd TelegramEngBot/TelegaEngBot && ls -la
RUN dotnet restore TelegaEngBot.sln

# Копирование файла Solution и восстановление зависимостей
# COPY . .
# RUN dotnet restore

# Сборка
# RUN dotnet publish -c Release -o out

# Определение базового образа для запуска
# FROM mcr.microsoft.com/dotnet/aspnet:6.0
# WORKDIR /app

# Копирование собранного приложения
# COPY --from=build-env /app/out .

# Установка точки входа
# ENTRYPOINT ["dotnet", "TelegaEngBot.dll"]
