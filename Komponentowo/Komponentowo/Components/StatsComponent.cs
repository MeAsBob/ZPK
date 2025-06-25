using System;
using System.IO;
using System.Text.Json;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Komponentowo.Models;

namespace Komponentowo.Components
{
    /// <summary>
    /// Komponent do obliczania statystyk zadań (liczba wszystkich, ukończonych, % wykonania)
    /// oraz zarządzania ustawieniami (np. kolorem tła statystyk) zapisywanymi do JSON.
    /// </summary>
    public class StatsComponent
    {
        private TaskListComponent _taskList;
        private string _configFilePath = "";
        private int _taskLimit = 100;
        private string _componentName = "StatsComponent";
        private bool _autoSaveEnabled = true;

        // Ustawienie domyślne (np. biały kolor tła statystyk)
        public string BackgroundColor { get; private set; } = "#FFFFFF";

        public StatsComponent()
        {
            _taskList = new TaskListComponent();
            _configFilePath = "stats_config.json";
            _taskLimit = 100;
            _componentName = "StatsComponent";
            _autoSaveEnabled = true;
        }

        public StatsComponent(TaskListComponent taskList, IWebHostEnvironment env)
        {
            _taskList = taskList;
            _configFilePath = Path.Combine(env.ContentRootPath, "stats_config.json");

            // Wczytaj ustawienia z pliku, jeśli istnieje
            if (File.Exists(_configFilePath))
            {
                string json = File.ReadAllText(_configFilePath);
                try
                {
                    var config = JsonSerializer.Deserialize<StatsSettings>(json);
                    if (config != null && !string.IsNullOrEmpty(config.BackgroundColor))
                    {
                        BackgroundColor = config.BackgroundColor;
                    }
                }
                catch (JsonException)
                {
                    // Jeśli JSON jest nieprawidłowy, można pominąć i zachować domyślne ustawienia
                }
            }
        }

        /// <summary> Zwraca liczbę wszystkich zadań. </summary>
        public int GetTotalTasks() => _taskList.GetAllTasks().Count;

        /// <summary> Zwraca liczbę ukończonych zadań. </summary>
        public int GetCompletedTasks() => _taskList.GetAllTasks().Count(t => t.IsCompleted);

        /// <summary> Zwraca procentowy poziom ukończenia zadań (0-100). </summary>
        public double GetCompletionPercentage()
        {
            int total = GetTotalTasks();
            int completed = GetCompletedTasks();
            if (total == 0) return 0;
            return Math.Round((double)completed / total * 100, 2);
        }

        /// <summary> Ustawia nowy kolor tła statystyk i zapisuje ustawienie do pliku. </summary>
        public void SetBackgroundColor(string hexColor)
        {
            if (string.IsNullOrWhiteSpace(hexColor)) return;
            BackgroundColor = hexColor;
            var config = new StatsSettings { BackgroundColor = hexColor };
            // Utworzenie pliku konfiguracyjnego (jeśli brak) i zapis ustawień
            Directory.CreateDirectory(Path.GetDirectoryName(_configFilePath) ?? string.Empty);
            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_configFilePath, json);
        }

        // Gettery i settery dla prywatnych pól:
        public TaskListComponent GetTaskList() => _taskList;
        public void SetTaskList(TaskListComponent taskList) => _taskList = taskList;
        public string GetConfigFilePath() => _configFilePath;
        public void SetConfigFilePath(string path) => _configFilePath = path;
        public int GetTaskLimit() => _taskLimit;
        public void SetTaskLimit(int limit) => _taskLimit = limit;
        public string GetComponentName() => _componentName;
        public void SetComponentName(string name) => _componentName = name;
        public bool GetAutoSaveEnabled() => _autoSaveEnabled;
        public void SetAutoSaveEnabled(bool enabled) => _autoSaveEnabled = enabled;

        /// <summary>
        /// Pomocnicza klasa ustawień statystyk do serializacji (np. kolor tła).
        /// </summary>
        private class StatsSettings
        {
            public string BackgroundColor { get; set; } = "#FFFFFF";
        }
    }
}
