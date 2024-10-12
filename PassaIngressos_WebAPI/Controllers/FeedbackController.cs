using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PassaIngressos_WebAPI.Database;
using PassaIngressos_WebAPI.Entity;
using PassaIngressos_WebAPI.Dto;

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
        [HttpPost("CriarFeedback")]
        public async Task<IActionResult> CriarFeedback([FromBody] FeedbackDto feedbackDto)
        {
            Feedback feedback = new Feedback
            {
                DescricaoFeedback = feedbackDto.DescricaoFeedback,
                IdPessoa = feedbackDto.IdPessoa,
            };

            if (feedbackDto == null)
                return BadRequest("Feedback inválido.");

            await _dbPassaIngressos.Feedbacks.AddAsync(feedback);
            await _dbPassaIngressos.SaveChangesAsync();

            return Ok(feedback);
        }

        // Método para listar todos os feedbacks
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

                fb.NomePessoa = pessoaSelecionada.Nome;
                fb.IdadePessoa = CalcularIdade(pessoaSelecionada.DataNascimento.Value);
                fb.IdArquivoFoto =pessoaSelecionada.IdArquivoFoto;
            }

            return Ok(feedbacks);
        }

        // Método para pesquisar feedbacks por IdPessoa
        [HttpGet("PesquisarFeedbacksPorPessoa/{idPessoa}")]
        public async Task<IActionResult> PesquisarFeedbacksPorPessoa(int idPessoa)
        {
            var feedbacks = await _dbPassaIngressos.Feedbacks
                                                   .Where(f => f.IdPessoa == idPessoa)
                                                   .ToListAsync();

            if (feedbacks == null || feedbacks.Count == 0)
                return NotFound("Nenhum feedback encontrado para essa pessoa.");

            return Ok(feedbacks);
        }

        // Método para alterar Feedback
        [HttpPut("AlterarFeedback/{idFeedback}")]
        public async Task<IActionResult> AlterarFeedback(int idFeedback, [FromBody] FeedbackDto feedbackAtualizado)
        {
            var feedbackExistente = await _dbPassaIngressos.Feedbacks.FindAsync(idFeedback);

            if (feedbackExistente == null)
                return NotFound("Feedback não encontrado.");

            feedbackExistente.DescricaoFeedback = feedbackAtualizado.DescricaoFeedback;
            feedbackExistente.IdPessoa = feedbackAtualizado.IdPessoa;

            _dbPassaIngressos.Feedbacks.Update(feedbackExistente);
            await _dbPassaIngressos.SaveChangesAsync();

            return Ok(feedbackExistente);
        }

        // Método para excluir Feedback
        [HttpDelete("ExcluirFeedback/{idFeedback}")]
        public async Task<IActionResult> ExcluirFeedback(int idFeedback)
        {
            var feedback = await _dbPassaIngressos.Feedbacks.FindAsync(idFeedback);

            if (feedback == null)
                return NotFound("Feedback não encontrado.");

            _dbPassaIngressos.Feedbacks.Remove(feedback);
            await _dbPassaIngressos.SaveChangesAsync();

            return Ok("Feedback excluído com sucesso.");
        }

        private int CalcularIdade(DateTime dataNascimento)
        {
            var hoje = DateTime.UtcNow;
            int idade = hoje.Year - dataNascimento.Year;

            // Verifica se já fez aniversário este ano
            if (hoje < dataNascimento.AddYears(idade))
                idade--;

            return idade;
        }

        #endregion
    }
}