using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PassaIngressos_WebAPI.Controllers;
using PassaIngressos_WebAPI.Database;
using PassaIngressos_WebAPI.Dto;
using PassaIngressos_WebAPI.Entity;

namespace PassaIngressos_WebAPI.Tests
{
    public class ArquivoTests
    {
        private readonly DbPassaIngressos _context;
        private readonly ArquivoController _controller;

        public ArquivoTests()
        {
            var options = new DbContextOptionsBuilder<DbPassaIngressos>()
                .UseInMemoryDatabase(databaseName: "DbTest")
                .Options;

            _context = new DbPassaIngressos(options);
            _controller = new ArquivoController(_context);
        }

        [Fact]
        public async Task PesquisarArquivoPorId_RetornaArquivo_QuandoArquivoExistir()
        {
            // Arrange
            var arquivo = new Arquivo
            {
                ConteudoArquivo = new byte[] { 1, 2, 3 },
                ContentType = "application/pdf",
                Extensao = ".pdf",
                Nome = "Teste.pdf"
            };
            _context.Arquivos.Add(arquivo);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.PesquisarArquivoPorId(arquivo.IdArquivo);

            // Assert
            var fileResult = Assert.IsAssignableFrom<FileResult>(result);
            Assert.Equal("application/pdf", fileResult.ContentType);
        }

        [Fact]
        public async Task PesquisarArquivoPorId_RetornaNotFound_QuandoArquivoNaoEncontrado()
        {
            // Act
            var result = await _controller.PesquisarArquivoPorId(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task SalvarArquivo_RetornaIdArquivo_QuandoArquivoValido()
        {
            // Arrange
            var arquivoDto = new ArquivoDto
            {
                ConteudoArquivo = new byte[] { 1, 2, 3 },
                ContentType = "application/pdf",
                Extensao = ".pdf",
                Nome = "Teste.pdf"
            };

            // Act
            var result = await _controller.SalvarArquivo(arquivoDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var idArquivo = Assert.IsType<int>(okResult.Value);
            Assert.True(idArquivo > 0);
        }

        [Fact]
        public async Task ExcluirArquivo_RetornaSucesso_QuandoArquivoForExcluido()
        {
            // Arrange
            var arquivo = new Arquivo
            {
                ConteudoArquivo = new byte[] { 1, 2, 3 },
                ContentType = "application/pdf",
                Extensao = ".pdf",
                Nome = "Teste.pdf"
            };
            _context.Arquivos.Add(arquivo);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.ExcluirArquivo(arquivo.IdArquivo);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Arquivo excluído com sucesso.", ((OkObjectResult)result).Value);
        }
    }
}