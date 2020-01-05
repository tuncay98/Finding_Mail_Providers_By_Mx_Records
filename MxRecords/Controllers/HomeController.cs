using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DnsClient;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MxRecords.Models;

namespace MxRecords.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IWebHostEnvironment _env;

        private Dictionary<string, string> companies = new Dictionary<string, string>() {
            { "google", "Google" },
            { "godaddy", "GoDaddy"},
            { "hotmail", "Hotmail"},
            { "outlook", "Outlook"},
            { "amazon", "Amazon"},
            { "apple", "Apple"},
            { "azet","Azet" },
            { "aol", "Aol" },
            { "bol.com.br", "BOL"},
            { "messagingengine.com", "Fastmail"},
            { "freenet", "Freenet"},
            { "gmx.net", "GMX"},
            { "interia", "Interia"},
            { "laposte", "laposte"},
            { "libero", "Libero"},
            { "locaweb.com.br", "Locaweb"},
            { "mail.com", "Mail.com"},
            { "mail.ru", "Mail.ru"},
            { "net-c.com", "Netcourrier"},
            { "tlen.pl", "O2.pl"},
            { "seznam", "Seznam.cz"},
            { "twcmail", "Twcmail.de"},
            { "virgilio","Virgilio"},
            { "yahoodns.net", "Yahoo"},
            { "yandex.ru", "Yandex"},
            { "zoho", "Zoho"},
            { "protonmail", "ProtonMail"},
            { "tutanota", "Tutanota"}

        };
        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment env)
        {
            _env = env;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Result(string path) {
            ViewBag.link = Request.Scheme + "://"+  Request.Host + "/" + path;
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> Analysis(List<IFormFile> allfiles)
        {
            string path = "";
            string loc = "";
            if (allfiles.Count > 0)
            {
                foreach (var files in allfiles)
                {
                    string Filename = DateTime.Now.ToString("yyyyMMddHHmmssffff") + files.FileName.Replace(" ", "");
                    path = Path.Combine(_env.WebRootPath, "Files/" + Filename);
                    loc = "Files/" + Filename;

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        files.CopyTo(stream);
                    }
                }
            }

            List<string> mails = CsvOperations.ReadCsv(path);

            List<string> mx = await MxOperations.SetListForSearchingAsync(mails);

            int index = 0;
            foreach (string mail in mx.ToArray())
            {
                foreach (KeyValuePair<string, string> pair in companies)
                {
                    if (mail.Contains(pair.Key))
                    {
                        mx[index] = pair.Value;
                        break;
                    }
                    else
                    {
                        mx[index] = "Web Mail";
                    }
                }
                index++;
            }

            CsvOperations.WriteCsv(path, mx);

            return RedirectToAction("Result","Home", new { path =  loc});
        }
    }
}
