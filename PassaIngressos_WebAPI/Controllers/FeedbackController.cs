using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PassaIngressos_WebAPI.Database;
using PassaIngressos_WebAPI.Entity;
using PassaIngressos_WebAPI.Dto;
using PassaIngressos_WebAPI.Util;
using Microsoft.AspNetCore.Authorization;

namespace PassaIngressos_WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        #region Contexto e Variáveis

        private readonly DbPassaIngressos _dbPassaIngressos;

        public FeedbackController(DbPassaIngressos _context)
        {
            _dbPassaIngressos = _context;
        }

        #endregion

        #region Feedback

        // Método para criar Feedback
        [Authorize]
        [HttpPost("CriarFeedback")]
        public async Task<IActionResult> CriarFeedback([FromBody] FeedbackDto feedbackDto)
        {
            if (feedbackDto == null)
                return BadRequest("Dados do feedback são obrigatórios.");

            if (string.IsNullOrWhiteSpace(feedbackDto.DescricaoFeedback))
                return BadRequest("A descrição do feedback é obrigatória.");

            if (feedbackDto.IdPessoa <= 0)
                return BadRequest("ID da pessoa inválido.");

            Feedback feedback = new Feedback
            {
                DescricaoFeedback = feedbackDto.DescricaoFeedback,
                IdPessoa = feedbackDto.IdPessoa,
            };

            await _dbPassaIngressos.Feedbacks.AddAsync(feedback);
            await _dbPassaIngressos.SaveChangesAsync();

            return Ok(feedback);
        }

        // Método para listar todos os feedbacks
        [AllowAnonymous]
        [HttpGet("ListarFeedbacks")]
        public async Task<IActionResult> ListarFeedbacks()
        {
            var feedbacks = await _dbPassaIngressos.Feedbacks
                                                   .Take(3)
                                                   .Select(xs => new FeedbackRetornoDto
                                                   {
                                                       IdFeedback = xs.IdFeedback,
                                                       DescricaoFeedback = xs.DescricaoFeedback,
                                                       IdPessoa = xs.IdPessoa
                                                   })
                                                   .ToListAsync();

            var pessoas = await _dbPassaIngressos.Pessoas
                                                 .Include(xs => xs.ArquivoFoto)
                                                 .ToListAsync();

            foreach (var fb in feedbacks)
            {
                var pessoaSelecionada = pessoas.Find(xs => xs.IdPessoa == fb.IdPessoa);

                if (pessoaSelecionada != null)
                {
                    ValidacaoHelper ValidacaoHelper = new ValidacaoHelper();

                    fb.NomePessoa = pessoaSelecionada.Nome;
                    fb.IdadePessoa = ValidacaoHelper.CalcularIdade(pessoaSelecionada.DataNascimento.Value);
                    fb.IdArquivoFoto = pessoaSelecionada.IdArquivoFoto;
                }
            }

            return Ok(feedbacks);
        }

        // Método para pesquisar feedbacks por IdPessoa
        [Authorize]
        [HttpGet("PesquisarFeedbacksPorPessoa/{idPessoa}")]
        public async Task<IActionResult> PesquisarFeedbacksPorPessoa(int idPessoa)
        {
            if (idPessoa <= 0)
                return BadRequest("ID da pessoa inválido.");

            var feedbacks = await _dbPassaIngressos.Feedbacks
                                                   .Where(f => f.IdPessoa == idPessoa)
                                                   .Select(xs => new FeedbackRetornoDto
                                                   {
                                                       IdFeedback = xs.IdFeedback,
                                                       DescricaoFeedback = xs.DescricaoFeedback,
                                                       IdPessoa = xs.IdPessoa
                                                   })
                                                   .ToListAsync();

            if (feedbacks == null || feedbacks.Count == 0)
                return NotFound("Nenhum feedback encontrado para essa pessoa.");

            var pessoas = await _dbPassaIngressos.Pessoas
                                                 .Include(xs => xs.ArquivoFoto)
                                                 .ToListAsync();

            foreach (var fb in feedbacks)
            {
                var pessoaSelecionada = pessoas.Find(xs => xs.IdPessoa == fb.IdPessoa);

                if (pessoaSelecionada != null)
                {
                    ValidacaoHelper ValidacaoHelper = new ValidacaoHelper();

                    fb.NomePessoa = pessoaSelecionada.Nome;
                    fb.IdadePessoa = ValidacaoHelper.CalcularIdade(pessoaSelecionada.DataNascimento.Value);
                    fb.IdArquivoFoto = pessoaSelecionada.IdArquivoFoto;
                }
            }

            return Ok(feedbacks);
        }

        // Método para alterar Feedback
        [Authorize]
        [HttpPut("AlterarFeedback/{idFeedback}")]
        public async Task<IActionResult> AlterarFeedback(int idFeedback, [FromBody] FeedbackDto feedbackAtualizado)
        {
            if (feedbackAtualizado == null)
                return BadRequest("Dados do feedback são obrigatórios.");

            if (string.IsNullOrWhiteSpace(feedbackAtualizado.DescricaoFeedback))
                return BadRequest("A descrição do feedback é obrigatória.");

            if (feedbackAtualizado.IdPessoa <= 0)
                return BadRequest("ID da pessoa inválido.");

            var feedbackExistente = await _dbPassaIngressos.Feedbacks.FindAsync(idFeedback);

            if (feedbackExistente == null)
                return NotFound("Feedback não encontrado.");

            feedbackExistente.DescricaoFeedback = feedbackAtualizado.DescricaoFeedback;
            feedbackExistente.IdPessoa = feedbackAtualizado.IdPessoa;

            var pessoa = await _dbPassaIngressos.Pessoas
                                                .Where(xs => xs.IdPessoa == feedbackExistente.IdPessoa)
                                                .FirstOrDefaultAsync();

            ValidacaoHelper ValidacaoHelper = new ValidacaoHelper();

            FeedbackRetornoDto feedbackRetorno = new FeedbackRetornoDto
            {
                IdFeedback = feedbackExistente.IdFeedback,
                DescricaoFeedback = feedbackExistente.DescricaoFeedback,

                IdPessoa = feedbackExistente.IdPessoa,
                NomePessoa = pessoa?.Nome,
                IdadePessoa = pessoa != null ? ValidacaoHelper.CalcularIdade(pessoa.DataNascimento.Value) : 0,
                IdArquivoFoto = pessoa?.IdArquivoFoto
            };

            _dbPassaIngressos.Feedbacks.Update(feedbackExistente);
            await _dbPassaIngressos.SaveChangesAsync();

            return Ok(feedbackRetorno);
        }

        // Método para excluir Feedback
        [Authorize]
        [HttpDelete("ExcluirFeedback/{idFeedback}")]
        public async Task<IActionResult> ExcluirFeedback(int idFeedback)
        {
            if (idFeedback <= 0)
                return BadRequest("ID do feedback inválido.");

            var feedback = await _dbPassaIngressos.Feedbacks.FindAsync(idFeedback);

            if (feedback == null)
                return NotFound("Feedback não encontrado.");

            _dbPassaIngressos.Feedbacks.Remove(feedback);
            await _dbPassaIngressos.SaveChangesAsync();

            return Ok("Feedback excluído com sucesso.");
        }

        #endregion
    }
}