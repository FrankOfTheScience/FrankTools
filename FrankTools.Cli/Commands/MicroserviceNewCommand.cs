using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Text;

namespace FrankTools.Cli.Commands;

public class MicroserviceNewCommand : Command<MicroserviceNewCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<NAME>")]
        [Description("Microservice name to create")]
        public string ServiceName { get; set; } = default!;

        [CommandOption("--mongo")]
        [Description("Add MongoDB template")]
        public bool UseMongo { get; set; }

        [CommandOption("--sql")]
        [Description("Add SQL Server template")]
        public bool UseSql { get; set; }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        var name = settings.ServiceName;
        var rootPath = Path.Combine(Directory.GetCurrentDirectory(), name);

        if (Directory.Exists(rootPath))
        {
            AnsiConsole.MarkupLine($"[red]❌ '{name}' folder already exists![/]");
            return -1;
        }

        AnsiConsole.Status().Start($"[yellow]🚧 Creatign structure for microservice '{name}'...[/]", ctx => 
        {
            Directory.CreateDirectory(rootPath);
            Directory.CreateDirectory(Path.Combine(rootPath, $"{name}.API", "Controllers"));
            Directory.CreateDirectory(Path.Combine(rootPath, $"{name}.Application"));
            Directory.CreateDirectory(Path.Combine(rootPath, $"{name}.Domain"));
            Directory.CreateDirectory(Path.Combine(rootPath, $"{name}.Infrastructure"));

            CreateFile(Path.Combine(rootPath, $"{name}.API", "Program.cs"), GetProgramTemplate(name, settings));
            CreateFile(Path.Combine(rootPath, $"{name}.API", $"{name}.API.csproj"), GetCsprojTemplate(name, settings));
            CreateFile(Path.Combine(rootPath, $"{name}.API", "appsettings.json"), GetAppSettingsTemplate(settings));
            CreateFile(Path.Combine(rootPath, "Dockerfile"), GetDockerfileTemplate(name));
            CreateFile(Path.Combine(rootPath, "docker-compose.yml"), GetDockerComposeTemplate(name, settings));
            CreateFile(Path.Combine(rootPath, ".gitignore"), GetGitignoreTemplate());
            CreateFile(Path.Combine(rootPath, "README.md"), GetReadmeTemplate(name, settings));
            CreateSolutionFile(rootPath, name);
        });
        
        AnsiConsole.MarkupLine($"[green]✅ Microservice '{name}' created in:[/] [blue]{rootPath}[/]");
        return 0;
    }

    private void CreateFile(string path, string content)
    {
        File.WriteAllText(path, content);
        AnsiConsole.MarkupLine($"[grey]  > Created:[/] {path}");
    }

    private void CreateSolutionFile(string rootPath, string name)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Microsoft Visual Studio Solution File, Format Version 12.00");
        sb.AppendLine("# Visual Studio Version 17");
        sb.AppendLine("VisualStudioVersion = 17.3.32922.545");
        sb.AppendLine("MinimumVisualStudioVersion = 10.0.40219.1");

        string projGuid = Guid.NewGuid().ToString("B").ToUpper();
        sb.AppendLine($"Project(\"{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}\") = \"{name}.API\", \"{name}.API\\{name}.API.csproj\", \"{projGuid}\"");
        sb.AppendLine("EndProject");

        sb.AppendLine("Global");
        sb.AppendLine("\tGlobalSection(SolutionConfigurationPlatforms) = preSolution");
        sb.AppendLine("\t\tDebug|Any CPU = Debug|Any CPU");
        sb.AppendLine("\t\tRelease|Any CPU = Release|Any CPU");
        sb.AppendLine("\tEndGlobalSection");

        sb.AppendLine("\tGlobalSection(ProjectConfigurationPlatforms) = postSolution");
        sb.AppendLine($"\t\t{projGuid}.Debug|Any CPU.ActiveCfg = Debug|Any CPU");
        sb.AppendLine($"\t\t{projGuid}.Debug|Any CPU.Build.0 = Debug|Any CPU");
        sb.AppendLine($"\t\t{projGuid}.Release|Any CPU.ActiveCfg = Release|Any CPU");
        sb.AppendLine($"\t\t{projGuid}.Release|Any CPU.Build.0 = Release|Any CPU");
        sb.AppendLine("\tEndGlobalSection");

        sb.AppendLine("\tGlobalSection(SolutionProperties) = preSolution");
        sb.AppendLine("\t\tHideSolutionNode = FALSE");
        sb.AppendLine("\tEndGlobalSection");

        sb.AppendLine("EndGlobal");

        var slnPath = Path.Combine(rootPath, $"{name}.sln");
        File.WriteAllText(slnPath, sb.ToString());
        AnsiConsole.MarkupLine($"[grey]  > Creato:[/] {slnPath}");
    }

    private string GetProgramTemplate(string name, Settings settings)
    {
        var mongoSetup = settings.UseMongo ? "builder.Services.AddSingleton<IMongoClient>(new MongoClient(\"mongodb://localhost:27017\"));" : "";
        var sqlSetup = settings.UseSql ? "builder.Services.AddDbContext<MyDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString(\"DefaultConnection\")));" : "";

        return
$"""
var builder = WebApplication.CreateBuilder(args);

{mongoSetup}
{sqlSetup}

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();
""";
    }

    private string GetCsprojTemplate(string name, Settings settings)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<Project Sdk=\"Microsoft.NET.Sdk.Web\">");
        sb.AppendLine("  <PropertyGroup>");
        sb.AppendLine("    <TargetFramework>net8.0</TargetFramework>");
        sb.AppendLine("    <Nullable>enable</Nullable>");
        sb.AppendLine("    <ImplicitUsings>enable</ImplicitUsings>");
        sb.AppendLine("  </PropertyGroup>");

        if (settings.UseSql)
        {
            sb.AppendLine("  <ItemGroup>");
            sb.AppendLine("    <PackageReference Include=\"Microsoft.EntityFrameworkCore.SqlServer\" Version=\"8.0.0\" />");
            sb.AppendLine("    <PackageReference Include=\"Microsoft.EntityFrameworkCore.Tools\" Version=\"8.0.0\">");
            sb.AppendLine("      <PrivateAssets>all</PrivateAssets>");
            sb.AppendLine("      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>");
            sb.AppendLine("    </PackageReference>");
            sb.AppendLine("  </ItemGroup>");
        }

        if (settings.UseMongo)
        {
            sb.AppendLine("  <ItemGroup>");
            sb.AppendLine("    <PackageReference Include=\"MongoDB.Driver\" Version=\"2.19.1\" />");
            sb.AppendLine("  </ItemGroup>");
        }

        sb.AppendLine("</Project>");
        return sb.ToString();
    }

    private string GetAppSettingsTemplate(Settings settings)
    {
        var sb = new StringBuilder();
        sb.AppendLine("{");
        sb.AppendLine("  \"Logging\": {");
        sb.AppendLine("    \"LogLevel\": {");
        sb.AppendLine("      \"Default\": \"Information\",");
        sb.AppendLine("      \"Microsoft.AspNetCore\": \"Warning\"");
        sb.AppendLine("    }");
        sb.AppendLine("  },");
        sb.AppendLine("  \"AllowedHosts\": \"*\"");

        if (settings.UseSql)
        {
            sb.AppendLine("  ,\"ConnectionStrings\": {");
            sb.AppendLine("    \"DefaultConnection\": \"Server=localhost;Database=MyDb;User Id=sa;Password=your_password;TrustServerCertificate=True;\"");
            sb.AppendLine("  }");
        }
        sb.AppendLine("}");
        return sb.ToString();
    }

    private string GetDockerfileTemplate(string name)
    {
        return $"""
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . ./
RUN dotnet publish {name}.API/{name}.API.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "{name}.API.dll"]
""";
    }

    private string GetDockerComposeTemplate(string name, Settings settings)
    {
        var mongoService = settings.UseMongo ? """
  mongo:
    image: mongo:latest
    restart: always
    ports:
      - "27017:27017"
""" : "";

        var sqlService = settings.UseSql ? """
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      SA_PASSWORD: "Your_password123"
      ACCEPT_EULA: "Y"
    ports:
      - "1433:1433"
    restart: always
""" : "";

        return
$"""
version: '3.8'

services:
  {name.ToLower()}:
    build:
      context: .
      dockerfile: Dockerfile
    ports:
      - "80:80"
    depends_on:
{(settings.UseMongo ? "      - mongo" : "")}
{(settings.UseSql ? "      - sqlserver" : "")}

{mongoService}
{sqlService}
""";
    }

    private string GetGitignoreTemplate()
    {
        return """
bin/
obj/
.vs/
*.user
*.suo
*.userprefs
*.DS_Store
*.log
""";
    }

    private string GetReadmeTemplate(string name, Settings settings)
    {
        var dbInfo = settings.UseMongo ? "- Uses MongoDB as NoSQL DB.\n" : "";
        dbInfo += settings.UseSql ? "- Uses SQL Server as relational DB.\n" : "";
        if (dbInfo == "") dbInfo = "- No default DB set.\n";

        return
$"""
# {name}

Microservizio generated with [FrankTools](https://github.com/FrankOfTheScience/franktools) 🚀

## Structure

- Folder API, Application, Domain, Infrastructure created.
- Dockerfile and docker-compose.yml included.
- .NET 8.0 project with nullable and implicit usings.

## Database

{dbInfo}

## How to use

- Open solution `{name}.sln` in Visual Studio or VS Code.
- Execute `dotnet build` and `dotnet run --project {name}.API`.
- Use Docker: `docker-compose up --build` to start the microservice DB containers.

""";
    }
}
