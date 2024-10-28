using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PassaIngressos_WebAPI.Controllers;
using PassaIngressos_WebAPI.Database;
using PassaIngressos_WebAPI.Dto;
using PassaIngressos_WebAPI.Entity;

namespace PassaIngressos_WebAPI.Tests
{
    public class FeedbackTests
    {
        private readonly DbPassaIngressos _context;
        private readonly FeedbackController _controller;

        public FeedbackTests()
        {
            var options = new DbContextOptionsBuilder<DbPassaIngressos>()
                .UseInMemoryDatabase(databaseName: "DbTest")
                .Options;

            _context = new DbPassaIngressos(options);
            _controller = new FeedbackController(_context);
        }

        [Fact]
        public async Task CriarFeedback_RetornaFeedbackCriado_QuandoFeedbackForValido()
        {
            // Arrange
            var feedbackDto = new FeedbackDto
            {
                DescricaoFeedback = "Ótimo serviço!",
                IdPessoa = 1
            };

            // Act
            var result = await _controller.CriarFeedback(feedbackDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var feedback = Assert.IsType<Feedback>(okResult.Value);
            Assert.Equal("Ótimo serviço!", feedback.DescricaoFeedback);
        }

        [Fact]
        public async Task ExcluirFeedback_FeedbackExistente_RetornaSucesso()
        {
            // Arrange
            var feedback = new Feedback { DescricaoFeedback = "Feedback para ser excluído", IdPessoa = 1 };
            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.ExcluirFeedback(feedback.IdFeedback);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Feedback excluído com sucesso.", okResult.Value);
            Assert.Null(await _context.Feedbacks.FindAsync(feedback.IdFeedback));
        }
    }
}