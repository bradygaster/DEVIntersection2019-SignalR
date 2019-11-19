using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Streaming.Pages
{
    public class Drive : PageModel
    {
        private readonly ILogger<Drive> _logger;

        public Drive(ILogger<Drive> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
    }
}
