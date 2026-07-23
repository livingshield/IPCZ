using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using IpczApp.Models;

namespace IpczApp.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(IConfiguration configuration)
        {
            // Prefer ProductionConnection if present, fallback to DefaultConnection
            var prod = configuration.GetConnectionString("ProductionConnection");
            var def = configuration.GetConnectionString("DefaultConnection");
            _connectionString = !string.IsNullOrEmpty(prod) ? prod : (def ?? string.Empty);
        }

        /// <summary>
        /// Uloží výsledky triáže do databáze pomocí uložené procedury Ipcz_SaveTriageResult
        /// </summary>
        public int SaveTriageResult(ResultViewModel model)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "Ipcz_SaveTriageResult";
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Role", model.Role);
                    command.Parameters.AddWithValue("@IsManagement", model.IsManagement);

                    // Skóre za jednotlivé návyky (raw skóre 2 až 10)
                    command.Parameters.AddWithValue("@Habit1Score", GetHabitRawScore(model, 1));
                    command.Parameters.AddWithValue("@Habit2Score", GetHabitRawScore(model, 2));
                    command.Parameters.AddWithValue("@Habit3Score", GetHabitRawScore(model, 3));
                    command.Parameters.AddWithValue("@Habit4Score", GetHabitRawScore(model, 4));
                    command.Parameters.AddWithValue("@Habit5Score", GetHabitRawScore(model, 5));
                    command.Parameters.AddWithValue("@Habit6Score", GetHabitRawScore(model, 6));

                    command.Parameters.AddWithValue("@OverallScore", (decimal)model.OverallScore);
                    command.Parameters.AddWithValue("@Level", model.Level);

                    var outputIdParam = new SqlParameter("@NewId", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(outputIdParam);

                    command.ExecuteNonQuery();

                    return (int)outputIdParam.Value;
                }
            }
        }

        /// <summary>
        /// Načte agregované statistiky z databáze pomocí uložené procedury Ipcz_GetTriageStatistics
        /// </summary>
        public StatsViewModel GetStatistics()
        {
            var stats = new StatsViewModel();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "Ipcz_GetTriageStatistics";
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = command.ExecuteReader())
                    {
                        // Sada 1: Celkový počet a průměrné skóre
                        if (reader.Read())
                        {
                            stats.TotalCount = reader.IsDBNull(0) ? 0 : Convert.ToInt32(reader.GetValue(0));
                            stats.AverageOverallScore = reader.IsDBNull(1) ? 0 : Convert.ToDouble(reader.GetValue(1));
                        }

                        // Sada 2: Statistiky dle rolí
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                stats.RoleStats.Add(new RoleStatModel
                                {
                                    Role = reader.GetString(0),
                                    Count = reader.IsDBNull(1) ? 0 : Convert.ToInt32(reader.GetValue(1)),
                                    AvgScore = reader.IsDBNull(2) ? 0 : Convert.ToDouble(reader.GetValue(2))
                                });
                            }
                        }

                        // Sada 3: Počty dle úrovní (Level)
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                stats.LevelStats.Add(new LevelStatModel
                                {
                                    Level = reader.GetString(0),
                                    Count = reader.IsDBNull(1) ? 0 : Convert.ToInt32(reader.GetValue(1))
                                });
                            }
                        }

                        // Sada 4: Průměry jednotlivých 6 návyků (přepočítané na %)
                        if (reader.NextResult() && reader.Read())
                        {
                            var habitNames = new[]
                            {
                                "Včasné zapojení AI",
                                "Experimentování",
                                "Iterace",
                                "Ověřování výstupů",
                                "Uložené postupy",
                                "Sdílení workflow"
                            };

                            for (int i = 0; i < 6; i++)
                            {
                                double val = reader.IsDBNull(i) ? 0 : Convert.ToDouble(reader.GetValue(i));
                                stats.HabitAverages[habitNames[i]] = val;
                            }
                        }
                    }
                }
            }

            return stats;
        }

        /// <summary>
        /// Vygeneruje testovací anonymizovaná data v databázi, pokud je databáze prázdná nebo na vyžádání.
        /// </summary>
        public void SeedDummyData(int count = 50)
        {
            var rnd = new Random();
            var roles = RoleSelectionViewModel.AvailableRoles;

            for (int i = 0; i < count; i++)
            {
                string role = roles[rnd.Next(roles.Count)];
                bool isMgmt = role == "Nezdravotnický personál nemocnice" && rnd.Next(2) == 1;

                int h1 = rnd.Next(2, 11);
                int h2 = rnd.Next(2, 11);
                int h3 = rnd.Next(2, 11);
                int h4 = rnd.Next(2, 11);
                int h5 = rnd.Next(2, 11);
                int h6 = rnd.Next(2, 11);

                int totalRaw = h1 + h2 + h3 + h4 + h5 + h6;
                double overall = Math.Round((totalRaw - 12) / 48.0 * 100.0, 1);

                string level;
                if (overall < 40) level = "Level 1 (Základní)";
                else if (overall < 75) level = "Level 2 (Pokročilý)";
                else level = "Level 3 (Specialista)";

                var dummyResult = new ResultViewModel
                {
                    Role = role,
                    IsManagement = isMgmt,
                    OverallScore = overall,
                    Level = level,
                    HabitScores = new List<HabitScoreModel>
                    {
                        new HabitScoreModel { HabitId = 1, RawScore = h1 },
                        new HabitScoreModel { HabitId = 2, RawScore = h2 },
                        new HabitScoreModel { HabitId = 3, RawScore = h3 },
                        new HabitScoreModel { HabitId = 4, RawScore = h4 },
                        new HabitScoreModel { HabitId = 5, RawScore = h5 },
                        new HabitScoreModel { HabitId = 6, RawScore = h6 }
                    }
                };

                SaveTriageResult(dummyResult);
            }
        }

        private int GetHabitRawScore(ResultViewModel model, int habitId)
        {
            var habit = model.HabitScores.Find(h => h.HabitId == habitId);
            return habit?.RawScore ?? 2;
        }
    }
}
