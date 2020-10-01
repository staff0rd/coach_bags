using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using coach_bags_selenium;
using coach_bags_selenium.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class TestAllCommandHandler : IRequestHandler<TestAllCommand>
{
    private readonly IMediator _mediator;
    private readonly DataFactory _data;

    public TestAllCommandHandler(IMediator mediator, DataFactory data)
    {
        _mediator = mediator;
        _data = data;
    }

    public async Task<Unit> Handle(TestAllCommand request, CancellationToken cancellationToken)
    {
        _data.GetDatabaseContext().Database.Migrate();

        var categories = Enumeration.GetAll<ProductCategory>().ToList();

        foreach (var category in categories)
        {
            await _mediator.Send(new ScrapeAndTweetCommand { CategoryName = category.DisplayName, Count = 30, PrepareOnly = true });
        }

        return Unit.Value;
    }
}