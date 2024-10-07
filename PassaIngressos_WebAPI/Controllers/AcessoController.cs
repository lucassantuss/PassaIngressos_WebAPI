using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PassaIngressos_WebAPI.Database;
using PassaIngressos_WebAPI.Entity;
using PassaIngressos_WebAPI.Dto;

namespace PassaIngressos_WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AcessoController : ControllerBase
    {
        #region Contexto e Variáveis
        private readonly DbPassaIngressos _dbPassaIngressos;

        public AcessoController(DbPassaIngressos _context)
        {
            _dbPassaIngressos = _context;
        }

        #endregion

        #region Perfil

        // Método para listar todos os Perfis
        [HttpGet("ListarPerfis")]
        public async Task<IActionResult> ListarPerfis()
        {
            var listaPerfis = await _dbPassaIngressos.Perfis.ToListAsync();

            if (listaPerfis == null)
                return NotFound("Não foi encontrado nenhum perfil.");

            return Ok(listaPerfis);
        }

        // Método para criar Perfil
        [HttpPost("CriarPerfil")]
        public async Task<IActionResult> CriarPerfil([FromBody] PerfilDto perfilDto)
        {
            var perfil = new Perfil
            {
                NomePerfil = perfilDto.NomePerfil,
                DescricaoPerfil = perfilDto.DescricaoPerfil
            };

            _dbPassaIngressos.Perfis.Add(perfil);
            await _dbPassaIngressos.SaveChangesAsync();

            return Ok(perfil);
        }

        // Método para alterar Perfil
        [HttpPut("AlterarPerfil/{idPerfil}")]
        public async Task<IActionResult> AlterarPerfil(int idPerfil, [FromBody] PerfilDto perfilAtualizado)
        {
            var perfil = await _dbPassaIngressos.Perfis.FindAsync(idPerfil);

            if (perfil == null)
                return NotFound("Perfil não encontrado.");

            perfil.NomePerfil = perfilAtualizado.NomePerfil;
            perfil.DescricaoPerfil = perfilAtualizado.DescricaoPerfil;

            await _dbPassaIngressos.SaveChangesAsync();

            return Ok(perfil);
        }

        // Método para remover Perfil
        [HttpDelete("RemoverPerfil/{idPerfil}")]
        public async Task<IActionResult> RemoverPerfil(int idPerfil)
        {
            var perfil = await _dbPassaIngressos.Perfis.FindAsync(idPerfil);

            if (perfil == null)
                return NotFound("Perfil não encontrado.");

            _dbPassaIngressos.Perfis.Remove(perfil);
            await _dbPassaIngressos.SaveChangesAsync();

            return Ok("Perfil removido com sucesso.");
        }

        // Método para pesquisar todos os usuários do Perfil
        [HttpGet("UsuariosDoPerfil/{idPerfil}")]
        public async Task<IActionResult> UsuariosDoPerfil(int idPerfil)
        {
            var listaIdsUsuarios = await _dbPassaIngressos.UsuarioPerfis
                                        .Where(up => up.IdPerfil == idPerfil)
                                        .Select(up => up.IdUsuario)
                                        .ToListAsync();

            if (listaIdsUsuarios == null || !listaIdsUsuarios.Any())
                return NotFound("Nenhum usuário encontrado para esse perfil.");

            var usuarios = await _dbPassaIngressos.Usuarios
                                 .Include(u => u.Pessoa)
                                 .Include(u => u.Pessoa.Sexo)
                                 .Include(u => u.Pessoa.Sexo.TabelaGeral)
                                 .Include(u => u.Pessoa.ArquivoFoto)
                                 .Where(xs => listaIdsUsuarios.Contains(xs.IdUsuario))
                                 .ToListAsync();

            // Mapear para o DTO
            var usuarioDtos = usuarios.Select(u => new UsuariosPerfilDto
            {
                IdUsuario = u.IdUsuario,
                Login = u.Login,

                Pessoa = new PessoaDto
                {
                    IdPessoa = u.Pessoa.IdPessoa,
                    Nome = u.Pessoa.Nome,
                    DataNascimento = u.Pessoa.DataNascimento,
                    CPF = u.Pessoa.CPF,
                    RG = u.Pessoa.RG,

                    Sexo = u.Pessoa.Sexo != null ? new ItemTabelaGeralDto
                    {
                        IdItemTabelaGeral = u.Pessoa.Sexo.IdItemTabelaGeral,
                        Sigla = u.Pessoa.Sexo.Sigla,
                        Descricao = u.Pessoa.Sexo.Descricao,

                        TabelaGeral = u.Pessoa.Sexo.TabelaGeral != null ? new TabelaGeralDto
                        {
                            IdTabelaGeral = u.Pessoa.Sexo.TabelaGeral.IdTabelaGeral,
                            Tabela = u.Pessoa.Sexo.TabelaGeral.Tabela
                        } : new TabelaGeralDto()
                    } : new ItemTabelaGeralDto(),

                    ArquivoFoto = u.Pessoa.ArquivoFoto != null ? new ArquivoDto
                    {
                        IdArquivo = u.Pessoa.ArquivoFoto.IdArquivo,
                        ConteudoArquivo = u.Pessoa.ArquivoFoto.ConteudoArquivo,
                        ContentType = u.Pessoa.ArquivoFoto.ContentType,
                        Extensao = u.Pessoa.ArquivoFoto.Extensao,
                        Nome = u.Pessoa.ArquivoFoto.Nome,
                    } : new ArquivoDto(),
                }
            }).ToList();

            return Ok(usuarioDtos);
        }

        #endregion

        #region Usuário

        // Método para criar usuário
        [HttpPost("CriarUsuario")]
        public async Task<IActionResult> CriarUsuario([FromBody] UsuarioDto usuarioDto)
        {
            // Cria Pessoa que será associada ao Usuário
            var pessoa = new Pessoa()
            {
                Nome = usuarioDto.NomePessoa,
                CPF = usuarioDto.CPF,
                RG = usuarioDto.RG,
                IdArquivoFoto = usuarioDto.IdArquivoFoto ?? 0,
                IdTgSexo = usuarioDto.IdTgSexo ?? 0
            };

            _dbPassaIngressos.Pessoas.Add(pessoa);
            await _dbPassaIngressos.SaveChangesAsync();

            // Cria Usuário
            var usuario = new Usuario()
            {
                Login = usuarioDto.Login,
                Senha = BCrypt.Net.BCrypt.HashPassword(usuarioDto.Senha), // Criptografa a senha
                IdPessoa = pessoa.IdPessoa
            };

            _dbPassaIngressos.Usuarios.Add(usuario);
            await _dbPassaIngressos.SaveChangesAsync();

            return Ok(usuario);
        }

        // Método para redefinir senha
        [HttpPut("RedefinirSenha/{idUsuario}")]
        public async Task<IActionResult> RedefinirSenha(int idUsuario, [FromBody] RedefineSenhaDto redefineSenhaDto)
        {
            var usuario = await _dbPassaIngressos.Usuarios.FindAsync(idUsuario);

            if (usuario == null)
                return NotFound("Usuário não encontrado.");

            usuario.Senha = BCrypt.Net.BCrypt.HashPassword(redefineSenhaDto.Senha);
            await _dbPassaIngressos.SaveChangesAsync();

            return Ok("Senha redefinida com sucesso.");
        }

        // Método para validar usuário e realizar o login
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            // TODO Verificar com a senha criptografada...
            var usuario = await _dbPassaIngressos.Usuarios
                                .SingleOrDefaultAsync(u => u.Login == loginDto.Login &&
                                                           u.Senha == loginDto.Senha);

            if (usuario == null)
                return Unauthorized("Login ou senha inválidos.");

            return Ok("Login realizado com sucesso.");
        }

        // Método para pesquisar usuário
        [HttpGet("PesquisarUsuario/{login}")]
        public async Task<IActionResult> PesquisarUsuario(string login)
        {
            var usuario = await _dbPassaIngressos.Usuarios
                                .Include(u => u.Pessoa)
                                .SingleOrDefaultAsync(u => u.Login == login);

            if (usuario == null)
                return NotFound("Usuário não encontrado.");

            return Ok(usuario);
        }

        // Método para excluir conta
        [HttpDelete("ExcluirConta/{idUsuario}")]
        public async Task<IActionResult> ExcluirConta(int idUsuario)
        {
            var usuario = await _dbPassaIngressos.Usuarios.FindAsync(idUsuario);

            if (usuario == null)
                return NotFound("Usuário não encontrado.");

            _dbPassaIngressos.Usuarios.Remove(usuario);
            await _dbPassaIngressos.SaveChangesAsync();

            return Ok("Conta excluída com sucesso.");
        }

        // Método para pesquisar Perfis associados ao IdUsuario
        [HttpGet("PerfisDoUsuario/{idUsuario}")]
        public async Task<IActionResult> PerfisDoUsuario(int idUsuario)
        {
            var listaIdsPerfis = await _dbPassaIngressos.UsuarioPerfis
                                .Where(up => up.IdUsuario == idUsuario)
                                .Select(up => up.IdPerfil)
                                .ToListAsync();

            if (listaIdsPerfis == null || !listaIdsPerfis.Any())
                return NotFound("Nenhum perfil associado a este usuário.");

            var perfis = await _dbPassaIngressos.Perfis
                                 .Where(xs => listaIdsPerfis.Contains(xs.IdPerfil))
                                 .ToListAsync();

            return Ok(perfis);
        }

        // Método para adicionar Perfil ao IdUsuario
        [HttpPost("AdicionarPerfilAoUsuario/{idUsuario}")]
        public async Task<IActionResult> AdicionarPerfilAoUsuario(int idUsuario, [FromBody] UsuarioAdicionaPerfilDto adicionaPerfilDto)
        {
            var usuario = await _dbPassaIngressos.Usuarios.FindAsync(idUsuario);
            var perfil = await _dbPassaIngressos.Perfis.FindAsync(adicionaPerfilDto.IdPerfil);

            if (usuario == null)
                return NotFound("Usuário não encontrado.");

            if (perfil == null)
                return NotFound("Perfil não encontrado.");

            var usuarioPerfil = new UsuarioPerfil {
                IdUsuario = idUsuario,
                IdPerfil = adicionaPerfilDto.IdPerfil
            };

            var usuarioPerfilExistente = await _dbPassaIngressos.UsuarioPerfis
                                               .Where(xs => xs.IdUsuario == usuarioPerfil.IdUsuario)
                                               .Where(xs => xs.IdPerfil == usuarioPerfil.IdPerfil)
                                               .FirstOrDefaultAsync();

            if (usuarioPerfilExistente != null)
                return Unauthorized("Perfil já está associado ao usuário informado.");

            _dbPassaIngressos.UsuarioPerfis.Add(usuarioPerfil);
            await _dbPassaIngressos.SaveChangesAsync();

            return Ok("Perfil adicionado ao usuário com sucesso.");
        }

        #endregion
    }
}