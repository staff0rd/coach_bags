using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using coach_bags_selenium.Data;
using McMaster.Extensions.CommandLineUtils;
using MediatR;

namespace coach_bags_selenium
{
    [Command("images")]
    public class GetMetadataCommand : Request<GetMetadataCommandResult>
    {
        public Product Product { get; set; }

        public DateTime Now { get; set; } = DateTime.UtcNow;
    }
}