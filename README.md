# Library-Web-API
# Библиотечное Приложение на ASP.NET Core

Это библиотечное приложение, созданное на ASP.NET Core с использованием .NET 9.0. Проект состоит из четырех подпроектов: `Library.DataAccess`, `Library.BusinessLogic`, `Library.WebAPI` и `Library.Tests`. Для базы данных используется MySQL.

## Структура Проекта

1. **Library.DataAccess**: Слой доступа к данным, использующий Entity Framework Core для взаимодействия с MySQL.
2. **Library.BusinessLogic**: Бизнес-логика приложения, включающая сервисы и валидацию данных.
3. **Library.WebAPI**: Веб-API для взаимодействия с клиентской частью приложения.
4. **Library.Tests**: Unit-тесты для проверки функциональности сервисов и репозиториев.

## Требования

- [.NET SDK 9.0](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker](https://www.docker.com/products/docker-desktop)
- [MySQL](https://dev.mysql.com/downloads/mysql/)
- [Visual Studio](https://visualstudio.microsoft.com/)

## Установка и Настройка

### 1. Клонирование Репозитория

Клонируйте репозиторий на свой компьютер:

```bash
git clone https://github.com/mda-les13/Library-Web-API.git
cd library-api
```

### 2. Установка Зависимостей

Убедитесь, что все необходимые пакеты установлены. Выполните следующую команду в корневой директории проекта:

```bash
dotnet restore
```

### 3. Настройка Базы Данных

#### 3.1. Установка MySQL

Установите MySQL и создайте базу данных для приложения.

#### 3.2. Настройка Connection String

Откройте файл `appsettings.json` в проекте `Library.WebAPI` и настройте строку подключения к базе данных MySQL:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;port=3306;database=LibraryDb;user=root;password=yourpassword;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "AppSettings": {
    "Secret": "your_secret_key_here"
  }
}
```

### 4. Запуск Проекта через Visual Studio

1. Откройте решение `Library.sln` в Visual Studio.
2. Убедитесь, что проект `Library.WebAPI` установлен как запускаемый.
3. Нажмите клавишу `F5` для запуска проекта.

### 5. Запуск Тестов

#### 5.1. Настройка Connection String для Тестов

Откройте файл `appsettings.json` в проекте `Library.Tests` и настройте строку подключения к базе данных MySQL:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;port=3306;database=LibraryDb;user=root;password=yourpassword;"
  },
  "AppSettings": {
    "Secret": "your_secret_key_here"
  }
}
```

#### 5.2. Запуск Тестов через Visual Studio

1. Откройте решение `LibraryApi.sln` в Visual Studio.
2. Щелкните правой кнопкой мыши на проекте `Library.Tests`.
3. Выберите `Run Tests`.

#### 5.3. Запуск Тестов через Командную Строку

Запустите тесты с помощью команды:

```bash
dotnet test Library.Tests/Library.Tests.csproj
```

### 6. Запуск через Docker
Для запуска приложения в Docker я использовал Visual Studio 2022, Docker Desktop и MySQL. В качестве dockerfile я взял стандартный, который создаётся вместе с проектом в Visual Studio.
Для успешного запуска необходимо сначала убедиться в правильности подключения к базе данных. Строка подключения у меня выглядит так:
```
"DefaultConnection": "server=host.docker.internal;Port=3306;database=LibraryDb;user=root;password=123456;"
```
* server=host.docker.internal оставляем без изменений;
* Port=3306 меняем на свой порт при необходимости (по стандарту в MySQL предлагается именно этот порт);
* database=LibraryDb оставляем без изменений;
* user=root меняем на свой (посмотреть можно в MySQL Connections);
* password=123456 меняем на свой (посмотреть можно в MySQL Connections);

Если всё сделано правильно, то проблем с подключением к бд быть не должно. Далее порядок действий довольно прост (при наличии всего необходимого):
1. Открываем проект через Visual Studio
2. Выбираем запуск с контейнера (Container (Dockerfile))
3. Запускаем Docker Desktop
4. Проверяем, что службы MySQL работают
5. Запускаем сам проект

После запуска убедиться в том, что всё работает можно через порт 8081. Для этого переходим по ссылке:
```
https://localhost:32769/swagger/index.html
```
Значение после localhost может различаться, но роли не играет. Если всё сделано правильно, то нас встретит Swagger.
