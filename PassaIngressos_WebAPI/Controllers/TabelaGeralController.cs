using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PassaIngressos_WebAPI.Database;
using PassaIngressos_WebAPI.Dto;
using PassaIngressos_WebAPI.Entity;

namespace PassaIngressos_WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TabelaGeralController : ControllerBase
    {
        #region Contexto e Variáveis

        private readonly DbPassaIngressos _dbPassaIngressos;

        public TabelaGeralController(DbPassaIngressos context)
        {
            _dbPassaIngressos = context;
        }

        #endregion

        #region Tabela Geral

        // Método para listar tabelas gerais
        [AllowAnonymous]
        [HttpGet("ListarTabelasGerais")]
        public async Task<IActionResult> ListarTabelasGerais()
        {
            var tabelasGerais = await _dbPassaIngressos.TabelasGerais.ToListAsync();
            return Ok(tabelasGerais);
        }

        // Método para pesquisar itens da tabela geral por nome da tabela
        [AllowAnonymous]
        [HttpGet("PesquisarItensPorTabela/{nomeTabela}")]
        public async Task<IActionResult> PesquisarItensPorTabela(string nomeTabela)
        {
            if (string.IsNullOrWhiteSpace(nomeTabela))
                return BadRequest("Nome da tabela é obrigatório.");

            var tabelaGeral = await _dbPassaIngressos.TabelasGerais
                                                     .FirstOrDefaultAsync(t => t.Tabela == nomeTabela);

            if (tabelaGeral == null)
                return NotFound("Tabela Geral não encontrada.");

            var itensTabela = await _dbPassaIngressos.ItensTabelaGeral
                                                     .Where(i => i.IdTabelaGeral == tabelaGeral.IdTabelaGeral)
                                                     .ToListAsync();

            return Ok(itensTabela);
        }

        // Método para adicionar item na tabela geral informada
        [Authorize]
        [HttpPost("AdicionarItem")]
        public async Task<IActionResult> AdicionarItem([FromBody] ItemTabelaGeralDto novoItemDto)
        {
            if (novoItemDto == null)
                return BadRequest("Dados do novo item são obrigatórios.");

            if (string.IsNullOrWhiteSpace(novoItemDto.Sigla))
                return BadRequest("A sigla do item é obrigatória.");

            if (string.IsNullOrWhiteSpace(novoItemDto.Descricao))
                return BadRequest("A descrição do item é obrigatória.");

            if (novoItemDto.TabelaGeral == null || novoItemDto.TabelaGeral.IdTabelaGeral <= 0)
                return BadRequest("Tabela Geral é obrigatória.");

            var tabelaGeral = await _dbPassaIngressos.TabelasGerais
                                                     .FirstOrDefaultAsync(t => t.IdTabelaGeral == novoItemDto.TabelaGeral.IdTabelaGeral);

            if (tabelaGeral == null)
                return NotFound("Tabela Geral não encontrada.");

            ItemTabelaGeral novoItem = new ItemTabelaGeral
            {
                Sigla = novoItemDto.Sigla,
                Descricao = novoItemDto.Descricao,
                IdTabelaGeral = novoItemDto.TabelaGeral.IdTabelaGeral
            };

            _dbPassaIngressos.ItensTabelaGeral.Add(novoItem);
            await _dbPassaIngressos.SaveChangesAsync();

            return Ok(novoItem);
        }

        // Método para criar nova tabela geral
        [Authorize]
        [HttpPost("CriarTabelaGeral")]
        public async Task<IActionResult> CriarTabelaGeral([FromBody] TabelaGeralDto novaTabelaDto)
        {
            if (novaTabelaDto == null)
                return BadRequest("Dados da nova tabela são obrigatórios.");

            if (string.IsNullOrWhiteSpace(novaTabelaDto.Tabela))
                return BadRequest("Nome da tabela é obrigatório.");

            TabelaGeral novaTabela = new TabelaGeral
            {
                Tabela = novaTabelaDto.Tabela,
            };

            _dbPassaIngressos.TabelasGerais.Add(novaTabela);
            await _dbPassaIngressos.SaveChangesAsync();

            return Ok(novaTabela);
        }

        // Método para excluir item da tabela geral
        [Authorize]
        [HttpDelete("ExcluirItem/{idItem}")]
        public async Task<IActionResult> ExcluirItem(int idItem)
        {
            if (idItem <= 0)
                return BadRequest("ID do item inválido.");

            var item = await _dbPassaIngressos.ItensTabelaGeral.FindAsync(idItem);

            if (item == null)
                return NotFound("Item não encontrado.");

            _dbPassaIngressos.ItensTabelaGeral.Remove(item);
            await _dbPassaIngressos.SaveChangesAsync();

            return Ok("Item excluído com sucesso.");
        }

        #endregion
    }
}