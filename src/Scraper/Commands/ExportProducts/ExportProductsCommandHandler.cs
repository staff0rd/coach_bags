using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using McMaster.Extensions.CommandLineUtils;
using MediatR;
using Microsoft.Extensions.Logging;

namespace coach_bags_selenium
{
    public class ExportProductsCommandHandler : IRequestHandler<ExportProductsCommand> {
        private readonly DataFactory _data;
        private readonly ILogger<ExportProductsCommandHandler> _logger;
        private readonly IMediator _mediator;
        const string LOCAL_DIRECTORY = "json";

        public ExportProductsCommandHandler(
            DataFactory data,
            ILogger<ExportProductsCommandHandler> logger,
            IMediator mediator)
        {
            _data = data;
            _logger = logger;
            _mediator = mediator;
        }

        private string QUERY = @"
            with cte as (
                SELECT * FROM products
                WHERE edit = @edit 
                AND last_posted_utc IS NOT NULL
                ORDER BY last_posted_utc 
            )
            SELECT *,
            LAG(last_posted_utc,1) OVER (
                    ORDER BY last_posted_utc
                ) prev_posted_utc
            FROM cte
            ORDER BY last_posted_utc DESC";

        public async Task<Unit> Handle(ExportProductsCommand request, CancellationToken cancellationToken)
        {
            var pages = new List<Page>();

            var records = await GetProducts(request.Edit);

            var indexPageSize = records.Count() % request.PageSize is var ix && ix == 0 ? request.PageSize : ix;

            var totalPages = records.Count() / request.PageSize + (indexPageSize == request.PageSize ? 0 : 1);
            
            _logger.LogInformation("Total records: {totalRecords}, total pages: {totalPages}", records.Count(), totalPages);

            pages.Add(new Page { Name = "index.json", Products = records.Take(indexPageSize).ToArray() });

            if (records.Count() > indexPageSize)
            {
                pages.Add(GetPage(records, request.PageSize, indexPageSize));
            }

            if (request.All)
            {
                var remainingPageCount = totalPages - 2;
                var firstPages = pages.SelectMany(p => p.Products).Count();
                for (int i = 0; i < remainingPageCount; i++)
                {
                    pages.Add(GetPage(records, request.PageSize, firstPages + request.PageSize * i));
                }
            }

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                IgnoreNullValues = true,
            };

            Directory.CreateDirectory(LOCAL_DIRECTORY);

            foreach (var page in pages)
            {
                var json = JsonSerializer.Serialize(page, options);
                var jsonFile = Path.Combine(LOCAL_DIRECTORY, page.Name);
                File.WriteAllText(jsonFile, json);
                await _mediator.Send(new S3UploadCommand {
                    SourceFilePath = jsonFile,
                    TargetDirectory = $"{request.Edit}/json",
                    TargetFileName = page.Name,
                 });
            }

            return Unit.Value;
        }

        private static Page GetPage(IEnumerable<LinkedProduct> records, int pageSize, int skip)
        {
            return new Page
            { 
                Name = $"{records.Skip(skip).First().PageName}.json", 
                Products = records.Skip(skip).Take(pageSize).ToArray()
            };
        }

        private async Task<IEnumerable<LinkedProduct>> GetProducts(Edit edit)
        {
            using (var connection = _data.GetConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@edit", edit, System.Data.DbType.Int16);
                var result = await connection.QueryAsync<LinkedProduct>(QUERY, parameters);
                return result;
            }
        }
    }
}
