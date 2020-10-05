using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using coach_bags_selenium;
using MediatR;

public class TestMetadataCommandHandler : IRequestHandler<TestMetadataCommand>
{
    private readonly DataFactory _data;
    private readonly IMediator _mediator;

    public TestMetadataCommandHandler(DataFactory data, IMediator mediator)
    {
        _data = data;
        _mediator = mediator;
    }
    public async Task<Unit> Handle(TestMetadataCommand request, CancellationToken cancellationToken)
    {
        var product = _data.GetDatabaseContext()
            .Products
            .Where(p => p.CategoryId == request.Category.Id && p.Id == request.Id)
            .First();
        
        var result = await _mediator.Send(new GetMetadataFromPageCommand {Product = product });

        return Unit.Value;
    }
}