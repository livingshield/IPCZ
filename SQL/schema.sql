-- ============================================================================
-- Databázové schéma pro aplikaci IPCZ (Vstupní triáž aivezdravotnictvi.cz)
-- Všechny objekty mají prefix Ipcz_ pro kompletní izolaci na sdíleném serveru
-- ============================================================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Ipcz_TriageResults')
BEGIN
    CREATE TABLE Ipcz_TriageResults (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Role NVARCHAR(100) NOT NULL,
        IsManagement BIT NOT NULL DEFAULT 0,
        
        -- Skóre za 6 jednotlivých návyků (1-5 bodů za 2 otázky = 2-10 celkem za návyk)
        Habit1Score INT NOT NULL, -- Včasné zapojení AI
        Habit2Score INT NOT NULL, -- Experimentování
        Habit3Score INT NOT NULL, -- Iterace
        Habit4Score INT NOT NULL, -- Ověřování výstupů
        Habit5Score INT NOT NULL, -- Uložené postupy
        Habit6Score INT NOT NULL, -- Sdílení workflow
        
        -- Přepočítané celkové skóre (0 - 100)
        OverallScore DECIMAL(5,2) NOT NULL,
        
        -- Doporučený level (Level 1, Level 2, Level 3)
        Level NVARCHAR(50) NOT NULL,
        
        -- Čas vytvoření
        CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE()
    );
END
GO
