# Educar Backend

## Technologies Used
- .NET Core 8
- PostgreSQL for data storage
- Blob Storage (Azure/OCI)

## Building the project

The project can be executed using the following command assuming you have the correct appsettings configured:

```bash
 dotnet run --project src/Web
```

There is an example of the appsettings file in the `src/Web/appsettings.Example.json` file. You can copy this file to `src/Web/appsettings.json` and configure the connection strings and other settings as needed.

Once the application is running, you can access the Swagger API documentation at `/api`.

## Running the tests

The tests can be executed using the following command:

```bash
 dotnet test
```

## Running the migrations
The migrations are executed automatically when the application is started. If you want to run them manually, you can use the following command:

```bash
 dotnet ef database update --project src/Infrastructure --startup-project src/Web
```

## Building a container

To build a container, you can use the provided Dockerfile or you can use `dotnet publish` as explained [here](https://learn.microsoft.com/en-us/dotnet/core/docker/publish-as-container):
```bash
dotnet publish src/Web/Web.csproj --os linux --arch x64 /t:PublishContainer -c Release --self-contained true
```