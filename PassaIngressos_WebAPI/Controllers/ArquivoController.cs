using Microsoft.AspNetCore.Mvc;
using PassaIngressos_WebAPI.Database;

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

        #endregion
    }
}