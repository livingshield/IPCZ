using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace IpczApp.Services
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GeminiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["AI:GeminiApiKey"] ?? string.Empty;
        }

        /// <summary>
        /// Vygeneruje dynamické hodnocení a personalizované radu pro 2 nejslabší oblasti uživatele.
        /// </summary>
        public async Task<string> GenerateWeaknessAnalysisAsync(string role, double overallScore, List<string> weakHabits)
        {
            if (string.IsNullOrEmpty(_apiKey) || weakHabits == null || weakHabits.Count == 0)
            {
                return GetFallbackAnalysis(role, weakHabits ?? new List<string>());
            }

            try
            {
                string habitsText = string.Join(" a ", weakHabits);
                string prompt = $"Jsi expert na AI gramotnost ve zdravotnictví. Uživatel s rolí '{role}' dosáhl celkového skóre AI gramotnosti {overallScore:F0}/100. " +
                                $"Jelo dvě nejslabší oblasti jsou: {habitsText}. " +
                                $"Napiš pro něj stručné (max 3-4 věty), konstruktivní a povzbudivé hodnocení v češtině. " +
                                $"Vysvětli mu, proč jsou tyto dvě oblasti pro jeho roli ve zdravotnictví klíčové a dej mu 1 konkrétní praktický tip, jak je zlepšit. Odpověz přímo textem hodnocení bez nadpisů a bez úvodních pozdravů.";

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                        }
                    }
                };

                string jsonContent = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={_apiKey}";
                var response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    using (JsonDocument doc = JsonDocument.Parse(jsonResponse))
                    {
                        var root = doc.RootElement;
                        var candidates = root.GetProperty("candidates");
                        if (candidates.GetArrayLength() > 0)
                        {
                            var parts = candidates[0].GetProperty("content").GetProperty("parts");
                            if (parts.GetArrayLength() > 0)
                            {
                                return parts[0].GetProperty("text").GetString()?.Trim() ?? GetFallbackAnalysis(role, weakHabits);
                            }
                        }
                    }
                }
            }
            catch
            {
                // V případě výpadku API bezpečně vrátíme záložní text
            }

            return GetFallbackAnalysis(role, weakHabits);
        }

        private string GetFallbackAnalysis(string role, List<string> weakHabits)
        {
            string habitsText = (weakHabits != null && weakHabits.Count > 0) ? string.Join(" a ", weakHabits) : "vybraných oblastech";
            return $"Jako {role} máte skvělý potenciál dále posouvat svou AI gramotnost. Největší prostor pro zlepšení u vás vidíme v oblastech **{habitsText}**. " +
                   $"Doporučujeme zaměřit se na pravidelné zkoušení nových postupů a ověřování výstupů v běžné praxi.";
        }
    }
}
