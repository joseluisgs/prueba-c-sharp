using NUnit.Framework;
using FluentAssertions;
using System.Reactive.Linq;
using TenistasReactive.Console.Models;
using TenistasReactive.Console.Services;
using TenistasReactive.Console.Streams;
using TenistasReactive.Console.ErrorHandling;

namespace TenistasReactive.Tests;

[TestFixture]
public class TenistaReactiveServiceTests
{
    private TenistaReactiveService _service = null!;

    [SetUp]
    public void Setup()
    {
        _service = new TenistaReactiveService();
    }

    [Test]
    public async Task GetTenistasColObservable_DeberiaEmitirTodosLosTenistas()
    {
        // Arrange
        var tenistas = new List<Tenista>();

        // Act
        await _service.GetTenistasColObservable()
            .Do(t => tenistas.Add(t))
            .LastOrDefaultAsync();

        // Assert
        tenistas.Should().HaveCount(5);
        tenistas.Should().Contain(t => t.Nombre == "Rafael Nadal");
    }

    [Test]
    public async Task GetTenistasByRankingTop_DeberiaFiltrarCorrectamente()
    {
        // Arrange
        var tenistas = new List<Tenista>();

        // Act
        await _service.GetTenistasByRankingTop(3)
            .Do(t => tenistas.Add(t))
            .LastOrDefaultAsync();

        // Assert
        tenistas.Should().NotBeEmpty();
        tenistas.Should().OnlyContain(t => t.Ranking <= 3);
    }

    [Test]
    public async Task MergeTenistaStreams_DeberiaCombinarStreams()
    {
        // Arrange
        var tenistas = new List<Tenista>();
        var stream1 = Observable.Return(new Tenista { Id = 1, Nombre = "T1", Ranking = 1, Pais = "A", Titulos = 1 });
        var stream2 = Observable.Return(new Tenista { Id = 2, Nombre = "T2", Ranking = 2, Pais = "B", Titulos = 2 });

        // Act
        await _service.MergeTenistaStreams(stream1, stream2)
            .Do(t => tenistas.Add(t))
            .LastOrDefaultAsync();

        // Assert
        tenistas.Should().HaveCount(2);
    }

    [Test]
    public async Task GetTenistasWithBackPressure_DeberiaAgrupar()
    {
        // Arrange
        var buffers = new List<IList<Tenista>>();

        // Act
        await _service.GetTenistasWithBackPressure(TimeSpan.FromMilliseconds(500), 3)
            .Do(buffer => buffers.Add(buffer))
            .LastOrDefaultAsync();

        // Assert
        buffers.Should().NotBeEmpty();
        buffers.Sum(b => b.Count).Should().Be(5);
    }
}

[TestFixture]
public class TenistaSubjectServiceTests
{
    private TenistaSubjectService _service = null!;

    [SetUp]
    public void Setup()
    {
        _service = new TenistaSubjectService();
    }

    [Test]
    public void PublishStream_DeberiaEmitirSoloEventosFuturos()
    {
        // Arrange
        var received = new List<Tenista>();
        var tenista1 = new Tenista { Id = 1, Nombre = "T1", Ranking = 1, Pais = "A", Titulos = 1 };
        var tenista2 = new Tenista { Id = 2, Nombre = "T2", Ranking = 2, Pais = "B", Titulos = 2 };

        // Act
        _service.PublishTenista(tenista1); // Antes de subscribe
        _service.PublishStream.Subscribe(t => received.Add(t));
        _service.PublishTenista(tenista2); // Después de subscribe

        // Assert
        received.Should().HaveCount(1);
        received[0].Should().Be(tenista2);
    }

    [Test]
    public void BehaviorStream_DeberiaEmitirUltimoValor()
    {
        // Arrange
        var received = new List<Tenista>();
        var tenista = new Tenista { Id = 1, Nombre = "T1", Ranking = 1, Pais = "A", Titulos = 1 };

        // Act
        _service.UpdateBehaviorTenista(tenista);
        _service.BehaviorStream.Subscribe(t => received.Add(t));

        // Assert
        received.Should().HaveCount(1);
        received[0].Should().Be(tenista);
    }

    [Test]
    public void CurrentTenista_DeberiaRetornarUltimoValor()
    {
        // Arrange
        var tenista = new Tenista { Id = 1, Nombre = "T1", Ranking = 1, Pais = "A", Titulos = 1 };

        // Act
        _service.UpdateBehaviorTenista(tenista);

        // Assert
        _service.CurrentTenista.Should().Be(tenista);
    }
}

[TestFixture]
public class ReactiveErrorHandlerTests
{
    [Test]
    public async Task WithFallback_DeberiaUsarValorPorDefecto()
    {
        // Arrange
        var errorSource = Observable.Throw<Tenista>(new Exception("Test error"));
        var fallback = new Tenista { Id = -1, Nombre = "Fallback", Ranking = 999, Pais = "N/A", Titulos = 0 };

        // Act
        var result = await ReactiveErrorHandler.WithFallback(errorSource, fallback)
            .FirstOrDefaultAsync();

        // Assert
        result.Should().Be(fallback);
    }

    [Test]
    public async Task WithTimeout_DeberiausarFallbackEnTimeout()
    {
        // Arrange
        var slowSource = Observable.Timer(TimeSpan.FromSeconds(10))
            .Select(_ => new Tenista { Id = 1, Nombre = "Slow", Ranking = 1, Pais = "Test", Titulos = 0 });
        var fallback = new Tenista { Id = -1, Nombre = "Fallback", Ranking = 999, Pais = "N/A", Titulos = 0 };

        // Act
        var result = await ReactiveErrorHandler.WithTimeout(slowSource, TimeSpan.FromMilliseconds(100), fallback)
            .FirstOrDefaultAsync();

        // Assert
        result.Should().Be(fallback);
    }
}
