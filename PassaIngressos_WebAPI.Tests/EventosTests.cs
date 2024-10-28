using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PassaIngressos_WebAPI.Controllers;
using PassaIngressos_WebAPI.Database;
using PassaIngressos_WebAPI.Dto;
using PassaIngressos_WebAPI.Entity;

namespace PassaIngressos_WebAPI.Tests
{
    public class EventosTests
    {
        private readonly DbPassaIngressos _context;
        private readonly EventosController _controller;

        public EventosTests()
        {
            var options = new DbContextOptionsBuilder<DbPassaIngressos>()
                .UseInMemoryDatabase(databaseName: "DbTest")
                .Options;

            _context = new DbPassaIngressos(options);
            _controller = new EventosController(_context);
        }

        [Fact]
        public async Task CriarEvento_RetornaOk_QuandoEventoCriadoComSucesso()
        {
            // Arrange
            var eventoDto = new EventoDto
            {
                NomeEvento = "Show de Rock",
                LocalEvento = "Estádio",
                DataHoraEvento = DateTime.Now.AddMonths(1)
            };

            // Act
            var result = await _controller.CriarEvento(eventoDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var eventoCriado = Assert.IsAssignableFrom<Evento>(okResult.Value);
            Assert.Equal("Show de Rock", eventoCriado.NomeEvento);
        }

        [Fact]
        public async Task EditarEvento_RetornaOk_QuandoEventoEditadoComSucesso()
        {
            // Arrange
            var eventoExistente = new Evento
            {
                IdEvento = 6,
                NomeEvento = "Show de Rock",
                LocalEvento = "Rio de Janeiro",
                DataHoraEvento = DateTime.Now.AddMonths(1)
            };
            await _context.Eventos.AddAsync(eventoExistente);
            await _context.SaveChangesAsync();

            var eventoAtualizado = new EventoDto
            {
                NomeEvento = "Show de Pop",
                LocalEvento = "Arena",
                DataHoraEvento = DateTime.Now.AddMonths(2)
            };

            // Act
            var result = await _controller.EditarEvento(6, eventoAtualizado);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Show de Pop", eventoExistente.NomeEvento);
            Assert.Equal("Arena", eventoExistente.LocalEvento);
        }

        [Fact]
        public async Task ExcluirEvento_RetornaOk_QuandoEventoExcluidoComSucesso()
        {
            // Arrange
            var eventoExistente = new Evento
            {
                IdEvento = 3,
                NomeEvento = "Show de Rock",
                LocalEvento = "Rio de Janeiro"
            };
            await _context.Eventos.AddAsync(eventoExistente);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.ExcluirEvento(3);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Evento excluído com sucesso.", (result as OkObjectResult).Value);
        }

        [Fact]
        public async Task ListarEventos_RetornaOk_QuandoExistemEventos()
        {
            // Arrange
            await _controller.ExcluirEvento(1);

            var eventos = new List<Evento>
            {
                new Evento { IdEvento = 2, NomeEvento = "Show de Rock", LocalEvento = "Rio de Janeiro" },
                new Evento { IdEvento = 3, NomeEvento = "Show de Jazz", LocalEvento = "São Paulo" }
            };

            await _context.Eventos.AddRangeAsync(eventos);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.ListarEventos();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var listaEventos = Assert.IsType<List<Evento>>(okResult.Value);
            Assert.Equal(2, listaEventos.Count);
        }

        [Fact]
        public async Task PesquisarEvento_RetornaOk_QuandoEventoEncontrado()
        {
            // Arrange
            var evento = new Evento
            {
                IdEvento = 5,
                NomeEvento = "Show de Rock",
                LocalEvento = "Rio de Janeiro"
            };

            await _context.Eventos.AddAsync(evento);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.PesquisarEvento(5);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var eventoRetornado = Assert.IsType<Evento>((result as OkObjectResult).Value);
            Assert.Equal(5, eventoRetornado.IdEvento);
        }
    }
}