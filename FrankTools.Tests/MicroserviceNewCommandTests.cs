using FrankTools.Cli.Commands;
using Spectre.Console.Cli;
using Moq;
using System.Runtime.CompilerServices;

namespace FrankTools.Tests;
public class MicroserviceNewCommandTests
{
    private readonly Mock<CommandContext> _commandContextMock;
    public MicroserviceNewCommandTests()
    {
        this._commandContextMock = new Mock<CommandContext>();
    }

    [Fact]
    public void Should_Create_Microservice_Structure()
    {
        // Arrange
        var settings = new MicroserviceNewCommand.Settings
        {
            ServiceName = "TestMicroservice",
            UseMongo = false,
            UseSql = false
        };
        var cmd = new MicroserviceNewCommand();

        // Act
        var result = cmd.Execute(_commandContextMock.Object, settings);

        // Assert
        Assert.Equal(0, result);
        Assert.True(Directory.Exists("TestMicroservice"));
        Assert.True(Directory.Exists("TestMicroservice/TestMicroservice.API"));
        Assert.True(Directory.Exists("TestMicroservice/TestMicroservice.Application"));
        Assert.True(Directory.Exists("TestMicroservice/TestMicroservice.Domain"));
        Assert.True(Directory.Exists("TestMicroservice/TestMicroservice.Infrastructure"));
        Assert.True(File.Exists("TestMicroservice/TestMicroservice.API/Program.cs"));
        Assert.True(File.Exists("TestMicroservice/TestMicroservice.API/TestMicroservice.API.csproj"));
        Assert.True(File.Exists("TestMicroservice/TestMicroservice.API/appsettings.json"));
        Assert.True(File.Exists("TestMicroservice/Dockerfile"));
        Assert.True(File.Exists("TestMicroservice/docker-compose.yml"));
        Assert.True(File.Exists("TestMicroservice/.gitignore"));
        Assert.True(File.Exists("TestMicroservice/README.md"));
        Assert.True(File.Exists("TestMicroservice/TestMicroservice.sln"));

        // Cleanup
        Directory.Delete("TestMicroservice", true);
    }

    [Fact]
    public void Should_Return_Error_If_Directory_Exists()
    {
        // Arrange
        var settings = new MicroserviceNewCommand.Settings
        {
            ServiceName = "ExistingMicroservice"
        };
        var cmd = new MicroserviceNewCommand();

        Directory.CreateDirectory("ExistingMicroservice");

        // Act
        var result = cmd.Execute(_commandContextMock.Object, settings);

        // Assert
        Assert.Equal(-1, result);

        // Cleanup
        Directory.Delete("ExistingMicroservice", true);
    }

    [Fact]
    public void Should_Create_Program_File_With_Mongo_Setup()
    {
        // Arrange
        var settings = new MicroserviceNewCommand.Settings
        {
            ServiceName = "MongoMicroservice",
            UseMongo = true,
            UseSql = false
        };
        var cmd = new MicroserviceNewCommand();

        // Act
        var result = cmd.Execute(_commandContextMock.Object, settings);

        // Assert
        Assert.Equal(0, result);
        var programContent = File.ReadAllText("MongoMicroservice/MongoMicroservice.API/Program.cs");
        Assert.Contains("builder.Services.AddSingleton<IMongoClient>", programContent);

        // Cleanup
        Directory.Delete("MongoMicroservice", true);
    }

    [Fact]
    public void Should_Create_Program_File_With_SQL_Setup()
    {
        // Arrange
        var settings = new MicroserviceNewCommand.Settings
        {
            ServiceName = "SqlMicroservice",
            UseMongo = false,
            UseSql = true
        };
        var cmd = new MicroserviceNewCommand();

        // Act
        var result = cmd.Execute(_commandContextMock.Object, settings);

        // Assert
        Assert.Equal(0, result);
        var programContent = File.ReadAllText("SqlMicroservice/SqlMicroservice.API/Program.cs");
        Assert.Contains("builder.Services.AddDb_commandContextMock.Object<MyDb_commandContextMock.Object>", programContent);

        // Cleanup
        Directory.Delete("SqlMicroservice", true);
    }
}
