-- ============================================================================
-- Uložené procedury pro aplikaci IPCZ
-- ============================================================================

-- 1. Procedura pro zápis nového výsledku triáže
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'Ipcz_SaveTriageResult')
    DROP PROCEDURE Ipcz_SaveTriageResult;
GO

CREATE PROCEDURE Ipcz_SaveTriageResult
    @Role NVARCHAR(100),
    @IsManagement BIT,
    @Habit1Score INT,
    @Habit2Score INT,
    @Habit3Score INT,
    @Habit4Score INT,
    @Habit5Score INT,
    @Habit6Score INT,
    @OverallScore DECIMAL(5,2),
    @Level NVARCHAR(50),
    @NewId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Ipcz_TriageResults (
        Role, IsManagement,
        Habit1Score, Habit2Score, Habit3Score, Habit4Score, Habit5Score, Habit6Score,
        OverallScore, Level, CreatedAt
    )
    VALUES (
        @Role, @IsManagement,
        @Habit1Score, @Habit2Score, @Habit3Score, @Habit4Score, @Habit5Score, @Habit6Score,
        @OverallScore, @Level, GETUTCDATE()
    );

    SET @NewId = SCOPE_IDENTITY();
END
GO

-- 2. Procedura pro výpočet a načtení statistik
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'Ipcz_GetTriageStatistics')
    DROP PROCEDURE Ipcz_GetTriageStatistics;
GO

CREATE PROCEDURE Ipcz_GetTriageStatistics
AS
BEGIN
    SET NOCOUNT ON;

    -- Výsledková sada 1: Celkové počty
    SELECT 
        COUNT(*) AS TotalCount,
        ISNULL(AVG(OverallScore), 0) AS AverageOverallScore
    FROM Ipcz_TriageResults;

    -- Výsledková sada 2: Průměrné celkové skóre dle role
    SELECT 
        Role,
        COUNT(*) AS RoleCount,
        ISNULL(AVG(OverallScore), 0) AS AvgScore
    FROM Ipcz_TriageResults
    GROUP BY Role;

    -- Výsledková sada 3: Počty uživatelů dle úrovní (Level 1, Level 2, Level 3)
    SELECT 
        Level,
        COUNT(*) AS LevelCount
    FROM Ipcz_TriageResults
    GROUP BY Level;

    -- Výsledková sada 4: Průměrné skóre dle jednotlivých návyků (přepočteno na 0-100)
    -- Poznámka: Skóre v DB je součet 2 otázek (2 až 10), přepočet na % = (Suma - 2) * 12.5
    SELECT 
        AVG((Habit1Score - 2) * 12.5) AS AvgHabit1,
        AVG((Habit2Score - 2) * 12.5) AS AvgHabit2,
        AVG((Habit3Score - 2) * 12.5) AS AvgHabit3,
        AVG((Habit4Score - 2) * 12.5) AS AvgHabit4,
        AVG((Habit5Score - 2) * 12.5) AS AvgHabit5,
        AVG((Habit6Score - 2) * 12.5) AS AvgHabit6
    FROM Ipcz_TriageResults;
END
GO
