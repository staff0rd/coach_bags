using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using coach_bags_selenium.Data;
using McMaster.Extensions.CommandLineUtils;
using MediatR;

namespace coach_bags_selenium
{
    [Command("images")]
    public class GetImagesCommand : Request<GetImagesCommandResult>
    {
        [Option("-c|--category", CommandOptionType.SingleValue)]
        [Required]
        public Category Category { get; set; }

        [Option("-s|--source", CommandOptionType.SingleValue)]
        [Required]
        public string SourceUrl { get; set; }
        public DateTime Now { get; set; } = DateTime.UtcNow;
    }
}