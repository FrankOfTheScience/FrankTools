using Spectre.Console.Cli;
using FrankTools.Cli.Commands;

var app = new CommandApp();

app.Configure(config =>
{
    config.SetApplicationName("franktools");

    // openapi-gen command
    config.AddCommand<OpenApiGenerateCommand>("openapi-gen")
          .WithDescription("Generate client/server C# from OpenAPI file");

    // microgen new command
    config.AddCommand<MicroserviceNewCommand>("microgen new")
          .WithDescription("Generate microservice C# structure from scratch")
          .WithExample([ "microservice", "new", "PaymentService", "--mongo" ]); ;
});

return app.Run(args);