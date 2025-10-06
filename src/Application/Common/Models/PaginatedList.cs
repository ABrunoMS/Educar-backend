using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Educar.Backend.Application.Common.Models;

// 1. CRIAMOS AS CLASSES PARA A ESTRUTURA ANINHADA DO PAYLOAD
public class Payload
{
    public PaginationModel? Pagination { get; set; }
}

public class PaginationModel
{
    public int PageNumber { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
}

// 2. MODIFICAMOS A CLASSE PRINCIPAL
public class PaginatedList<T>
{
    // A lista de itens agora se chama 'Data'
    public IReadOnlyCollection<T> Data { get; }
    
    // Todas as informações de paginação agora ficam dentro de 'Payload'
    public Payload Payload { get; }

    public PaginatedList(IReadOnlyCollection<T> items, int count, int pageNumber, int pageSize)
    {
        // A lógica principal continua a mesma, mas agora atribuímos às novas propriedades
        Data = items;

        var totalPages = (int)Math.Ceiling(count / (double)pageSize);

        Payload = new Payload
        {
            Pagination = new PaginationModel
            {
                PageNumber = pageNumber,
                TotalPages = totalPages,
                TotalCount = count,
                HasPreviousPage = pageNumber > 1,
                HasNextPage = pageNumber < totalPages
            }
        };
    }

    // O MÉTODO ESTÁTICO CreateAsync NÃO PRECISA DE NENHUMA ALTERAÇÃO
    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }
    public static PaginatedList<T> Create(List<T> items, int count, int pageNumber, int pageSize)
    {
        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }
}