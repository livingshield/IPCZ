using System;
using System.Collections.Generic;
using System.Linq;
using IpczApp.Models;

namespace IpczApp.Services
{
    public class TriageService
    {
        public List<QuestionModel> GetQuestions()
        {
            return new List<QuestionModel>
            {
                // Návyk 1: Včasné zapojení AI
                new QuestionModel { Id = 1, HabitId = 1, HabitName = "Včasné zapojení AI", QuestionText = "Jak často se snažíte zapojit AI nástroje (např. pro osnovu, inspiraci či rešerši) hned na začátku řešení nového administrativního či medicínského úkolu?" },
                new QuestionModel { Id = 2, HabitId = 1, HabitName = "Včasné zapojení AI", QuestionText = "Když obdržíte komplexní zprávu, odborný článek či metodický pokyn, využijete AI pro jeho rychlé shrnutí dříve, než se pustíte do podrobného čtení?" },

                // Návyk 2: Experimentování
                new QuestionModel { Id = 3, HabitId = 2, HabitName = "Experimentování", QuestionText = "Zkoušíte při práci s AI nástroji aktivně různé způsoby zadání (prompty) nebo různé modely, abyste zjistili, který dává nejlepší výsledky pro daný klinický či provozní kontext?" },
                new QuestionModel { Id = 4, HabitId = 2, HabitName = "Experimentování", QuestionText = "Testujete využití AI i na úkoly, o kterých si nejste jisti, zda je AI zvládne (např. formátování nestrukturovaných poznámek z vizity do standardní propouštěcí zprávy)?" },

                // Návyk 3: Iterace
                new QuestionModel { Id = 5, HabitId = 3, HabitName = "Iterace", QuestionText = "Když vám AI nedá uspokojivou odpověď na první pokus, upřesňujete své zadání (např. dodáním detailů, opravou chyb, vymezením role) a pokračujete v konverzaci?" },
                new QuestionModel { Id = 6, HabitId = 3, HabitName = "Iterace", QuestionText = "Používáte metodu postupného upřesňování (např. nejdřív vygenerovat strukturu, pak nechat AI rozpracovat jednotlivé sekce) při tvorbě delších textů nebo prezentací?" },

                // Návyk 4: Ověřování výstupů
                new QuestionModel { Id = 7, HabitId = 4, HabitName = "Ověřování výstupů", QuestionText = "Kontrolujete si faktickou správnost a odborné detaily (např. citace, lékové interakce, shodu s českou legislativou) v textech generovaných AI, než je použijete v praxi?" },
                new QuestionModel { Id = 8, HabitId = 4, HabitName = "Ověřování výstupů", QuestionText = "Vyhledáváte aktivně halucinace (vymyšlené informace) nebo logické chyby ve výstupech, které vám AI poskytne k odborným tématům?" },

                // Návyk 5: Uložené postupy
                new QuestionModel { Id = 9, HabitId = 5, HabitName = "Uložené postupy", QuestionText = "Máte vytvořený a uložený vlastní seznam osvědčených promptů či šablon (např. pro psaní propouštěcí zpráv, e-mailů pacientům), které opakovaně používáte?" },
                new QuestionModel { Id = 10, HabitId = 5, HabitName = "Uložené postupy", QuestionText = "Využíváte pokročilé funkce jako Custom Instructions (vlastní instrukce) nebo specializované GPTs/agenty přizpůsobené vaší každodenní zdravotnické praxi?" },

                // Návyk 6: Sdílení workflow
                new QuestionModel { Id = 11, HabitId = 6, HabitName = "Sdílení workflow", QuestionText = "Sdílíte své úspěšné prompty, postupy nebo zkušenosti s AI nástroji se svými kolegy v ordinaci, na oddělení nebo v týmu?" },
                new QuestionModel { Id = 12, HabitId = 6, HabitName = "Sdílení workflow", QuestionText = "Spolupracujete s kolegy na vytváření společných šablon (workflow) pro efektivní integraci AI do procesů vaší ambulance či nemocničního oddělení?" }
            };
        }

        public ResultViewModel EvaluateTest(TestViewModel test)
        {
            var result = new ResultViewModel
            {
                Role = test.Role,
                IsManagement = test.IsManagement,
                CreatedAt = DateTime.UtcNow
            };

            var questions = GetQuestions();
            var habitNames = new Dictionary<int, string>
            {
                { 1, "Včasné zapojení AI" },
                { 2, "Experimentování" },
                { 3, "Iterace" },
                { 4, "Ověřování výstupů" },
                { 5, "Uložené postupy" },
                { 6, "Sdílení workflow" }
            };

            int totalSum = 0;

            for (int habitId = 1; habitId <= 6; habitId++)
            {
                var qIds = questions.Where(q => q.HabitId == habitId).Select(q => q.Id).ToList();
                int rawSum = 0;
                foreach (var qId in qIds)
                {
                    if (test.Answers.TryGetValue(qId, out int val))
                    {
                        rawSum += Math.Clamp(val, 1, 5);
                    }
                    else
                    {
                        rawSum += 1; // Fallback min
                    }
                }

                totalSum += rawSum;

                // Přepočet 2 až 10 na 0 až 100%
                double pct = (rawSum - 2) * 12.5;

                result.HabitScores.Add(new HabitScoreModel
                {
                    HabitId = habitId,
                    HabitName = habitNames[habitId],
                    RawScore = rawSum,
                    PercentageScore = Math.Round(pct, 1),
                    Description = GetHabitDescription(habitId, pct)
                });
            }

            // Celkové skóre SP Score (0 - 100)
            double overallPct = (totalSum - 12) / 48.0 * 100.0;
            result.OverallScore = Math.Round(overallPct, 1);

            // Zařazení do úrovní
            if (result.OverallScore < 40)
            {
                result.Level = "Level 1 (Základní)";
                result.LevelDescription = "Jste na začátku své cesty s AI. Doporučujeme seznámit se se základními možnostmi a vyzkoušet volné e-learningové kurzy.";
            }
            else if (result.OverallScore < 75)
            {
                result.Level = "Level 2 (Pokročilý)";
                result.LevelDescription = "AI nástroje již běžně zkoušíte. Pro další rozvoj doporučujeme praktické workshopy IPVZ a účast v AI čtvrthodince.";
            }
            else
            {
                result.Level = "Level 3 (Specialista)";
                result.LevelDescription = "Máte výborné návyky při práci s AI! Doporučujeme specializovaná školení a zapojení do pilotních projektů.";
            }

            // Masterclass pro management nezdravotnického personálu
            result.ShowMasterclass = (test.Role == "Nezdravotnický personál nemocnice" && test.IsManagement);

            // Určení 2 nejslabších oblastí
            result.WeakestHabits = result.HabitScores
                .OrderBy(h => h.PercentageScore)
                .Take(2)
                .ToList();

            // Doporučené kroky
            result.RecommendedSteps = GetRecommendedSteps(result.Level, result.Role);

            return result;
        }

        private string GetHabitDescription(int habitId, double pct)
        {
            if (pct < 40) return "Nízké zapojení — doporučujeme zařadit tento návyk do týdenní praxe.";
            if (pct < 75) return "Střední úroveň — návyk využíváte příležitostně, zkuste jej systematizovat.";
            return "Vynikající úroveň — tento návyk máte plně osvojený!";
        }

        private List<string> GetRecommendedSteps(string level, string role)
        {
            var steps = new List<string>();

            if (level.Contains("Level 1"))
            {
                steps.Add("Absolvovat bezplatný e-learningový kurz 'Základy AI pro zdravotnictví' přímo na portálu aivezdravotnictvi.cz.");
                steps.Add("Vyzkoušet si vytvořit první 3 prompty pro sumarizaci dlouhého článku či propouštěcí zprávy.");
                steps.Add("Přihlásit se na úvodní webinář IPVZ: 'AI jako pravá ruka v ordinaci a na oddělení'.");
            }
            else if (level.Contains("Level 2"))
            {
                steps.Add("Zapojit se do pravidelných online setkání 'AI čtvrthodinka IPVZ' a sdílet zkušenosti s kolegy.");
                steps.Add("Vytvořit si vlastní knihovnu uložených promptů (Custom Instructions) pro opakované administrativní úkoly.");
                steps.Add("Zúčastnit se praktického workshopu IPVZ zaměřeného na bezpečné ověřování AI výstupů a prevenci halucinací.");
            }
            else
            {
                steps.Add("Zapojit se do pilotních workshopů IPVZ zaměřených na testování pokročilých AI agentů a LLM modelů.");
                steps.Add("Působit jako AI ambasador ve vaší organizaci a pomáhat kolegům se zaváděním osvědčených postupů.");
                steps.Add("Přihlásit se na certifikovaný kurz 'Architektura a integrace AI systémů ve zdravotnictví'.");
            }

            return steps;
        }
    }
}
