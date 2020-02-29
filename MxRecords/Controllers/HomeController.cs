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
using MxRecords.Data;
using MxRecords.Models;

namespace MxRecords.Controllers
{
    public class HomeController : Controller
    {
        private readonly SystemContext _context;
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
        public HomeController(SystemContext context, ILogger<HomeController> logger, IWebHostEnvironment env)
        {
            _env = env;
            _logger = logger;
            _context = context;

        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ViewLogFile()
        {
            ViewBag.hostlink = Request.Scheme + "://" + Request.Host + "/";
            List<Log> log = _context.Logs.OrderByDescending(w=>w.enteredTime).Take(1000).ToList();


            return View(log);
        }


        public IActionResult SaveEnteredTime(DateTime date, string session)
        {
            if(HttpContext.Session.GetString("session") == null)
            {
                Log log = new Log()
                {
                    session = session,
                    enteredTime = TimeZoneInfo.ConvertTimeToUtc(date),
                };
                
                _context.Logs.Add(log);
                _context.SaveChanges();

                HttpContext.Session.SetInt32("id", log.id);
                HttpContext.Session.SetString("session", session);

                return Content("Done");
            }
            return Content("Exist");
        }


        public IActionResult SaveFile(string fileName)
        {
            int? id = HttpContext.Session.GetInt32("id");
            if (id != null)
            {
                Log log = _context.Logs.Find(id);
                if(log.uploadedFile == null)
                {
                    log.uploadedFile = fileName;
                    _context.Logs.Update(log);
                    _context.SaveChanges();
                }

                return Content("Done");
            }

            return Content("Error");
        }

   
        [HttpGet]
        public IActionResult Result(string path) {
            ViewBag.link = Request.Scheme + "://"+  Request.Host + "/" + path;
            ViewBag.path = path;
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
            //DeleteFilesIfReachMax();

            string path = "";
            string loc = "";
            if (allfiles.Count > 0)
            {
                foreach (var files in allfiles)
                {
                    string currFilename = DateTime.Now.ToString("yyyyMMddHHmmss") + files.FileName.Replace(" ", "");
                    string Filename = currFilename.Replace("#", "Number-");
                    path = Path.Combine(_env.WebRootPath, "Files/" + Filename);
                    loc = "Files/" + Filename;

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        files.CopyTo(stream);
                    }
                }
            }
            else
                return RedirectToAction("Index", "Home");

            List<string> mails = CsvOperations.ReadCsv(path);

            List<string> mx = await MxOperations.SetListForSearchingAsync(mails);

           /* if (mx[0] == "Try Again")
                return new ContentResult{
                ContentType = "text/html",
                Content = "<h1>"+mx[1]+"</h1>"};*/

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
                    else if (mail == "" || mail == " ")
                        mx[index] = "Unknown";
                    else
                        mx[index] = "Web Mail";
                }
                index++;
            }

            CsvOperations.WriteCsv(path, mx);

            return RedirectToAction("Result","Home", new { path =  loc});
        }



        public void DeleteFilesIfReachMax()
        {
            string path = Path.Combine(_env.WebRootPath, "Files");
            int count = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Length;

            if(count > 5)
            {
                DirectoryInfo di = new DirectoryInfo(path);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
        }
    }
}
