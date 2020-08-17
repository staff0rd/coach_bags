using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Dom;
using coach_bags_selenium.Data;

namespace coach_bags_selenium
{
    public class OutnetProductDetail
    {
        private readonly IDocument _document;

        public IEnumerable<string> Images => _document.QuerySelectorAll("div[class$=Thumbnails] div[class$=thumbnailImage] img")
            .Select(e => e.GetAttribute("srcset"))
            .Select(src => src.GetGroupMatches(@"(\/\/.+w\d+_q80\.jpg) 1020w").FirstOrDefault())
            .Where(src => src != null)
            .Select(src => "https:" + src);

        public OutnetProductDetail(IDocument document)
        {
            _document = document;
        }
    }
}
