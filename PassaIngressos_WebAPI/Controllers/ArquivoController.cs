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

        [HttpGet("PesquisarArquivoPorId/{id}")]
        public async Task<IActionResult> PesquisarArquivoPorId(int id)
        {
            var arquivo = await _dbPassaIngressos.Arquivos.FindAsync(id);

            if (arquivo == null)
                return NotFound();

            return File(arquivo.ConteudoArquivo, arquivo.ContentType);
        }

        [HttpPost("SalvarArquivo")]
        public async Task<IActionResult> SalvarArquivo([FromBody] ArquivoDto arquivoDto)
        {
            if (arquivoDto == null || arquivoDto.ConteudoArquivo == null)
                return BadRequest("Arquivo inválido.");

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

        [HttpDelete("ExcluirArquivo/{id}")]
        public async Task<IActionResult> ExcluirArquivo(int id)
        {
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