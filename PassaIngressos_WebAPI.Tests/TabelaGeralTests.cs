using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PassaIngressos_WebAPI.Controllers;
using PassaIngressos_WebAPI.Database;
using PassaIngressos_WebAPI.Dto;
using PassaIngressos_WebAPI.Entity;

namespace PassaIngressos_WebAPI.Tests
{
    public class TabelaGeralTests
    {
        private readonly DbPassaIngressos _context;
        private readonly TabelaGeralController _controller;

        public TabelaGeralTests()
        {
            var options = new DbContextOptionsBuilder<DbPassaIngressos>()
                .UseInMemoryDatabase(databaseName: "DbTest")
                .Options;

            _context = new DbPassaIngressos(options);
            _controller = new TabelaGeralController(_context);
        }

        [Fact]
        public async Task ListarTabelasGerais_RetornaTabelas()
        {
            // Arrange
            _context.TabelasGerais.Add(new TabelaGeral { Tabela = "Tabela 1" });
            _context.TabelasGerais.Add(new TabelaGeral { Tabela = "Tabela 2" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.ListarTabelasGerais();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var tabelas = Assert.IsAssignableFrom<List<TabelaGeral>>(okResult.Value);
            Assert.Equal(2, tabelas.Count);
        }

        [Fact]
        public async Task PesquisarItensPorTabela_TabelaExistente_RetornaItens()
        {
            // Arrange
            var tabela = new TabelaGeral { Tabela = "Tabela de Exemplo" };
            _context.TabelasGerais.Add(tabela);
            _context.ItensTabelaGeral.Add(new ItemTabelaGeral { Descricao = "Item 1", Sigla = "I1", IdTabelaGeral = tabela.IdTabelaGeral });
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.PesquisarItensPorTabela("Tabela de Exemplo");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var itens = Assert.IsAssignableFrom<List<ItemTabelaGeral>>(okResult.Value);
            Assert.Single(itens);
            Assert.Equal("Item 1", itens[0].Descricao);
        }

        [Fact]
        public async Task AdicionarItem_TabelaExistente_RetornaItemAdicionado()
        {
            // Arrange
            var tabela = new TabelaGeral { Tabela = "Tabela para Adicionar" };
            _context.TabelasGerais.Add(tabela);
            await _context.SaveChangesAsync();

            var novoItemDto = new ItemTabelaGeralDto
            {
                Descricao = "Novo Item",
                Sigla = "NI",
                TabelaGeral = new TabelaGeralDto { IdTabelaGeral = tabela.IdTabelaGeral }
            };

            // Act
            var result = await _controller.AdicionarItem(novoItemDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var itemAdicionado = Assert.IsType<ItemTabelaGeral>(okResult.Value);
            Assert.Equal("Novo Item", itemAdicionado.Descricao);
            Assert.Equal(tabela.IdTabelaGeral, itemAdicionado.IdTabelaGeral);
        }

        [Fact]
        public async Task ExcluirItem_ItemExistente_RetornaSucesso()
        {
            // Arrange
            var item = new ItemTabelaGeral { Descricao = "Item para excluir", Sigla = "IE" };
            _context.ItensTabelaGeral.Add(item);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.ExcluirItem(item.IdItemTabelaGeral);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Item excluído com sucesso.", okResult.Value);
            Assert.Null(await _context.ItensTabelaGeral.FindAsync(item.IdItemTabelaGeral));
        }
    }
}