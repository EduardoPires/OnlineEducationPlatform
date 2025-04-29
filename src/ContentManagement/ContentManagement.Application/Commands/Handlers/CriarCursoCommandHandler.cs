using ContentManagement.Application.DTOs;
using ContentManagement.Domain.Aggregates;
using ContentManagement.Domain.Enums;
using ContentManagement.Domain.Repositories;
using ContentManagement.Domain.ValueObjects;
using MediatR;

namespace ContentManagement.Application.Commands.Handlers
{
    public class CriarCursoCommandHandler : IRequestHandler<CriarCursoCommand, CursoDTO>
    {
        private readonly ICursoRepository _cursoRepository;

        public CriarCursoCommandHandler(ICursoRepository cursoRepository)
        {
            _cursoRepository = cursoRepository;
        }

        public async Task<CursoDTO> Handle(CriarCursoCommand request, CancellationToken cancellationToken)
        {
            // Valida��o dos dados do comando
            if (string.IsNullOrWhiteSpace(request.Nome))
                throw new ArgumentException("O nome do curso � obrigat�rio.", nameof(request.Nome));

            if (string.IsNullOrWhiteSpace(request.DescricaoConteudo))
                throw new ArgumentException("A descri��o do conte�do program�tico � obrigat�ria.", nameof(request.DescricaoConteudo));

            if (request.Objetivos == null || !request.Objetivos.Any())
                throw new ArgumentException("Pelo menos um objetivo deve ser informado.", nameof(request.Objetivos));

            if (request.PreRequisitos == null)
                request.PreRequisitos = new List<string>();

            // Cria��o do Value Object ConteudoProgramatico
            var conteudoProgramatico = new ConteudoProgramatico(
                request.DescricaoConteudo,
                request.Objetivos,
                request.PreRequisitos
            );

            // Cria��o da entidade Curso
            var curso = new Curso(request.Nome, conteudoProgramatico);

            // Persist�ncia no reposit�rio
            var cursoCriado = await _cursoRepository.AddAsync(curso);

            // Mapeamento para DTO
            var cursoDTO = new CursoDTO
            {
                Id = cursoCriado.Id,
                Titulo = cursoCriado.Nome,
                Descricao = cursoCriado.ConteudoProgramatico.Descricao,
                Preco = 0, // Pre�o n�o est� no comando, ajuste conforme necess�rio
                Duracao = 0, // Dura��o n�o est� no comando, ajuste conforme necess�rio
                Nivel = string.Empty, // N�vel n�o est� no comando, ajuste conforme necess�rio
                Status = StatusCurso.Ativo,
                DataCriacao = DateTime.UtcNow,
                DataAtualizacao = null,
                Aulas = new List<AulaDTO>() // Inicialmente sem aulas
            };

            return cursoDTO;
        }
    }
}