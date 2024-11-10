using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PassaIngressos_WebAPI.Database;
using PassaIngressos_WebAPI.Dto;
using PassaIngressos_WebAPI.Entity;

namespace PassaIngressos_WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ArquivoController : ControllerBase
    {
        #region Contexto e Variáveis

        private readonly DbPassaIngressos _dbPassaIngressos;

        public ArquivoController(DbPassaIngressos _context)
        {
            _dbPassaIngressos = _context;
        }

        #endregion

        #region Arquivo

        [AllowAnonymous]
        [HttpGet("PesquisarArquivoPorId/{id}")]
        public async Task<IActionResult> PesquisarArquivoPorId(int id)
        {
            if (id <= 0)
                return BadRequest("ID inválido.");

            var arquivo = await _dbPassaIngressos.Arquivos.FindAsync(id);

            if (arquivo == null)
                return NotFound("Arquivo não encontrado.");

            return File(arquivo.ConteudoArquivo, arquivo.ContentType);
        }

        [AllowAnonymous]
        [HttpPost("SalvarArquivo")]
        public async Task<IActionResult> SalvarArquivo([FromBody] ArquivoDto arquivoDto)
        {
            if (arquivoDto == null)
                return BadRequest("Dados do arquivo são obrigatórios.");

            if (arquivoDto.ConteudoArquivo == null || arquivoDto.ConteudoArquivo.Length == 0)
                return BadRequest("Conteúdo do arquivo é obrigatório.");

            if (string.IsNullOrWhiteSpace(arquivoDto.ContentType))
                return BadRequest("Tipo de conteúdo é obrigatório.");

            if (string.IsNullOrWhiteSpace(arquivoDto.Extensao))
                return BadRequest("Extensão do arquivo é obrigatória.");

            if (string.IsNullOrWhiteSpace(arquivoDto.Nome))
                return BadRequest("Nome do arquivo é obrigatório.");

            var novoArquivo = new Arquivo
            {
                ConteudoArquivo = arquivoDto.ConteudoArquivo,
                ContentType = arquivoDto.ContentType,
                Extensao = arquivoDto.Extensao,
                Nome = arquivoDto.Nome
            };

            _dbPassaIngressos.Arquivos.Add(novoArquivo);
            await _dbPassaIngressos.SaveChangesAsync();

            return Ok(novoArquivo.IdArquivo);
        }

        [Authorize]
        [HttpDelete("ExcluirArquivo/{id}")]
        public async Task<IActionResult> ExcluirArquivo(int id)
        {
            if (id <= 0)
                return BadRequest("ID inválido.");

            var arquivo = await _dbPassaIngressos.Arquivos.FindAsync(id);

            if (arquivo == null)
                return NotFound("Arquivo não encontrado.");

            _dbPassaIngressos.Arquivos.Remove(arquivo);
            await _dbPassaIngressos.SaveChangesAsync();

            return Ok("Arquivo excluído com sucesso.");
        }

        #endregion
    }
}