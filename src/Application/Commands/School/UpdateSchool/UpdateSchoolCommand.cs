using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Entities;
using FluentValidation.Results;

namespace Educar.Backend.Application.Commands.School.UpdateSchool;

public record UpdateSchoolCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Guid? AddressId { get; set; }
    public Guid? ClientId { get; set; }
    public Guid? RegionalId { get; set; }
    public DateTime? ContractStartDate { get; set; }
    public List<Guid>? TeacherIds { get; init; }
    public List<Guid>? StudentIds { get; init; }
}

public class UpdateSchoolCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateSchoolCommand, Unit>
{
    public async Task<Unit> Handle(UpdateSchoolCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Schools
            .Include(s => s.Client)
            .Include(s => s.AccountSchools)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, entity);

        if (request.Name != null) entity.Name = request.Name;
        if (request.Description != null) entity.Description = request.Description;
        if (request.ContractStartDate.HasValue) entity.ContractStartDate = request.ContractStartDate;
        
        if (request.AddressId.HasValue)
        {
            var address = await context.Addresses.FindAsync([request.AddressId.Value], cancellationToken: cancellationToken);
            if (address != null)
            {
                entity.AddressId = request.AddressId.Value;
            }
        }

        if (request.ClientId.HasValue)
        {
            var client = await context.Clients.FindAsync([request.ClientId.Value], cancellationToken: cancellationToken);
            if (client != null)
            {
                entity.ClientId = request.ClientId.Value;
            }
        }

        if (request.RegionalId.HasValue)
        {
            // Primeiro, validar se a Regional existe
            var regional = await context.Regionais
                .Include(r => r.Subsecretaria)
                .FirstOrDefaultAsync(r => r.Id == request.RegionalId.Value, cancellationToken);
            
            if (regional == null)
            {
                throw new Educar.Backend.Application.Common.Exceptions.NotFoundException(nameof(Domain.Entities.Regional), request.RegionalId.Value.ToString());
            }

            // Depois de confirmar que a Regional é válida, validar se pertence ao Client correto
            var clientIdToValidate = request.ClientId ?? entity.ClientId;
            if (regional.Subsecretaria?.ClientId != clientIdToValidate)
            {
                var failures = new List<ValidationFailure>
                {
                    new ValidationFailure("RegionalId", "A Regional informada não pertence ao Client especificado.")
                };
                throw new Educar.Backend.Application.Common.Exceptions.ValidationException(failures);
            }
            
            entity.RegionalId = request.RegionalId.Value;
        }

        // Atualização de Professores e Alunos
        if (request.TeacherIds != null || request.StudentIds != null)
        {
            var newAccountIds = (request.TeacherIds ?? new List<Guid>())
                .Concat(request.StudentIds ?? new List<Guid>())
                .Distinct().ToList();
            
            UpdateJunctionTable(entity.AccountSchools, newAccountIds, acs => acs.AccountId, id => new AccountSchool { AccountId = id });
        }

        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    // Método genérico para atualizar tabelas de ligação (Junction Tables)
    private void UpdateJunctionTable<TEntity, TKey>(
        ICollection<TEntity> currentItems, 
        ICollection<TKey> newItemIds, 
        Func<TEntity, TKey> keySelector, 
        Func<TKey, TEntity> createFactory) 
        where TEntity : class 
        where TKey : IEquatable<TKey>
    {
        var currentIds = currentItems.Select(keySelector).ToList();
        
        // 1. Adicionar novos
        var idsToAdd = newItemIds.Except(currentIds).ToList();
        foreach (var id in idsToAdd)
        {
            currentItems.Add(createFactory(id));
        }

        // 2. Remover antigos
        var idsToRemove = currentIds.Except(newItemIds).ToList();
        var itemsToRemove = currentItems.Where(item => idsToRemove.Contains(keySelector(item))).ToList();
        foreach (var item in itemsToRemove)
        {
            currentItems.Remove(item);
        }
    }
}
