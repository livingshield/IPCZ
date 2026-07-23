# CLAUDE.md / Agentic Development Rules for IPCZ

## Architectural Principles
1. **Classic C# Stack**: Use ASP.NET Core MVC (.NET 8) with explicit C# syntax. Avoid Blazor to maintain maximum transparency and ease of explanation.
2. **Pure ADO.NET**: Access MS SQL Server using `Microsoft.Data.SqlClient` (`SqlConnection`, `SqlCommand`) and stored procedures (`Ipcz_SaveTriageResult`, `Ipcz_GetTriageStatistics`) without heavy ORM layers like EF Core.
3. **Prefix Isolation**: All SQL database objects must use the prefix `Ipcz_` to prevent collisions on the shared SQL server.
4. **Resilient AI Service Integration**: All calls to Google Gemini API must be wrapped with fallback error handling so that an API failure never crashes the triage user flow.

## Design Principles
1. **State-of-the-Art Medical-Tech UI**: Use Vanilla CSS, Glassmorphism, Google Fonts (`Outfit` + `Inter`), score gauges, progress bars, and custom color palettes.
2. **7 Theme System**: Support 7 dynamic color themes (Dark, Light, Red, Blue, Green, Yellow, Rainbow) with side-panel graphic assets in `wwwroot/assets/` keeping text areas dark/clean for readability.
3. **High-Contrast Light Theme**: Ensure all text elements, headers, and PDF preview cards use dark slate tones when Light Theme is active.

## Code Standards
- Czech language UI with 100% proper diacritics.
- Never commit plain-text credentials or API keys to the repository (use placeholders in `appsettings.json` and keep secrets in ignored files).
