# 📖 Uživatelská a Průvodní Dokumentace k Aplikaci IPCZ
## Vstupní Triáž AI Gramotnosti pro Zdravotnictví (aivezdravotnictvi.cz)

Vytvořeno pro **Institut postgraduálního vzdělávání ve zdravotnictví (IPVZ)** a **Ministerstvo zdravotnictví ČR**.

---

## 📋 Obsah
1. [Účel a Cíl Aplikace](#-účel-a-cíl-aplikace)
2. [Metodika Aibility & Hodnocení](#-metodika-aibility--hodnocení)
3. [Průvodce Používáním Aplikace](#-průvodce-používáním-aplikace)
   - [Krok 1: Výběr Role ve Zdravotnictví](#krok-1-výběr-role-ve-zdravotnictví)
   - [Krok 2: Vyplnění Benchmark Testu](#krok-2-vyplnění-benchmark-testu)
   - [Krok 3: Vyhodnocení & Výsledky](#krok-3-vyhodnocení--výsledky)
   - [Krok 4: Výběr ze 7 Barevných Témat](#krok-4-výběr-ze-7-barevných-témat)
   - [Krok 5: Grafický Export do PDF](#krok-5-grafický-export-do-pdf)
   - [Krok 6: Odeslání Výsledků E-mailem](#krok-6-odeslání-výsledků-e-mailem)
   - [Krok 7: Statistiky & Srovnání](#krok-7-statistiky--srovnání)
4. [Bezpečnost & Konfigurace Klíčů](#-bezpečnost--konfigurace-klíčů)
5. [Struktura Kódu a Architektura](#-struktura-kódu-a-architektura)

---

## 🎯 Účel a Cíl Aplikace

Aplikace **Vstupní Triáž AI Gramotnosti** slouží jako interaktivní rozcestník na portálu **www.aivezdravotnictvi.cz**. Jejím cílem je:
* Změřit aktuální úroveň práce s umělou inteligencí u zdravotníků, nezdravotnického personálu i pacientů.
* Poskytnout okamžitý personalizovaný **Studijní plán vzdělávání** doporučených kurzů a workshopů IPVZ.
* Využít **Google Gemini API** k dynamickému rozboru 2 nejslabších oblastí uživatele.
* Uložit anonymizovaná data do produkční databáze MS SQL pro potřeby analytického srovnání Ministerstva zdravotnictví ČR.

---

## 📊 Metodika Aibility & Hodnocení

Hodnocení vychází z české metodiky **Aibility „Superpowered Professional™“** a měří 6 klíčových návyků:
1. **Včasné zapojení AI**: Schopnost identifikovat úkoly vhodné pro AI.
2. **Experimentování**: Zkoušení různých AI nástrojů a postupů.
3. **Iterace**: Schopnost upřesňovat dotazy (prompty) a komunikovat s AI.
4. **Ověřování výstupů**: Kontrola faktické správnosti, halucinací a bezpečnosti dat.
5. **Uložené postupy**: Tvorba opakovaně použitelných šablon a systémových promptů.
6. **Sdílení workflow**: Sdílení dobré praxe a AI postupů s kolegy na pracovišti.

### Výsledné Úrovně (Levels) & SP Score (0–100 %):
* **`0 % - 39 %` → Level 1 (Základní)**: Návrh volně přístupných e-learningových kurzů bez nutnosti registrace.
* **`40 % - 74 %` → Level 2 (Pokročilý)**: Doporučení praktických webinářů, workshopů IPVZ a formátu „AI čtvrthodinka“.
* **`75 % - 100 %` → Level 3 (Specialista)**: Pozvánka do specializovaných školení, pilotních projektů a komunity lektorů.
* **👑 Masterclass pro Management**: Speciální nabídka pro nezdravotnické manažery nemocnic zaměřená na AI Act, legislativu a strategii.

---

## 💻 Průvodce Používáním Aplikace

### Krok 1: Výběr Role ve Zdravotnictví
Na úvodní stránce si uživatel zvolí svou roli ve zdravotnickém ekosystému:
- *Pacient / Pečující*
- *Zdravotnický pracovník (Lékař, Sestra, Farmaceut)*
- *IT odborník / Dodavatel zdravotnických technologií*
- *Tvůrce politik / Výzkumník / Média*
- *Nezdravotnický personál nemocnice* (u této role se zobrazí volba, zda je uživatel členem vedení/managementu).

### Krok 2: Vyplnění Benchmark Testu
Test obsahuje **12 otázek** přizpůsobených zdravotnickému prostředí. Na každou otázku odpovídá uživatel na škále **1 (Vůbec nesouhlasím)** až **5 (Plně souhlasím)**:
- Intuitivní tlačítková škála s okamžitým vizuálním zvýrazněním vybrané hodnoty.
- Otázky pokrývají reálné scénáře (sumarizace propouštěcích zpráv, etické overování, anonymizace dat pacientů, tvorba promptů).

### Krok 3: Vyhodnocení & Výsledky
Po odeslání testu aplikace okamžitě zobrazí:
- Celkové **SP Skóre (0–100 %)** ve výrazném grafickém ukazateli.
- Přidělený **Level (1, 2 nebo 3)** s podrobným popisem.
- Detailní grafický rozpad skóre za všech **6 návyků Aibility**.
- **AI Analýzu z Google Gemini API**: Dynamicky vygenerovaný text zaměřený na 2 nejslabší oblasti s konkrétními tipy pro zlepšení.
- **Kroky studijního plánu**: 2–3 doporučené kurzy a akce.

### Krok 4: Výběr ze 7 Barevných Témat
V horní navigační liště je k dispozici tlačítko **🎨 Téma**, které umožňuje okamžitě přepínat kompletní vzhled webu i pozadí:
- 🌙 **Tmavá (Dark)** — Výchozí elegantní tmavě modrý styl s cyan akcenty.
- ☀️ **Světlá (Light)** — Čistý klinický světle šedý styl s vysokým kontrastem tmavého textu.
- ❤️ **Červená (Red)** — Kardiologický styl v tmavě červených a rubínových tónech.
- 💙 **Modrá (Blue)** — Královská modř s safírovými neurovými sítěmi.
- 💚 **Zelená (Green)** — Bio-health styl v tmavě smaragdových a mátových tónech.
- 💛 **Žlutá (Yellow)** — Teplý jantarovo-zlatý styl.
- 🌈 **Duhová (Rainbow)** — Spektrální holografický tmavý styl.

*Volba tématu se automaticky ukládá do prohlížeče (`localStorage`).*

### Krok 5: Grafický Export do PDF
Klepnutím na tlačítko **`🎨 Grafické PDF s výběrem témat`** na stránce výsledků se otevře modal okno:
- **100% podpora české diakritiky (UTF-8)** — Háčky a čárky v celém dokumentu.
- **Živý A4 Náhled**: V reálném čase vidíte přesnou podobu dokumentu A4.
- **Výběr barevného pozadí**: Uživatel si může před stažením vybrat libovolné ze 7 témat pozadí.
- **1-Click Stažení**: Vytvoří oficiální PDF dokument v profesionálním designu protokolu aivezdravotnictvi.cz.

### Krok 6: Odeslání Výsledků E-mailem
Ve spodní části výsledků stačí zadat e-mailovou adresu a kliknout na **Odeslat e-mail s PDF**:
- Uživatel obdrží přehledný HTML e-mail se svým studijním plánem.
- Součástí e-mailu je přiložený soubor **`Vysledky_Triaze_AI_Gramotnosti.pdf`**.

### Krok 7: Statistiky & Srovnání
V menu **Statistiky a Srovnání** aplikace zobrazí anonymizované souhrnné údaje počítané přímo na SQL serveru:
- Celkový počet dokončených triáží a průměrné skóre.
- Tabulka průměrného skóre a počtu uživatelů podle jednotlivých rolí.
- Grafická distribuce uživatelů do úrovní Level 1, 2 a 3.
- Průměrné plnění u všech 6 návyků Aibility.
- Tlačítko **`+ Vygenerovat 50 testovacích dat`** pro rychlé otestování analytických grafů.

---

## 🔒 Bezpečnost & Konfigurace Klíčů

Pro zajištění bezpečnosti při publikaci na GitHub jsou všechny přístupové údaje a klíče odděleny:

### 1. Pravidla v `.gitignore`
Citlivé konfigurace a soubory s produkčními hesly jsou ignorovány a **nikdy se neodesílají do veřejného repozitáře**:
- `appsettings.Production.json`
- `appsettings.Development.json`
- `appsettings.secrets.json`
- `.env*`
- `*.pem`, `*.pfx`, `*.key`

### 2. Šablona `appsettings.json` pro repozitář
V git repozitáři je obsažen soubor `appsettings.json` se zástupnými hodnotami:
```json
{
  "ConnectionStrings": {
    "ProductionConnection": "Server=sql8.aspone.cz;Database=db4937;User Id=db4937;Password=YOUR_SQL_PASSWORD;Encrypt=False"
  },
  "AI": {
    "GeminiApiKey": "YOUR_GEMINI_API_KEY"
  },
  "Smtp": {
    "Host": "smtp.forpsi.com",
    "Port": 587,
    "Username": "scio@ekobio.org",
    "Password": "YOUR_SMTP_PASSWORD"
  }
}
```

---

## 🏗 Struktura Kódu a Architektura

Aplikace je postavena v **ASP.NET Core MVC (.NET 8)** s klasickým čitelným C# kódem bez složitých ORM magií:

```
IPCZ/
├── Controllers/
│   └── TriageController.cs      # Směrování, řízení testu, PDF export a e-mail
├── Models/
│   └── TriageViewModels.cs      # DTO modely pro role, otázky, výsledky a statistiky
├── Services/
│   ├── DatabaseService.cs       # ADO.NET (SqlConnection/SqlCommand) a SQL procedury
│   ├── TriageService.cs         # Výpočetní logika SP Score, úrovní a doporučení
│   ├── GeminiService.cs         # REST klient pro Google Gemini 1.5 Flash API
│   ├── EmailService.cs          # Služba odesílání e-mailů s PDF přílohou přes MailKit
│   └── PdfService.cs            # Generování PDF protokolu
├── Views/
│   ├── Triage/
│   │   ├── Index.cshtml         # Obrazovka 1: Výběr role
│   │   ├── Test.cshtml          # Obrazovka 2: Benchmark test (12 otázek)
│   │   ├── Results.cshtml       # Obrazovka 3: Výsledky + PDF Modal + E-mail
│   │   └── Stats.cshtml         # Obrazovka 4: Anonymní statistiky
│   └── Shared/
│       └── _Layout.cshtml       # Hlavní šablona s 7-tématovým přepínačem
├── SQL/
│   ├── schema.sql               # Vytvoření tabulky Ipcz_TriageResults
│   └── procedures.sql           # Procedury Ipcz_SaveTriageResult a Ipcz_GetTriageStatistics
├── wwwroot/
│   ├── assets/                  # 7 vygenerovaných pozadí pro jednotlivá témata (bg_dark.jpg - bg_rainbow.jpg)
│   └── css/site.css             # Custom Medical-Tech design systém a proměnné témat
├── .gitignore                   # Ignorování sestavení a citlivých klíčů
└── README.md                    # Vývojářská dokumentace a AI report
```
