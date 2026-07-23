using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IpczApp.Models
{
    public class RoleSelectionViewModel
    {
        [Required(ErrorMessage = "Prosím vyberte vaši roli.")]
        public string SelectedRole { get; set; } = string.Empty;

        public bool IsManagement { get; set; } = false;

        public static readonly List<string> AvailableRoles = new List<string>
        {
            "Pacient",
            "Zdravotník",
            "IT odborník či dodavatel",
            "Tvůrce politik a média",
            "Nezdravotnický personál nemocnice"
        };
    }

    public class QuestionModel
    {
        public int Id { get; set; }
        public int HabitId { get; set; } // 1 až 6
        public string HabitName { get; set; } = string.Empty;
        public string QuestionText { get; set; } = string.Empty;
    }

    public class TestViewModel
    {
        public string Role { get; set; } = string.Empty;
        public bool IsManagement { get; set; }

        // Odpovědi na 12 otázek (hodnoty 1 až 5)
        public Dictionary<int, int> Answers { get; set; } = new Dictionary<int, int>();

        public List<QuestionModel> Questions { get; set; } = new List<QuestionModel>();
    }

    public class HabitScoreModel
    {
        public int HabitId { get; set; }
        public string HabitName { get; set; } = string.Empty;
        public int RawScore { get; set; } // 2-10
        public double PercentageScore { get; set; } // 0-100%
        public string Description { get; set; } = string.Empty;
    }

    public class ResultViewModel
    {
        public int ResultId { get; set; }
        public string Role { get; set; } = string.Empty;
        public bool IsManagement { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<HabitScoreModel> HabitScores { get; set; } = new List<HabitScoreModel>();
        public double OverallScore { get; set; } // 0-100%
        public string Level { get; set; } = string.Empty; // Level 1 (Základní), Level 2 (Pokročilý), Level 3 (Specialista)
        public string LevelDescription { get; set; } = string.Empty;

        public List<HabitScoreModel> WeakestHabits { get; set; } = new List<HabitScoreModel>();
        public List<string> RecommendedSteps { get; set; } = new List<string>();

        public bool ShowMasterclass { get; set; }

        // Dynamická AI analýza slabých stránek od Gemini API
        public string AiAnalysis { get; set; } = string.Empty;

        // E-mail formulář
        [EmailAddress(ErrorMessage = "Zadejte platnou e-mailovou adresu.")]
        public string? EmailAddress { get; set; }
        public string? EmailStatusMessage { get; set; }
        public bool EmailSentSuccess { get; set; }
    }

    public class RoleStatModel
    {
        public string Role { get; set; } = string.Empty;
        public int Count { get; set; }
        public double AvgScore { get; set; }
    }

    public class LevelStatModel
    {
        public string Level { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class StatsViewModel
    {
        public int TotalCount { get; set; }
        public double AverageOverallScore { get; set; }

        public List<RoleStatModel> RoleStats { get; set; } = new List<RoleStatModel>();
        public List<LevelStatModel> LevelStats { get; set; } = new List<LevelStatModel>();

        public Dictionary<string, double> HabitAverages { get; set; } = new Dictionary<string, double>();

        public string? SystemMessage { get; set; }
    }
}
