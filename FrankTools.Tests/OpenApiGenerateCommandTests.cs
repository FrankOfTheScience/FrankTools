using FrankTools.Cli.Commands;
using Spectre.Console.Cli;

public class OpenApiGenerateCommandTests
{
    [Fact]
    public async Task Should_Generate_Client_Code()
    {
        // Arrange
        var settings = new OpenApiGenerateCommand.Settings
        {
            Input = "test-openapi.json",
            Output = "./GeneratedClient",
            Type = "client",
            ClassName = "TestClient",
            Namespace = "TestNamespace"
        };

        var command = new OpenApiGenerateCommand();
        var context = new CommandContext(null!, null!, null!, null!);

        // Mock OpenAPI document
        File.WriteAllText(settings.Input, "{}");

        // Act
        var result = await Task.Run(() => command.Execute(context, settings));

        // Assert
        Assert.Equal(0, result);
        Assert.True(Directory.Exists(settings.Output));
        Assert.True(File.Exists(Path.Combine(settings.Output, "ApiClient.cs")));

        // Cleanup
        Directory.Delete(settings.Output, true);
        File.Delete(settings.Input);
    }

    [Fact]
    public async Task Should_Generate_Server_Code()
    {
        // Arrange
        var settings = new OpenApiGenerateCommand.Settings
        {
            Input = "test-openapi.json",
            Output = "./GeneratedServer",
            Type = "server",
            Namespace = "TestNamespace"
        };

        var command = new OpenApiGenerateCommand();
        var context = new CommandContext(null!, null!, null!, null!);

        // Mock OpenAPI document
        File.WriteAllText(settings.Input, "{}");

        // Act
        var result = await Task.Run(() => command.Execute(context, settings));

        // Assert
        Assert.Equal(0, result);
        Assert.True(Directory.Exists(settings.Output));
        Assert.True(File.Exists(Path.Combine(settings.Output, "Controllers.cs")));

        // Cleanup
        Directory.Delete(settings.Output, true);
        File.Delete(settings.Input);
    }

    [Fact]
    public async Task Should_Handle_Invalid_Input_Path()
    {
        // Arrange
        var settings = new OpenApiGenerateCommand.Settings
        {
            Input = "invalid-path.json",
            Output = "./GeneratedInvalid",
            Type = "client"
        };

        var command = new OpenApiGenerateCommand();
        var context = new CommandContext(null!, null!, null!, null!);

        // Act
        var result = await Task.Run(() => command.Execute(context, settings));

        // Assert
        Assert.NotEqual(0, result);
        Assert.False(Directory.Exists(settings.Output));
    }

    [Fact]
    public async Task Should_Save_Generated_Code_In_Output_Directory()
    {
        // Arrange
        var settings = new OpenApiGenerateCommand.Settings
        {
            Input = "test-openapi.json",
            Output = "./CustomOutput",
            Type = "client",
            Namespace = "CustomNamespace"
        };

        var command = new OpenApiGenerateCommand();
        var context = new CommandContext(null!, null!, null!, null!);

        // Mock OpenAPI document
        File.WriteAllText(settings.Input, "{}");

        // Act
        var result = await Task.Run(() => command.Execute(context, settings));

        // Assert
        Assert.Equal(0, result);
        Assert.True(Directory.Exists(settings.Output));
        Assert.True(File.Exists(Path.Combine(settings.Output, "ApiClient.cs")));

        // Cleanup
        Directory.Delete(settings.Output, true);
        File.Delete(settings.Input);
    }
}
