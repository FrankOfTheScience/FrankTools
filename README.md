# 🛠️ FrankTools CLI

[![NuGet](https://img.shields.io/nuget/v/FrankTools.Cli.svg?style=flat-square)](https://www.nuget.org/packages/FrankTools.Cli)
[![NuGet Downloads](https://img.shields.io/nuget/dt/FrankTools.Cli.svg?style=flat-square)](https://www.nuget.org/packages/FrankTools.Cli)


**FrankTools CLI** is a lightweight command-line tool built in .NET for various automation and developer utilities.

> 🚀 Published on NuGet as a .NET CLI Tool.

---

## 📦 Installation

To install the tool globally using the .NET CLI:

```bash
dotnet tool install --global FrankTools.Cli
```

## 🔄 To update to the latest version:

```bash
dotnet tool update --global FrankTools.Cli
```

## ❌ To uninstall:

```bash
dotnet tool uninstall --global FrankTools.Cli
```

--------

## 🧪 Usage
Once installed, you can use the tool from your terminal by running these two commands:

### MicroserviceNewCommand
The MicroserviceNewCommand is a CLI command designed to scaffold a new microservice project structure with optional database templates. It is part of the FrankTools CLI tool and helps developers quickly set up a microservice with a predefined folder structure, configuration files, and optional database integrations.

#### Command
```bash
franktools microservice-new <NAME> [--mongo] [--sql]
```

#### Parameters
•	<NAME>: (Required) The name of the microservice to create. This will be used as the root folder name and project name.
•	--mongo: (Optional) Adds a MongoDB template to the microservice.
•	--sql: (Optional) Adds a SQL Server template to the microservice.

#### Output
When the command is executed, the following structure is created in the current directory:

```text
MyMicroservice/
├── MyMicroservice.API/
│   ├── Controllers/
│   ├── Program.cs
│   ├── MyMicroservice.API.csproj
│   ├── appsettings.json
├── MyMicroservice.Application/
├── MyMicroservice.Domain/
├── MyMicroservice.Infrastructure/
├── Dockerfile
├── docker-compose.yml
├── .gitignore
├── README.md
├── MyMicroservice.sln
```

#### Key Features
1.	Folder Structure:
•	API: Contains the main API project.
•	Application: For application-level logic.
•	Domain: For domain models and business logic.
•	Infrastructure: For infrastructure-related code.
2.	Configuration Files:
•	Program.cs: Pre-configured with optional MongoDB and SQL Server setup.
•	appsettings.json: Includes logging and optional database connection strings.
•	.gitignore: Pre-configured to ignore common files and folders.
3.	Docker Support:
•	Dockerfile: Configured for building and running the microservice.
•	docker-compose.yml: Includes optional MongoDB and SQL Server services.
4.	Solution File:
•	A .sln file is generated for easy integration with Visual Studio.
5.	README.md:
•	A generated README file with instructions on how to use the microservice.

### OpenApiGenerateCommand Documentation
The OpenApiGenerateCommand is a CLI command that generates C# client or server code from an OpenAPI/Swagger specification. It simplifies the process of integrating APIs into your application by automatically creating the necessary code based on the provided OpenAPI document.

#### Command

```bash
franktools openapi-generate -i <URL_OR_PATH> [-o <DIR>] [--type <client|server>] [OPTIONS]
```

#### Parameters
•	-i|--input <URL_OR_PATH> (Required)
The path or URL to the OpenAPI/Swagger .json file.
Example: https://example.com/swagger.json or ./swagger.json.
•	-o|--output <DIR> (Optional)
The destination folder where the generated code will be saved.
Default: ./Generated.
•	--type <client|server> (Optional)
Specifies whether to generate client or server code.
Default: client.
•	--class-name <NAME> (Optional)
The name of the generated client or server class
Default: ApiClient (for client) or Controllers (for server).
•	--use-httpclient-factory (Optional)
Enables the use of IHttpClientFactory for dependency injection (client only).
•	--generate-interfaces (Optional)
Generates interfaces for the client classes.
•	--use-async (Optional)
Generates asynchronous methods.
Default: true.
•	--namespace <NAMESPACE> (Optional)
The namespace to use for the generated code.
Default: FrankTools.Generated.
•	--filename <FILENAME> (Optional)
The name of the output file (without extension).
Default: ApiClient.cs (for client) or Controllers.cs (for server).

#### Output
The command generates the following:
•	Client Code (ApiClient.cs):
A fully functional C# client for interacting with the API, including optional support for IHttpClientFactory, interfaces, and asynchronous methods.
•	Server Code (Controllers.cs):
Server-side controllers based on the OpenAPI specification, ready to be integrated into an ASP.NET Core project.
•	Namespace:
The generated code uses the specified namespace (default: FrankTools.Generated).

-------

## ⚙️ How It Works
FrankTools CLI utilizes C# source generators to automatically discover and register commands at compile-time. This approach allows for:

Modularity: Easily add or remove commands without modifying the core application logic.

Performance: Compile-time generation ensures minimal runtime overhead.

Maintainability: Clear separation of concerns between command definitions and execution logic.


