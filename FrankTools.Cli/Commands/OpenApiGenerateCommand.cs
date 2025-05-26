using NJsonSchema.CodeGeneration.CSharp;
using NSwag;
using NSwag.CodeGeneration.CSharp;
using NSwag.CodeGeneration.CSharp.Models;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace FrankTools.Cli.Commands;

public class OpenApiGenerateCommand : Command<OpenApiGenerateCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandOption("-i|--input <URL_OR_PATH>")]
        [Description("Swagger/OpenAPI path (to .json file) or URL")]
        public string Input { get; set; } = default!;

        [CommandOption("-o|--output <DIR>")]
        [Description("Destination folder of generated code")]
        public string Output { get; set; } = "./Generated";

        [CommandOption("--type <client|server>")]
        [Description("Generation type: client o server")]
        public string Type { get; set; } = "client";

        [CommandOption("--class-name <NAME>")]
        [Description("Class name of client/server")]
        public string? ClassName { get; set; }

        [CommandOption("--use-httpclient-factory")]
        [Description("Use IHttpClientFactory (only for client)")]
        public bool UseHttpClientFactory { get; set; }

        [CommandOption("--generate-interfaces")]
        [Description("Generate interface for clients")]
        public bool GenerateInterfaces { get; set; }

        [CommandOption("--use-async")]
        [Description("Generate async methods (default: true)")]
        public bool UseAsync { get; set; } = true;

        [CommandOption("--namespace <NAMESPACE>")]
        [Description("Namespace da usare per il codice generato")]
        public string Namespace { get; set; } = "FrankTools.Generated";

        [CommandOption("--filename <FILENAME>")]
        [Description("Nome del file di output (senza estensione)")]
        public string? FileName { get; set; }
    }


    public override int Execute(CommandContext context, Settings settings)
    {
        return Run(settings).GetAwaiter().GetResult();
    }

    private async Task<int> Run(Settings s)
    {
        var filePath = await AnsiConsole.Status().Start("[bold yellow]🔧 Loading OpenAPI document...[/]", async ctx =>
        {
            var doc = s.Input.StartsWith("http")
            ? await OpenApiDocument.FromUrlAsync(s.Input)
            : await OpenApiDocument.FromFileAsync(s.Input);

            string code;
            string filePath;

            if (s.Type.ToLower() == "server")
            {
                var serverSettings = new CSharpControllerGeneratorSettings
                {
                    ControllerBaseClass = "ControllerBase",
                    ControllerStyle = CSharpControllerStyle.Partial,
                    CSharpGeneratorSettings =
                    {
                        Namespace = s.Namespace
                    }
                };

                var controllerGenerator = new CSharpControllerGenerator(doc, serverSettings);
                code = controllerGenerator.GenerateFile();
                filePath = Path.Combine(s.Output, "Controllers.cs");
            }
            else
            {
                var clientSettings = new CSharpClientGeneratorSettings
                {
                    ClassName = s.ClassName ?? "ApiClient",
                    UseHttpClientCreationMethod = s.UseHttpClientFactory,
                    GenerateClientInterfaces = s.GenerateInterfaces,
                    GenerateOptionalParameters = true,
                    InjectHttpClient = s.UseHttpClientFactory,
                    DisposeHttpClient = !s.UseHttpClientFactory,
                    UseBaseUrl = true,
                    GenerateClientClasses = true,
                    GenerateResponseClasses = true,
                    GenerateBaseUrlProperty = true,
                    CSharpGeneratorSettings =
                    {
                        Namespace = s.Namespace,
                        GenerateDefaultValues = true,
                        GenerateDataAnnotations = true,
                        GenerateNullableReferenceTypes = true,
                        RequiredPropertiesMustBeDefined = false,
                        JsonLibrary = CSharpJsonLibrary.SystemTextJson,
                    }
                };

                var clientGenerator = new CSharpClientGenerator(doc, clientSettings);
                code = clientGenerator.GenerateFile();
                var baseName = s.FileName ?? (s.Type.ToLower() == "server" ? "Controllers" : "ApiClient");
                filePath = Path.Combine(s.Output, baseName + ".cs");

            }

            Directory.CreateDirectory(s.Output);
            await File.WriteAllTextAsync(filePath, code);
            return filePath;
        });

        AnsiConsole.MarkupLine($"[green]✅ Code generated in: [underline]{Path.GetFullPath(filePath)}[/][/]");
        return 0;
    }
}

