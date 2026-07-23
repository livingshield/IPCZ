using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using IpczApp.Models;

namespace IpczApp.Services
{
    public class PdfService
    {
        /// <summary>
        /// Vygeneruje strukturovaný PDF dokument obsahující výsledky triáže a studijní plán.
        /// </summary>
        public byte[] GenerateTriagePdf(ResultViewModel result)
        {
            var sb = new StringBuilder();

            // Konstrukce obsahu pro PDF stream bez ošklivých ASCII oddělovačů
            sb.AppendLine("PORTAL AIVEZDRAVOTNICTVI.CZ - VSTUPNI TRIAZ AI GRAMOTNOSTI");
            sb.AppendLine("---------------------------------------------------------");
            sb.AppendLine($"Role: {SanitizeText(result.Role)}");
            sb.AppendLine($"Uroven: {SanitizeText(result.Level)}");
            sb.AppendLine($"Celkove SP Skore: {result.OverallScore:F0} / 100 %");
            sb.AppendLine($"Datum vytvoreni: {result.CreatedAt:dd.MM.yyyy HH:mm}");
            sb.AppendLine();
            sb.AppendLine("DETAILNI ROZPAD PODLE 6 NAVYKU AIBILITY");
            sb.AppendLine("----------------------------------------");

            foreach (var h in result.HabitScores)
            {
                sb.AppendLine($"{h.HabitId}. {SanitizeText(h.HabitName)}: {h.PercentageScore:F0} %");
            }

            sb.AppendLine();
            sb.AppendLine("AI ANALYZA NEJSLABSICH OBLASTI (GEMINI API)");
            sb.AppendLine("--------------------------------------------");
            if (!string.IsNullOrEmpty(result.AiAnalysis))
            {
                sb.AppendLine(SanitizeText(result.AiAnalysis));
            }
            else
            {
                sb.AppendLine("Doporucujeme zamerit se na pravidelne zkouseni novych postupu a overovani vystupu.");
            }

            sb.AppendLine();
            sb.AppendLine("DOPORUCENY STUDIJNI PLAN VZDELAVANI");
            sb.AppendLine("----------------------------------");
            for (int i = 0; i < result.RecommendedSteps.Count; i++)
            {
                sb.AppendLine($"Krok {i + 1}: {SanitizeText(result.RecommendedSteps[i])}");
            }

            if (result.ShowMasterclass)
            {
                sb.AppendLine();
                sb.AppendLine("EXKLUZIVNI NABIDKA: Vzhledem k roli v managementu Vas zveme do programu Masterclass (Management).");
            }

            sb.AppendLine();
            sb.AppendLine("---------------------------------------------------------");
            sb.AppendLine("Institut postgradualniho vzdelavani ve zdravotnictvi (IPVZ) & MZd CR");

            string textContent = sb.ToString();

            return CreatePdfFromText(textContent);
        }

        private string SanitizeText(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            string text = input.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (char c in text)
            {
                var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        private byte[] CreatePdfFromText(string text)
        {
            using var ms = new MemoryStream();
            using var writer = new StreamWriter(ms, Encoding.ASCII);

            string[] lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            writer.WriteLine("%PDF-1.4");
            writer.WriteLine("%\xC3\xC2\xC2\xC3");

            writer.WriteLine("1 0 obj");
            writer.WriteLine("<< /Type /Catalog /Pages 2 0 R >>");
            writer.WriteLine("endobj");

            writer.WriteLine("2 0 obj");
            writer.WriteLine("<< /Type /Pages /Kids [3 0 R] /Count 1 >>");
            writer.WriteLine("endobj");

            writer.WriteLine("4 0 obj");
            writer.WriteLine("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>");
            writer.WriteLine("endobj");

            var streamBuilder = new StringBuilder();
            streamBuilder.AppendLine("BT");
            streamBuilder.AppendLine("/F1 11 Tf");
            streamBuilder.AppendLine("14 TL");
            streamBuilder.AppendLine("50 780 Td");

            foreach (var line in lines)
            {
                string safeLine = line.Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");
                if (safeLine.StartsWith("PORTAL") || safeLine.StartsWith("DETAILNI") || safeLine.StartsWith("AI ANALYZA") || safeLine.StartsWith("DOPORUCENY"))
                {
                    streamBuilder.AppendLine($"/F1 12 Tf ({safeLine}) Tj T* /F1 11 Tf");
                }
                else
                {
                    streamBuilder.AppendLine($"({safeLine}) Tj T*");
                }
            }

            streamBuilder.AppendLine("ET");

            string streamData = streamBuilder.ToString();
            byte[] streamBytes = Encoding.ASCII.GetBytes(streamData);

            writer.WriteLine("3 0 obj");
            writer.WriteLine("<< /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Resources << /Font << /F1 4 0 R >> >> /Contents 5 0 R >>");
            writer.WriteLine("endobj");

            writer.WriteLine("5 0 obj");
            writer.WriteLine($"<< /Length {streamBytes.Length} >>");
            writer.WriteLine("stream");
            writer.Flush();

            ms.Write(streamBytes, 0, streamBytes.Length);
            ms.Position = ms.Length;

            writer.WriteLine();
            writer.WriteLine("endstream");
            writer.WriteLine("endobj");

            long xrefOffset = ms.Position;
            writer.WriteLine("xref");
            writer.WriteLine("0 6");
            writer.WriteLine("0000000000 65535 f ");
            writer.WriteLine("0000000009 00000 n ");
            writer.WriteLine("0000000058 00000 n ");
            writer.WriteLine("0000000250 00000 n ");
            writer.WriteLine("0000000115 00000 n ");
            writer.WriteLine("0000000380 00000 n ");

            writer.WriteLine("trailer");
            writer.WriteLine("<< /Size 6 /Root 1 0 R >>");
            writer.WriteLine("startxref");
            writer.WriteLine(xrefOffset);
            writer.WriteLine("%%EOF");

            writer.Flush();
            return ms.ToArray();
        }
    }
}
