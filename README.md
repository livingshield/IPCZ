# Prototyp vstupní triáže portálu www.aivezdravotnictvi.cz

Funkční webový prototyp vstupní triáže AI gramotnosti pro zdravotnictví vyvinutý v rámci výběrového řízení pro **Institut postgraduálního vzdělávání ve zdravotnictví (IPVZ)** a **Ministerstvo zdravotnictví ČR**.

---

## 🚀 O projektu

Aplikace slouží jako úvodní rozcestník AI gramotnosti na portálu `aivezdravotnictvi.cz`. Vychází z české metodiky **Aibility „Superpowered Professional™“** a vyhodnocuje 6 klíčových návyků práce s AI v medicínském a nemocničním kontextu.

### Hlavní funkce prototypu:
1. **Výběr role**: 5 vstupních rolí (Pacient, Zdravotník, IT odborník/dodavatel, Tvůrce politik a média, Nezdravotnický personál nemocnice) + příznak managementu.
2. **Benchmark test**: 12 otázek hodnocených na škále 1–5 (2 otázky na každý ze 6 návyků Aibility upravené pro zdravotnictví).
3. **SP Score & Výsledkové úrovně**:
   - Skóre 0–100 pro každý návyk + celkové skóre.
   - Zařazení do úrovní:
     - `0 - 39` → **Level 1 (Základní)** — volné e-learningy bez registrace.
     - `40 - 74` → **Level 2 (Pokročilý)** — webináře a workshopy IPVZ, AI čtvrthodinka.
     - `75 - 100` → **Level 3 (Specialista)** — specializovaná školení a pilotní workshopy.
   - **Masterclass (Management)** — speciální nabídka pro nezdravotnické manažery nemocnic.
4. **Recommender (Doporučovací plán)**: 2–3 konkrétní doporučené kroky a kurzy + dynamická **AI analýza 2 nejslabších oblastí** generovaná přes **Google Gemini API**.
5. **Ukládání do databáze**: Anonymizované uložení výsledků do MS SQL databáze pomocí vlastního schématu a uložených procedur (`Ipcz_SaveTriageResult`).
6. **Statistiky & Srovnání**: Obrazovka s anonymizovaným srovnáním (průměrná skóre dle rolí, distribuce úrovní, průměry návyků) počítaná přes proceduru `Ipcz_GetTriageStatistics`.
7. **E-mailing**: Možnost zaslání vygenerovaného studijního plánu na e-mail uživatele (SMTP MailKit).

---

## 🛠 Použité technologie

| Technologie | Účel |
|---|---|
| **ASP.NET Core MVC (.NET 8)** | Webový framework (klasická serverová C# architektura) |
| **ADO.NET (Microsoft.Data.SqlClient)** | Čistý a transparentní přístup k databázi přes uložené procedury |
| **MS SQL Server** | Výrobní databáze (sdílená DB `db4937` na `sql8.aspone.cz`) |
| **Google Gemini 1.5 Flash** | Dynamické AI vyhodnocení nejslabších oblastí uživatele |
| **MailKit** | Odesílání studijního plánu e-mailem přes SMTP |
| **Medical-Tech Design System** | Custom Vanilla CSS (Glassmorphism, Outfit + Inter fonty, responzivní grid) |

---

## 📂 Databázová logika (SQL skripty)

Ve složce `SQL/` naleznete kompletní databázové skripty:
* `SQL/schema.sql`: Vytvoření tabulky `Ipcz_TriageResults` s prefixem `Ipcz_` pro izolaci na sdíleném serveru.
* `SQL/procedures.sql`: Uložené procedury:
  - `Ipcz_SaveTriageResult`: Zápis nového anonymního výsledku triáže a vrácení vygenerovaného ID.
  - `Ipcz_GetTriageStatistics`: Výpočet celkových statistik, průměrů dle rolí, distribuci úrovní a průměrů jednotlivých návyků.

---

## 🏃 Postup spuštění (Krok za krokem)

### Požadavky:
* `.NET 8.0 SDK` (nebo novější)

### 1. Klonování / Rozbalení
Stáhněte si zdrojové kódy ze složky nebo git repozitáře.

### 2. Konfigurace (`appsettings.json`)
Ověřte nastavení v `appsettings.json`. Soubor obsahuje připojení k MS SQL databázi, klíč k Gemini API a SMTP serveru.

```json
{
  "ConnectionStrings": {
    "ProductionConnection": "Server=sql8.aspone.cz;Database=db4937;User Id=db4937;Password=VÁŠ_SQL_PASSWORD;Encrypt=False"
  },
  "AI": {
    "GeminiApiKey": "VÁŠ_GEMINI_API_KLÍČ"
  }
}
```

### 3. Spuštění aplikace
Otevřete terminál ve složce projektu a spusťte:

```bash
dotnet restore
dotnet run
```

Aplikace se spustí na adrese `https://localhost:7057` nebo `http://localhost:5057`.

---

## 🤖 Zpráva o práci s AI nástroji

Při vývoji tohoto prototypu byly využity pokročilé AI nástroje (**Google Antigravity / Gemini 3.6 Flash / Cursor / GitHub Copilot**).

### 1. Použité AI nástroje & Úlohy
* **Google Antigravity / Gemini 3.6 Flash**: Plánování architektury, generování klinických otázek pro zdravotnický kontext, tvorba SQL schématu a uložených procedur, návrh Medical-Tech design systému, integrace Gemini API pro dynamické AI hodnocení.
* **Cursor / Copilot**: Autokompletace C# kódu a rychlá refaktorace Controllerů a ViewModelů.

### 2. Klíčové prompty a zavedená pravidla (CLAUDE.md / .cursorrules)
* **Designové pravidlo**: *"Create a state-of-the-art Medical-Tech UI using Vanilla CSS, Glassmorphism, Outfit/Inter typography and dynamic score bars. Avoid default generic styles."*
* **Architektonické pravidlo**: *"Use classic C# with ASP.NET Core MVC and pure ADO.NET (SqlConnection/SqlCommand) to execute stored procedures without heavy ORM magic, ensuring the code is transparent and easy to explain."*
* **Izolační pravidlo**: *"All database objects must use the prefix Ipcz_ to prevent collision with other applications on the shared SQL server."*

### 3. Co bylo po AI kontrolováno a revidováno
* **Faktická správnost SQL procedur**: Ručně ověřena správnost agregačních dotazů a přepočtu skóre (přepočet škály 2-10 bodů na procenta `(Suma - 2) * 12.5`).
* **Handling chyb u AI API**: Implementován fallback mechanismus v `GeminiService.cs` pro případ, že API selže nebo je nedostupné, aby aplikace nespadla.
* **Responzivita & Diakritika**: Vizuální kontrola vykreslování v Chrome DevTools na mobilních i desktopových rozlišeních.

### 4. Co si z postupu uložit pro příště
* **Generování SQL procedur pomocí AI**: AI je extrémně efektivní v psaní čistého T-SQL schématu a procedur včetně ošetření parametrů.
* **Hybridní recommender (Pravidla + AI)**: Kombinace deterministických pravidel (zařazení do úrovní 1–3) s dynamickou AI analýzou (Gemini komentář pro 2 nejslabší oblasti) vytváří pro uživatele nejlepší možný zážitek.
