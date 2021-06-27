using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IO;
using UploadDownload_DocumentStore.Models;

namespace UploadDownload_DocumentStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly DateiRepository repository = new DateiRepository();

        public IActionResult Index()
        {
            return View(repository.DateiListeAbrufen());
        }

        [HttpPost]
        public ActionResult Index(IFormFile dieDatei)
        {
            if (dieDatei != null && dieDatei.Length > 0)
            {
                ViewBag.Meldung = repository.DateiInDatenbankSchreiben(dieDatei);
            }
            else
            {
                ViewBag.Meldung = "Es wurde keine Datei angegeben oder die Datei ist leer.";
            }

            return View(repository.DateiListeAbrufen());
        }

        public FileResult Download(int id, string dateiTyp, string dateiName)
        {
            // Inhalt der Datei aus der DB lesen
            Stream inhalt = repository.DateiAusDBLaden(id);

            return File(inhalt, dateiTyp, dateiName);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}