using NUnit.Framework;
using FluentAssertions;
using Moq;
using RefitClient.Console.Clients;
using RefitClient.Console.Models;
using RefitClient.Console.Services;
using Refit;

namespace RefitClient.Tests;

[TestFixture]
public class TenistaHttpServiceTests
{
    private Mock<ITenistaApiClient> _mockClient = null!;
    private TenistaHttpService _service = null!;

    [SetUp]
    public void Setup()
    {
        _mockClient = new Mock<ITenistaApiClient>();
        _service = new TenistaHttpService(_mockClient.Object);
    }

    [Test]
    public async Task GetAllTenistasAsync_DeberiaRetornarTenistas()
    {
        // Arrange
        var tenistas = new List<Tenista>
        {
            new() { Id = 1, Nombre = "Nadal", Ranking = 1, Pais = "España", Titulos = 22 },
            new() { Id = 2, Nombre = "Federer", Ranking = 2, Pais = "Suiza", Titulos = 20 }
        };
        
        _mockClient.Setup(c => c.GetTenistasAsync())
            .ReturnsAsync(tenistas);

        // Act
        var result = await _service.GetAllTenistasAsync();

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.StatusCode.Should().Be(200);
    }

    [Test]
    public async Task GetTenistaByIdAsync_ConIdValido_DeberiaRetornarTenista()
    {
        // Arrange
        var tenista = new Tenista { Id = 1, Nombre = "Nadal", Ranking = 1, Pais = "España", Titulos = 22 };
        
        _mockClient.Setup(c => c.GetTenistaAsync(1))
            .ReturnsAsync(tenista);

        // Act
        var result = await _service.GetTenistaByIdAsync(1);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Nombre.Should().Be("Nadal");
    }

    [Test]
    public async Task GetTenistaByIdAsync_ConIdInvalido_DeberiaRetornarError()
    {
        // Arrange
        _mockClient.Setup(c => c.GetTenistaAsync(999))
            .ThrowsAsync(new Exception("Not found"));

        // Act
        var result = await _service.GetTenistaByIdAsync(999);

        // Assert
        result.Success.Should().BeFalse();
    }

    [Test]
    public async Task CreateTenistaAsync_ConDatosValidos_DeberiaCrearTenista()
    {
        // Arrange
        var nuevoTenista = new Tenista { Id = 0, Nombre = "Alcaraz", Ranking = 1, Pais = "España", Titulos = 2 };
        var createdTenista = new Tenista { Id = 3, Nombre = "Alcaraz", Ranking = 1, Pais = "España", Titulos = 2 };
        
        _mockClient.Setup(c => c.CreateTenistaAsync(nuevoTenista))
            .ReturnsAsync(createdTenista);

        // Act
        var result = await _service.CreateTenistaAsync(nuevoTenista);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(3);
    }

    [Test]
    public async Task SearchTenistasAsync_ConNombre_DeberiaRetornarResultados()
    {
        // Arrange
        var tenistas = new List<Tenista>
        {
            new() { Id = 1, Nombre = "Rafael Nadal", Ranking = 1, Pais = "España", Titulos = 22 }
        };
        
        _mockClient.Setup(c => c.SearchTenistasAsync("Nadal", null))
            .ReturnsAsync(tenistas);

        // Act
        var result = await _service.SearchTenistasAsync("Nadal");

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().HaveCount(1);
        result.Data![0].Nombre.Should().Contain("Nadal");
    }
}

[TestFixture]
public class ApiResponseTests
{
    [Test]
    public void SuccessResponse_DeberiaCrearRespuestaExitosa()
    {
        // Arrange & Act
        var response = TenistaApiResponse<string>.SuccessResponse("test data");

        // Assert
        response.Success.Should().BeTrue();
        response.Data.Should().Be("test data");
        response.StatusCode.Should().Be(200);
        response.Message.Should().Be("OK");
    }

    [Test]
    public void ErrorResponse_DeberiaCrearRespuestaError()
    {
        // Arrange & Act
        var response = TenistaApiResponse<string>.ErrorResponse("Error message", 500);

        // Assert
        response.Success.Should().BeFalse();
        response.Data.Should().BeNull();
        response.StatusCode.Should().Be(500);
        response.Message.Should().Be("Error message");
    }
}
