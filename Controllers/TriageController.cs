using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IpczApp.Models;
using IpczApp.Services;

namespace IpczApp.Controllers
{
    public class TriageController : Controller
    {
        private readonly TriageService _triageService;
        private readonly DatabaseService _databaseService;
        private readonly GeminiService _geminiService;
        private readonly EmailService _emailService;
        private readonly PdfService _pdfService;

        public TriageController(
            TriageService triageService,
            DatabaseService databaseService,
            GeminiService geminiService,
            EmailService emailService,
            PdfService pdfService)
        {
            _triageService = triageService;
            _databaseService = databaseService;
            _geminiService = geminiService;
            _emailService = emailService;
            _pdfService = pdfService;
        }

        // 1. Obrazovka výběru role
        [HttpGet]
        public IActionResult Index()
        {
            var model = new RoleSelectionViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(RoleSelectionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            TempData["SelectedRole"] = model.SelectedRole;
            TempData["IsManagement"] = model.IsManagement;

            return RedirectToAction("Test");
        }

        // 2. Obrazovka s 12 otázkami benchmark testu
        [HttpGet]
        public IActionResult Test()
        {
            string role = TempData["SelectedRole"] as string ?? string.Empty;
            bool isManagement = (TempData["IsManagement"] as bool?) ?? false;

            if (string.IsNullOrEmpty(role))
            {
                return RedirectToAction("Index");
            }

            // Uchováme TempData pro POST
            TempData.Keep("SelectedRole");
            TempData.Keep("IsManagement");

            var model = new TestViewModel
            {
                Role = role,
                IsManagement = isManagement,
                Questions = _triageService.GetQuestions()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Test(TestViewModel model)
        {
            if (model.Answers == null || model.Answers.Count < 12)
            {
                ModelState.AddModelError("", "Prosím odpovězte na všechny 12 otázek.");
                model.Questions = _triageService.GetQuestions();
                return View(model);
            }

            // Vyhodnocení testu
            var result = _triageService.EvaluateTest(model);

            // Dynamická AI analýza nejslabších stránek od Gemini API
            var weakNames = result.WeakestHabits.Select(h => h.HabitName).ToList();
            result.AiAnalysis = await _geminiService.GenerateWeaknessAnalysisAsync(result.Role, result.OverallScore, weakNames);

            // Uložení do databáze přes ADO.NET (Ipcz_SaveTriageResult)
            try
            {
                result.ResultId = _databaseService.SaveTriageResult(result);
            }
            catch (Exception ex)
            {
                // Pokud DB selže (např. při lokálním vývoji bez připojení), pokračujeme bez pádu
                ViewBag.DbError = ex.Message;
            }

            // Uložíme do TempData / Session pro Results View
            TempData["ResultModelJson"] = System.Text.Json.JsonSerializer.Serialize(result);

            return RedirectToAction("Results");
        }

        // 3. Obrazovka vyhodnocení (Recommender)
        [HttpGet]
        public IActionResult Results()
        {
            string json = TempData["ResultModelJson"] as string ?? string.Empty;
            if (string.IsNullOrEmpty(json))
            {
                return RedirectToAction("Index");
            }

            TempData.Keep("ResultModelJson");

            var result = System.Text.Json.JsonSerializer.Deserialize<ResultViewModel>(json);
            return View(result);
        }

        // Odeslání výsledků na e-mail
        [HttpPost]
        public async Task<IActionResult> SendEmail(string emailAddress)
        {
            string json = TempData["ResultModelJson"] as string ?? string.Empty;
            TempData.Keep("ResultModelJson");

            if (string.IsNullOrEmpty(json) || string.IsNullOrEmpty(emailAddress))
            {
                return RedirectToAction("Results");
            }

            var result = System.Text.Json.JsonSerializer.Deserialize<ResultViewModel>(json)!;

            var (success, message) = await _emailService.SendResultEmailAsync(emailAddress, result);
            result.EmailStatusMessage = message;
            result.EmailSentSuccess = success;
            result.EmailAddress = emailAddress;

            // Aktualizujeme TempData s hláškou o e-mailu
            TempData["ResultModelJson"] = System.Text.Json.JsonSerializer.Serialize(result);

            return View("Results", result);
        }

        // Stáhnout výsledky jako PDF dokument
        [HttpGet]
        public IActionResult DownloadPdf()
        {
            string json = TempData["ResultModelJson"] as string ?? string.Empty;
            TempData.Keep("ResultModelJson");

            if (string.IsNullOrEmpty(json))
            {
                return RedirectToAction("Index");
            }

            var result = System.Text.Json.JsonSerializer.Deserialize<ResultViewModel>(json)!;
            byte[] pdfBytes = _pdfService.GenerateTriagePdf(result);

            return File(pdfBytes, "application/pdf", $"Vysledky_Triaze_AI_Gramotnosti_{result.OverallScore:F0}pct.pdf");
        }

        // 4. Obrazovka statistik
        [HttpGet]
        public IActionResult Stats()
        {
            StatsViewModel stats;
            try
            {
                stats = _databaseService.GetStatistics();

                // Pokud v databázi ještě nejsou žádná data, automaticky vygenerujeme testovací
                if (stats.TotalCount == 0)
                {
                    _databaseService.SeedDummyData(50);
                    stats = _databaseService.GetStatistics();
                    stats.SystemMessage = "V databázi nebyly žádné záznamy — automaticky bylo vygenerováno 50 anonymizovaných testovacích dat pro vygenerování přehledů.";
                }
            }
            catch (Exception ex)
            {
                stats = new StatsViewModel
                {
                    SystemMessage = $"Chyba při načítání statistik z databáze: {ex.Message}"
                };
            }

            return View(stats);
        }

        // Ruční vygenerování testovacích dat
        [HttpPost]
        public IActionResult SeedData()
        {
            try
            {
                _databaseService.SeedDummyData(50);
                TempData["StatsMessage"] = "Úspěšně vygenerováno 50 nových testovacích záznamů v databázi.";
            }
            catch (Exception ex)
            {
                TempData["StatsMessage"] = $"Chyba při generování dat: {ex.Message}";
            }

            return RedirectToAction("Stats");
        }
    }
}
