using System;
using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Komponentowo.Models;

namespace Komponentowo.Components
{
    /// <summary>
    /// Komponent do obsługi pojedynczego zadania: ładowanie szczegółów, 
    /// aktualizacja (zapisywanie zmian) oraz obsługa szkicu zadania.
    /// </summary>
    public class TaskDetailComponent
    {
        private TaskListComponent _taskList;
        private string _draftFilePath = "";
        private bool _hasDraft = false;
        private string _lastError = string.Empty;
        private DateTime _lastEdited = DateTime.MinValue;

        public TaskDetailComponent()
        {
            _taskList = new TaskListComponent();
            _draftFilePath = "draft_task.json";
            _hasDraft = false;
            _lastError = string.Empty;
            _lastEdited = DateTime.MinValue;
        }

        public TaskDetailComponent(TaskListComponent taskList, IWebHostEnvironment env)
        {
            _taskList = taskList;
            _draftFilePath = Path.Combine(env.ContentRootPath, "draft_task.json");
        }

        /// <summary> Pobiera zadanie o podanym ID (szczegóły zadania). </summary>
        public TaskItem? GetTaskById(int taskId)
        {
            var tasks = _taskList.GetAllTasks();
            return tasks.Find(t => t.Id == taskId);
        }

        /// <summary> Zapisuje zmiany istniejącego zadania. </summary>
        public bool SaveTaskChanges(TaskItem task)
        {
            // Wykorzystujemy TaskListComponent do aktualizacji zadania
            return _taskList.UpdateTask(task);
        }

        /// <summary> Tworzy nowe zadanie i dodaje je do listy zadań. </summary>
        public void CreateNewTask(TaskItem task)
        {
            // Dodaje zadanie do listy (TaskListComponent sam nada ID i zapisze plik)
            _taskList.AddTask(task);
            // Po dodaniu nowego zadania można wyczyścić szkic
            ClearDraft();
        }

        /// <summary> Zapisuje bieżący edytowany task jako szkic (np. w trakcie tworzenia nowego zadania). </summary>
        public void SaveDraft(TaskItem draft)
        {
            if (draft == null) return;
            var json = JsonSerializer.Serialize(draft, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_draftFilePath, json);
        }

        /// <summary> Ładuje zapisany szkic zadania (jeśli istnieje). </summary>
        public TaskItem? LoadDraft()
        {
            if (!File.Exists(_draftFilePath)) return null;
            try
            {
                var json = File.ReadAllText(_draftFilePath);
                return JsonSerializer.Deserialize<TaskItem>(json);
            }
            catch (JsonException)
            {
                return null;
            }
        }

        /// <summary> Usuwa plik szkicu (np. po zakończeniu edycji/zapisie nowego zadania). </summary>
        public void ClearDraft()
        {
            if (File.Exists(_draftFilePath))
            {
                File.Delete(_draftFilePath);
            }
        }

        /// <summary> Przełącza status ukończenia zadania o podanym ID. </summary>
        public bool ToggleTaskCompletion(int taskId)
        {
            var task = GetTaskById(taskId);
            if (task == null) return false;
            task.IsCompleted = !task.IsCompleted;
            return SaveTaskChanges(task);
        }

        // Gettery i settery dla prywatnych pól:
        public TaskListComponent GetTaskList() => _taskList;
        public void SetTaskList(TaskListComponent taskList) => _taskList = taskList;
        public string GetDraftFilePath() => _draftFilePath;
        public void SetDraftFilePath(string path) => _draftFilePath = path;
        public bool GetHasDraft() => _hasDraft;
        public void SetHasDraft(bool hasDraft) => _hasDraft = hasDraft;
        public string GetLastError() => _lastError;
        public void SetLastError(string error) => _lastError = error;
        public DateTime GetLastEdited() => _lastEdited;
        public void SetLastEdited(DateTime dateTime) => _lastEdited = dateTime;
    }
}
