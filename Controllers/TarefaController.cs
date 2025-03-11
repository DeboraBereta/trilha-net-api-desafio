using Microsoft.AspNetCore.Mvc;
using TrilhaApiDesafio.Context;
using TrilhaApiDesafio.Models;

namespace TrilhaApiDesafio.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TarefaController : ControllerBase
    {
        private readonly OrganizadorContext _context;

        public TarefaController(OrganizadorContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public IActionResult ObterPorId(int id)
        {
            var tarefa = _context.Tarefas.Find(id); // Busca a tarefa pelo ID no banco de dados

            if (tarefa == null) // Verifica se a tarefa não foi encontrada
                return NotFound(); // Retorna status 404 (Not Found)

            return Ok(tarefa); // Retorna a tarefa com status 200 (OK)
        }

        [HttpGet("ObterTodos")]
        public IActionResult ObterTodos()
        {
            var tarefas = _context.Tarefas.ToList(); // Busca todas as tarefas
            return Ok(tarefas); // Retorna a lista com status 200 (OK)
        }

        [HttpGet("ObterPorTitulo")]
        public IActionResult ObterPorTitulo(string titulo)
        {
            var tarefas = _context.Tarefas.Where(x => x.Titulo.Contains(titulo)).ToList(); // Filtra tarefas pelo título
            return Ok(tarefas); // Retorna a lista filtrada
        }

        [HttpGet("ObterPorData")]
        public IActionResult ObterPorData(DateTime data)
        {
            var tarefa = _context.Tarefas.Where(x => x.Data.Date == data.Date);
            return Ok(tarefa);
        }

        [HttpGet("ObterPorStatus")]
        public IActionResult ObterPorStatus(EnumStatusTarefa status)
        {
            // Verifica se o valor do status é válido
            if (!Enum.IsDefined(typeof(EnumStatusTarefa), status))
            {
                return BadRequest(new { Mensagem = "Status inválido." });
            }

            // Busca as tarefas no banco que tenham o status recebido como parâmetro
            var tarefas = _context.Tarefas
                .Where(x => x.Status == status)
                .ToList(); // Materializa a consulta

            // Verifica se há tarefas com o status especificado
            if (tarefas == null || !tarefas.Any())
            {
                return NotFound(new { Mensagem = "Nenhuma tarefa encontrada com o status especificado." });
            }

            // Retorna as tarefas encontradas
            return Ok(tarefas);
        }

        [HttpPost]
        public async Task<IActionResult> Criar(Tarefa tarefa)
        {
            if (tarefa.Data == DateTime.MinValue)
                return BadRequest(new { Erro = "A data da tarefa não pode ser vazia" });


            if (!Enum.IsDefined(typeof(EnumStatusTarefa), tarefa.Status)) // Verifica se o status é válido
            {
                return BadRequest(new { Erro = "Status da tarefa inválido" });
            }

            _context.Tarefas.Add(tarefa); // Adiciona a tarefa ao contexto
            await _context.SaveChangesAsync(); // Salva as mudanças no banco de dados de forma assíncrona

            // Retorna a resposta com o status 201 (Created) e a localização do recurso criado
            return CreatedAtAction(nameof(ObterPorId), new { id = tarefa.Id }, tarefa);
        }

        [HttpPut("{id}")]
        public IActionResult Atualizar(int id, Tarefa tarefa)
        {
            var tarefaBanco = _context.Tarefas.Find(id);

            if (tarefaBanco == null)
                return NotFound();

            if (tarefa.Data == DateTime.MinValue)
                return BadRequest(new { Erro = "A data da tarefa não pode ser vazia" });

            // Atualiza os dados da tarefa
            tarefaBanco.Titulo = tarefa.Titulo;
            tarefaBanco.Descricao = tarefa.Descricao;
            tarefaBanco.Data = tarefa.Data;
            tarefaBanco.Status = tarefa.Status;

            _context.Tarefas.Update(tarefaBanco); // Marca a tarefa como "modificada"
            _context.SaveChanges(); // Salva as mudanças

            return Ok(tarefaBanco); // Retorna a tarefa atualizada
        }

        [HttpDelete("{id}")]
        public IActionResult Deletar(int id)
        {
            // Busca a tarefa no banco de dados pelo Id
            var tarefaBanco = _context.Tarefas.Find(id);

            // Verifica se a tarefa foi encontrada
            if (tarefaBanco == null)
            {
                return NotFound(new { Mensagem = "Tarefa não encontrada." }); // Retorna 404
            }

            // Remove a tarefa do banco de dados
            _context.Tarefas.Remove(tarefaBanco);

            // Salva as mudanças no banco de dados
            _context.SaveChanges();

            // Retorna uma resposta de sucesso (204 No Content)
            return NoContent();
        }
    }
}