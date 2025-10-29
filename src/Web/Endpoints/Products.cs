using Educar.Backend.Application.Common.Models;
using Educar.Backend.Application.Queries.Content;
using Educar.Backend.Application.Queries.Product;

namespace Educar.Backend.Web.Endpoints;

public class Products : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        // Todas as rotas de Produtos/Conteúdos exigem que o usuário esteja logado
        var group = app.MapGroup(this)
            .RequireAuthorization(); 

        // Rota 1: GET /api/Products (para listar todos os produtos)
        group.MapGet(GetAllProducts);

        // Rota 2: GET /api/Products/{id}/contents (para listar conteúdos compatíveis)
        group.MapGet(GetCompatibleContents, "{id}/contents");
    }

    // Método que lida com a Rota 1
    public Task<PaginatedList<ProductDto>> GetAllProducts(ISender sender, [AsParameters] GetProductsPaginatedQuery query)
    {
        return sender.Send(query);
    }

    // Método que lida com a Rota 2
    public Task<List<ContentDto>> GetCompatibleContents(ISender sender, Guid id)
    {
        // O 'id' da rota é passado para a query
        return sender.Send(new GetCompatibleContentsQuery(id));
    }
}