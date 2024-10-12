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

        #region Ingresso

        // Método para vender/anunciar o Ingresso
        [HttpPost("AnunciarIngresso")]
        public async Task<IActionResult> AnunciarIngresso([FromBody] IngressoDto novoIngressoDto)
        {
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
                Valor = novoIngressoDto.Valor
            };

            _dbPassaIngressos.Ingressos.Add(novoIngresso);
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

            // Verifica se o TipoIngresso existe
            var tipoIngressoExistente = await _dbPassaIngressos.ItensTabelaGeral
                                              .AnyAsync(i => i.IdItemTabelaGeral == ingressoAtualizado.IdTipoIngresso);

            if (!tipoIngressoExistente)
                return BadRequest("Tipo de ingresso não encontrado.");

            // Valida se o valor do ingresso é maior que 0
            if (ingressoAtualizado.Valor <= 0)
                return BadRequest("O valor do ingresso deve ser maior que 0.");

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

            if (ingresso.IdPessoaAnunciante == ingressoComprado.IdPessoaComprador)
                return BadRequest("O comprador não pode ser o mesmo que o anunciante.");

            ingresso.IdPessoaComprador = ingressoComprado.IdPessoaComprador;
            ingresso.Vendido = true;

            await _dbPassaIngressos.SaveChangesAsync();

            return Ok("Ingresso comprado com sucesso.");
        }

        // Método para buscar ingressos por evento
        [HttpGet("BuscarIngressosPorEvento/{idEvento}")]
        public async Task<IActionResult> BuscarIngressosPorEvento(int idEvento)
        {
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
        [HttpPost("CriarEvento")]
        public async Task<IActionResult> CriarEvento([FromBody] EventoDto eventoDto)
        {
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
        [HttpPut("EditarEvento/{idEvento}")]
        public async Task<IActionResult> EditarEvento(int idEvento, [FromBody] EventoDto eventoAtualizado)
        {
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

        // Método para listar os Próximos Eventos
        [HttpGet("ListarProximosEventos")]
        public async Task<IActionResult> ListarProximosEventos()
        {
            var proximosEventos = await _dbPassaIngressos.Eventos
                                                 .Take(3)
                                                 .Select(xs => new ProximoEventoDto
                                                 {
                                                     NomeEvento = xs.NomeEvento,
                                                     Ano = xs.DataHoraEvento.Value.Year,
                                                     IdArquivoEvento = xs.IdArquivoEvento,
                                                 })
                                                 .ToListAsync();

            return Ok(proximosEventos);
        }

        // Método para listar os Eventos Relacionados
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

        // Método para pesquisar Eventos específicos
        [HttpGet("PesquisarEventosPorNome/{nomeEvento}")]
        public async Task<IActionResult> PesquisarEventosPorNome(string nomeEvento)
        {
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
            
            if (eventos == null)
                return NotFound("Evento não encontrado.");

            var ingressos = await _dbPassaIngressos.Ingressos.ToListAsync();

            foreach (var evento in eventos)
                evento.QuantidadeIngressosDisponiveis = ingressos.Count(xs => xs.IdEvento == evento.IdEvento);

            return Ok(eventos);
        }

        #endregion
    }
}