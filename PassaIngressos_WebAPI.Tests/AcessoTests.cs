using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PassaIngressos_WebAPI.Controllers;
using PassaIngressos_WebAPI.Database;
using PassaIngressos_WebAPI.Dto;
using PassaIngressos_WebAPI.Entity;

namespace PassaIngressos_WebAPI.Tests
{
    public class AcessoTests
    {
        private readonly DbPassaIngressos _context;
        private readonly AcessoController _controller;
        private readonly IConfiguration _configuration;

        public AcessoTests()
        {
            var configurationBuilder = new ConfigurationBuilder()
                 .AddEnvironmentVariables();

            _configuration = configurationBuilder.Build();

            var keyJWT = _configuration["Jwt_ChaveSecreta_PassaIngressos"];

            if (string.IsNullOrEmpty(keyJWT))
            {
                throw new InvalidOperationException("A chave JWT não foi encontrada.");
            }

            var options = new DbContextOptionsBuilder<DbPassaIngressos>()
                .UseInMemoryDatabase(databaseName: "DbTest")
                .Options;

            _context = new DbPassaIngressos(options);
            _controller = new AcessoController(_configuration, _context);
        }

        [Fact]
        public async Task CriarUsuario_RetornaOk_QuandoUsuarioForCriado()
        {
            // Arrange
            var usuarioDto = new UsuarioDto 
            { 
                Login = "newuser", 
                Senha = "password", 

                NomePessoa = "New User",
                CPF = "132.730.760-03",
                RG = "12.345.678-9",
                DataNascimento = DateTime.Now.AddYears(-20),
                IdArquivoFoto = 1,
                IdTgSexo = 1,
            };

            // Act
            var result = await _controller.CriarUsuario(usuarioDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Usuário criado com sucesso!", okResult.Value);
        }

        [Fact]
        public async Task RemoverUsuario_RetornaOk_QuandoUsuarioForRemovido()
        {
            // Arrange
            var usuarioDto = new UsuarioDto
            {
                Login = "newuser",
                Senha = "password",

                NomePessoa = "New User",
                CPF = "132.730.760-03",
                RG = "12.345.678-9",
                DataNascimento = DateTime.Now.AddYears(-20),
                IdArquivoFoto = 1,
                IdTgSexo = 1,
            };

            // Act
            await _controller.CriarUsuario(usuarioDto);
            var result = await _controller.ExcluirConta(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Conta excluída com sucesso.", okResult.Value);
        }

        [Fact]
        public async Task RemoverUsuario_RetornaNaoEncontrado_QuandoUsuarioNaoExistir()
        {
            // Act
            var result = await _controller.ExcluirConta(1);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.Equal("Usuário não encontrado.", notFoundResult.Value);
        }

        [Fact]
        public async Task CriarPerfil_RetornaOk_QuandoPerfilForCriado()
        {
            // Arrange
            var perfilDto = new PerfilDto { NomePerfil = "NovoPerfil", DescricaoPerfil = "DescricaoDoPerfil" };

            // Act
            var result = await _controller.CriarPerfil(perfilDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var createdPerfil = Assert.IsAssignableFrom<Perfil>(okResult.Value);
            Assert.Equal("NovoPerfil", createdPerfil.NomePerfil);
        }

        [Fact]
        public async Task RemoverPerfil_RetornaOk_QuandoPerfilForRemovido()
        {
            // Arrange
            var perfil = new Perfil 
            { 
                IdPerfil = 1, 
                NomePerfil = "PerfilParaRemover", 
                DescricaoPerfil = "Perfil a ser excluído" 
            };
            
            await _context.Perfis.AddAsync(perfil);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.RemoverPerfil(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Perfil removido com sucesso.", okResult.Value);
        }

        [Fact]
        public async Task RemoverPerfil_RetornaNaoEncontrado_QuandoPerfilNaoExistir()
        {
            // Act
            var result = await _controller.RemoverPerfil(1);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.Equal("Perfil não encontrado.", notFoundResult.Value);
        }
    }
}