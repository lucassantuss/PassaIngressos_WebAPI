using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PassaIngressos_WebAPI.Database;
using PassaIngressos_WebAPI.Entity;
using PassaIngressos_WebAPI.Dto;
using Microsoft.AspNetCore.Authorization;

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

        #region Ingresso

        // Método para anunciar/vender ingresso
        [Authorize]
        [HttpPost("AnunciarIngresso")]
        public async Task<IActionResult> AnunciarIngresso([FromBody] IngressoDto novoIngressoDto)
        {
            if (novoIngressoDto == null)
                return BadRequest("Dados do ingresso são obrigatórios.");

            if (string.IsNullOrWhiteSpace(novoIngressoDto.NomeEvento))
                return BadRequest("Nome do evento é obrigatório.");

            if (novoIngressoDto.DataHoraEvento == default)
                return BadRequest("Data e hora do evento são obrigatórias.");

            if (novoIngressoDto.Valor <= 0)
                return BadRequest("O valor do ingresso deve ser maior que 0.");

            var novoEvento = new Evento
            {
                NomeEvento = novoIngressoDto.NomeEvento,
                DataHoraEvento = novoIngressoDto.DataHoraEvento,
                LocalEvento = novoIngressoDto.LocalEvento,
                IdArquivoEvento = novoIngressoDto.IdArquivoEvento
            };

            _dbPassaIngressos.Eventos.Add(novoEvento);
            await _dbPassaIngressos.SaveChangesAsync();

            var novoIngresso = new Ingresso
            {
                IdTgTipoIngresso = novoIngressoDto.IdTipoIngresso,
                IdPessoaAnunciante = novoIngressoDto.IdPessoaAnunciante,
                IdEvento = novoEvento.IdEvento,
                Valor = novoIngressoDto.Valor,

                IdPessoaComprador = null,
                Vendido = false,
            };

            _dbPassaIngressos.Ingressos.Add(novoIngresso);
            await _dbPassaIngressos.SaveChangesAsync();

            return Ok("Ingresso colocado à venda com sucesso.");
        }

        // Método para alterar Ingresso
        [Authorize]
        [HttpPut("AlterarIngresso/{idIngresso}")]
        public async Task<IActionResult> AlterarIngresso(int idIngresso, [FromBody] AlterarIngressoDto ingressoAtualizado)
        {
            if (ingressoAtualizado == null)
                return BadRequest("Dados do ingresso são obrigatórios.");

            var ingresso = await _dbPassaIngressos.Ingressos.FindAsync(idIngresso);

            if (ingresso == null)
                return NotFound("Ingresso não encontrado.");

            var tipoIngressoExistente = await _dbPassaIngressos.ItensTabelaGeral
                                              .AnyAsync(i => i.IdItemTabelaGeral == ingressoAtualizado.IdTipoIngresso);

            if (!tipoIngressoExistente)
                return BadRequest("Tipo de ingresso não encontrado.");

            if (ingressoAtualizado.Valor <= 0)
                return BadRequest("O valor do ingresso deve ser maior que 0.");

            ingresso.IdTgTipoIngresso = ingressoAtualizado.IdTipoIngresso;
            ingresso.Valor = ingressoAtualizado.Valor;

            await _dbPassaIngressos.SaveChangesAsync();

            return Ok(ingresso);
        }

        // Método para excluir Ingresso
        [Authorize]
        [HttpDelete("ExcluirIngresso/{idIngresso}")]
        public async Task<IActionResult> ExcluirIngresso(int idIngresso)
        {
            if (idIngresso <= 0)
                return BadRequest("ID do ingresso inválido.");

            var ingresso = await _dbPassaIngressos.Ingressos.FindAsync(idIngresso);

            if (ingresso == null)
                return NotFound("Ingresso não encontrado.");

            _dbPassaIngressos.Ingressos.Remove(ingresso);
            await _dbPassaIngressos.SaveChangesAsync();

            return Ok("Ingresso excluído com sucesso.");
        }

        // Método para comprar Ingresso
        [Authorize]
        [HttpPost("ComprarIngresso/{idIngresso}")]
        public async Task<IActionResult> ComprarIngresso(int idIngresso, [FromBody] ComprarIngressoDto ingressoComprado)
        {
            if (ingressoComprado == null)
                return BadRequest("Dados do ingresso são obrigatórios.");

            var ingresso = await _dbPassaIngressos.Ingressos.FindAsync(idIngresso);

            if (ingresso == null)
                return NotFound("Ingresso não encontrado.");

            if (ingresso.IdPessoaAnunciante == ingressoComprado.IdPessoaComprador)
                return BadRequest("O comprador não pode ser o mesmo que o anunciante.");

            ingresso.IdPessoaComprador = ingressoComprado.IdPessoaComprador;
            ingresso.Vendido = true;

            await _dbPassaIngressos.SaveChangesAsync();

            return Ok("Ingresso comprado com sucesso.");
        }

        // Método para buscar ingressos por evento
        [AllowAnonymous]
        [HttpGet("BuscarIngressosPorEvento/{idEvento}")]
        public async Task<IActionResult> BuscarIngressosPorEvento(int idEvento)
        {
            if (idEvento <= 0)
                return BadRequest("ID do evento inválido.");

            var ingressos = await _dbPassaIngressos.Ingressos
                                  .Include(u => u.Evento)
                                  .Where(i => i.IdEvento == idEvento)
                                  .Select(xs => new IngressoEventoDto
                                  {
                                      IdIngresso = xs.IdIngresso,
                                      NomeEvento = xs.Evento.NomeEvento,
                                      LocalEvento = xs.Evento.LocalEvento,
                                      DataHoraEvento = xs.Evento.DataHoraEvento.Value.ToString("dd/MM/yyyy - HH'h'"),
                                      Valor = xs.Valor,

                                      IdPessoaAnunciante = xs.IdPessoaAnunciante,
                                      IdTipoIngresso = xs.IdTgTipoIngresso,
                                      IdArquivoEvento = xs.Evento.IdArquivoEvento
                                  })
                                  .ToListAsync();

            if (ingressos == null || !ingressos.Any())
                return NotFound("Nenhum ingresso encontrado para o evento.");

            return Ok(ingressos);
        }

        #endregion

        #region Evento

        // Método para criar Evento
        [Authorize]
        [HttpPost("CriarEvento")]
        public async Task<IActionResult> CriarEvento([FromBody] EventoDto eventoDto)
        {
            if (eventoDto == null)
                return BadRequest("Dados do evento são obrigatórios.");

            if (string.IsNullOrWhiteSpace(eventoDto.NomeEvento))
                return BadRequest("Nome do evento é obrigatório.");

            if (eventoDto.DataHoraEvento == default)
                return BadRequest("Data e hora do evento são obrigatórias.");

            var evento = new Evento
            {
                NomeEvento = eventoDto.NomeEvento,
                LocalEvento = eventoDto.LocalEvento,
                DataHoraEvento = eventoDto.DataHoraEvento,
                IdArquivoEvento = eventoDto.IdArquivoEvento
            };

            _dbPassaIngressos.Eventos.Add(evento);
            await _dbPassaIngressos.SaveChangesAsync();

            return Ok(evento);
        }

        // Método para editar Evento
        [Authorize]
        [HttpPut("EditarEvento/{idEvento}")]
        public async Task<IActionResult> EditarEvento(int idEvento, [FromBody] EventoDto eventoAtualizado)
        {
            if (eventoAtualizado == null)
                return BadRequest("Dados do evento são obrigatórios.");

            var evento = await _dbPassaIngressos.Eventos.FindAsync(idEvento);

            if (evento == null)
                return NotFound("Evento não encontrado.");

            evento.NomeEvento = eventoAtualizado.NomeEvento;
            evento.LocalEvento = eventoAtualizado.LocalEvento;
            evento.DataHoraEvento = eventoAtualizado.DataHoraEvento;
            evento.IdArquivoEvento = eventoAtualizado.IdArquivoEvento;

            await _dbPassaIngressos.SaveChangesAsync();

            return Ok(evento);
        }

        // Método para excluir Evento
        [Authorize]
        [HttpDelete("ExcluirEvento/{idEvento}")]
        public async Task<IActionResult> ExcluirEvento(int idEvento)
        {
            if (idEvento <= 0)
                return BadRequest("ID do evento inválido.");

            var evento = await _dbPassaIngressos.Eventos.FindAsync(idEvento);

            if (evento == null)
                return NotFound("Evento não encontrado.");

            _dbPassaIngressos.Eventos.Remove(evento);
            await _dbPassaIngressos.SaveChangesAsync();

            return Ok("Evento excluído com sucesso.");
        }

        // Método para listar todos os Eventos
        [AllowAnonymous]
        [HttpGet("ListarEventos")]
        public async Task<IActionResult> ListarEventos()
        {
            var eventos = await _dbPassaIngressos.Eventos
                                .Include(u => u.ArquivoEvento)
                                .ToListAsync();

            return Ok(eventos);
        }

        // Método para listar os Próximos Eventos
        [AllowAnonymous]
        [HttpGet("ListarProximosEventos")]
        public async Task<IActionResult> ListarProximosEventos()
        {
            var proximosEventos = await _dbPassaIngressos.Eventos
                                                 .Take(3)
                                                 .Select(xs => new ProximoEventoDto
                                                 {
                                                     IdEvento = xs.IdEvento,
                                                     NomeEvento = xs.NomeEvento,
                                                     Ano = xs.DataHoraEvento.Value.Year,
                                                     IdArquivoEvento = xs.IdArquivoEvento,
                                                 })
                                                 .ToListAsync();

            return Ok(proximosEventos);
        }

        // Método para listar os Eventos Relacionados
        [AllowAnonymous]
        [HttpGet("ListarEventosRelacionados")]
        public async Task<IActionResult> ListarEventosRelacionados()
        {
            var eventosRelacionados = await _dbPassaIngressos.Eventos
                                                 .Take(2)
                                                 .Select(xs => new EventoRelacionadoDto
                                                 {
                                                     IdEvento = xs.IdEvento,
                                                     NomeEvento = xs.NomeEvento,
                                                     Ano = xs.DataHoraEvento.Value.Year,
                                                     IdArquivoEvento = xs.IdArquivoEvento,
                                                 })
                                                 .ToListAsync();

            var ingressos = await _dbPassaIngressos.Ingressos.ToListAsync();

            foreach (var eventoRel in eventosRelacionados)
                eventoRel.QuantidadeIngressosDisponiveis = ingressos.Count(xs => xs.IdEvento == eventoRel.IdEvento);

            return Ok(eventosRelacionados);
        }

        // Método para pesquisar Evento específico
        [AllowAnonymous]
        [HttpGet("PesquisarEvento/{idEvento}")]
        public async Task<IActionResult> PesquisarEvento(int idEvento)
        {
            if (idEvento <= 0)
                return BadRequest("ID do evento inválido.");

            var evento = await _dbPassaIngressos.Eventos
                               .Include(u => u.ArquivoEvento)
                               .Where(xs => xs.IdEvento == idEvento)
                               .FirstOrDefaultAsync();

            if (evento == null)
                return NotFound("Evento não encontrado.");

            return Ok(evento);
        }

        // Método para pesquisar Eventos específicos
        [AllowAnonymous]
        [HttpGet("PesquisarEventosPorNome/{nomeEvento}")]
        public async Task<IActionResult> PesquisarEventosPorNome(string nomeEvento)
        {
            if (string.IsNullOrWhiteSpace(nomeEvento))
                return BadRequest("Nome do evento é obrigatório.");

            var eventos = await _dbPassaIngressos.Eventos
                               .Where(xs => xs.NomeEvento.Contains(nomeEvento))
                               .Select(xs => new EventoRelacionadoDto
                               {
                                   IdEvento = xs.IdEvento,
                                   NomeEvento = xs.NomeEvento,
                                   Ano = xs.DataHoraEvento.Value.Year,
                                   IdArquivoEvento = xs.IdArquivoEvento
                               })
                               .ToListAsync();

            if (eventos == null || !eventos.Any())
                return NotFound("Nenhum evento encontrado.");

            var ingressos = await _dbPassaIngressos.Ingressos.ToListAsync();

            foreach (var evento in eventos)
                evento.QuantidadeIngressosDisponiveis = ingressos.Count(xs => xs.IdEvento == evento.IdEvento);

            return Ok(eventos);
        }

        #endregion
    }
}