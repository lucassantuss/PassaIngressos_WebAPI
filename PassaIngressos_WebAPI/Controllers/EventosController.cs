using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PassaIngressos_WebAPI.Database;
using PassaIngressos_WebAPI.Entity;
using PassaIngressos_WebAPI.Dto;

namespace PassaIngressos_WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EventosController : ControllerBase
    {
        #region Contexto e Variáveis

        private readonly DbPassaIngressos _dbPassaIngressos;

        public EventosController(DbPassaIngressos _context)
        {
            _dbPassaIngressos = _context;
        }

        #endregion

        #region Evento

        // Método para criar Evento
        [HttpPost("CriarEvento")]
        public async Task<IActionResult> CriarEvento([FromBody] EventoDto eventoDto)
        {
            var evento = new Evento
            {
                NomeEvento = eventoDto.NomeEvento,
                LocalEvento = eventoDto.LocalEvento,
                DataHoraEvento = eventoDto.DataHoraEvento
            };

            _dbPassaIngressos.Eventos.Add(evento);
            await _dbPassaIngressos.SaveChangesAsync();

            return Ok(evento);
        }

        // Método para editar Evento
        [HttpPut("EditarEvento/{idEvento}")]
        public async Task<IActionResult> EditarEvento(int idEvento, [FromBody] EventoDto eventoAtualizado)
        {
            var evento = await _dbPassaIngressos.Eventos.FindAsync(idEvento);

            if (evento == null)
                return NotFound("Evento não encontrado.");

            evento.NomeEvento = eventoAtualizado.NomeEvento;
            evento.LocalEvento = eventoAtualizado.LocalEvento;
            evento.DataHoraEvento = eventoAtualizado.DataHoraEvento;

            await _dbPassaIngressos.SaveChangesAsync();

            return Ok(evento);
        }

        // Método para excluir Evento
        [HttpDelete("ExcluirEvento/{idEvento}")]
        public async Task<IActionResult> ExcluirEvento(int idEvento)
        {
            var evento = await _dbPassaIngressos.Eventos.FindAsync(idEvento);

            if (evento == null)
                return NotFound("Evento não encontrado.");

            _dbPassaIngressos.Eventos.Remove(evento);
            await _dbPassaIngressos.SaveChangesAsync();

            return Ok("Evento excluído com sucesso.");
        }

        // Método para listar todos os Eventos
        [HttpGet("ListarEventos")]
        public async Task<IActionResult> ListarEventos()
        {
            var eventos = await _dbPassaIngressos.Eventos
                                .Include(u => u.ArquivoEvento)
                                .ToListAsync();

            return Ok(eventos);
        }

        // Método para pesquisar Evento específico
        [HttpGet("PesquisarEvento/{idEvento}")]
        public async Task<IActionResult> PesquisarEvento(int idEvento)
        {
            var evento = await _dbPassaIngressos.Eventos
                               .Include(u => u.ArquivoEvento)
                               .Where(xs => xs.IdEvento == idEvento)
                               .FirstOrDefaultAsync();

            if (evento == null)
                return NotFound("Evento não encontrado.");

            return Ok(evento);
        }

        #endregion

        #region Ingresso

        // Método para vender/anunciar o Ingresso
        [HttpPost("VenderIngresso")]
        public async Task<IActionResult> VenderIngresso([FromBody] IngressoDto ingressoDto)
        {
            var ingresso = new Ingresso
            {
                IdTgTipoIngresso = ingressoDto.IdTipoIngresso,
                Valor = ingressoDto.Valor,
                IdPessoaAnunciante = ingressoDto.IdPessoaAnunciante,
                IdEvento = ingressoDto.IdEvento,
            };

            _dbPassaIngressos.Ingressos.Add(ingresso);
            await _dbPassaIngressos.SaveChangesAsync();

            return Ok("Ingresso colocado à venda com sucesso.");
        }

        // Método para alterar Ingresso
        [HttpPut("AlterarIngresso/{idIngresso}")]
        public async Task<IActionResult> AlterarIngresso(int idIngresso, [FromBody] AlterarIngressoDto ingressoAtualizado)
        {
            var ingresso = await _dbPassaIngressos.Ingressos.FindAsync(idIngresso);

            if (ingresso == null)
                return NotFound("Ingresso não encontrado.");

            ingresso.IdTgTipoIngresso = ingressoAtualizado.IdTipoIngresso;
            ingresso.Valor = ingressoAtualizado.Valor;

            await _dbPassaIngressos.SaveChangesAsync();

            return Ok(ingresso);
        }

        // Método para excluir Ingresso
        [HttpDelete("ExcluirIngresso/{idIngresso}")]
        public async Task<IActionResult> ExcluirIngresso(int idIngresso)
        {
            var ingresso = await _dbPassaIngressos.Ingressos.FindAsync(idIngresso);

            if (ingresso == null)
                return NotFound("Ingresso não encontrado.");

            _dbPassaIngressos.Ingressos.Remove(ingresso);
            await _dbPassaIngressos.SaveChangesAsync();

            return Ok("Ingresso excluído com sucesso.");
        }

        // Método para comprar Ingresso
        [HttpPost("ComprarIngresso/{idIngresso}")]
        public async Task<IActionResult> ComprarIngresso(int idIngresso, [FromBody] ComprarIngressoDto ingressoComprado)
        {
            var ingresso = await _dbPassaIngressos.Ingressos.FindAsync(idIngresso);

            if (ingresso == null)
                return NotFound("Ingresso não encontrado.");

            // TODO Implementar lógica para comprar e associar ingresso ao IdPessoa logado

            await _dbPassaIngressos.SaveChangesAsync();

            return Ok("Ingresso comprado com sucesso.");
        }

        // Método para buscar ingressos por evento
        [HttpGet("BuscarIngressosPorEvento/{idEvento}")]
        public async Task<IActionResult> BuscarIngressosPorEvento(int idEvento)
        {
            var ingressos = await _dbPassaIngressos.Ingressos
                                  .Include(u => u.TipoIngresso)
                                  .Include(u => u.PessoaAnunciante)
                                  .Include(u => u.Evento)
                                  .Where(i => i.IdEvento == idEvento)
                                  .ToListAsync();

            if (ingressos == null || !ingressos.Any())
                return NotFound("Nenhum ingresso encontrado para o evento.");

            return Ok(ingressos);
        }

        #endregion
    }
}