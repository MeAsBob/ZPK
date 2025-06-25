using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Komponentowo.Models;

namespace Komponentowo.Components
{
    public class TaskListComponent
    {
        private string filePath = "data/tasks.json";
        private List<TaskItem> tasks = new();
        private int nextId = 1;
        private bool _isDirty = false;
        private string _listName = "Default List";

        public TaskListComponent()
        {
            _isDirty = false;
            _listName = "Default List";
            LoadFromFile();
        }

        public List<TaskItem> GetAllTasks() => tasks;

        public void AddTask(TaskItem task)
        {
            task.Id = nextId++;
            tasks.Add(task);
            SaveToFile();
        }

        public bool UpdateTask(TaskItem updatedTask)
        {
            var task = tasks.FirstOrDefault(t => t.Id == updatedTask.Id);
            if (task == null) return false;
            task.Title = updatedTask.Title;
            task.IsCompleted = updatedTask.IsCompleted;
            SaveToFile();
            return true;
        }

        public bool RemoveTask(int id)
        {
            var task = tasks.FirstOrDefault(t => t.Id == id);
            if (task == null) return false;
            tasks.Remove(task);
            SaveToFile();
            return true;
        }

        private void SaveToFile()
        {
            Directory.CreateDirectory("data");
            var json = JsonSerializer.Serialize(tasks);
            File.WriteAllText(filePath, json);
        }

        private void LoadFromFile()
        {
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                var loaded = JsonSerializer.Deserialize<List<TaskItem>>(json);
                if (loaded != null)
                {
                    tasks = loaded;
                    nextId = tasks.Any() ? tasks.Max(t => t.Id) + 1 : 1;
                }
            }
            TaskItem.AllTasksSource = tasks;
        }

        // Gettery i settery dla prywatnych pól:
        public string GetFilePath() => filePath;
        public void SetFilePath(string path) => filePath = path;
        public List<TaskItem> GetTasks() => tasks;
        public void SetTasks(List<TaskItem> taskList) => tasks = taskList;
        public int GetNextId() => nextId;
        public void SetNextId(int id) => nextId = id;
        public bool GetIsDirty() => _isDirty;
        public void SetIsDirty(bool isDirty) => _isDirty = isDirty;
        public string GetListName() => _listName;
        public void SetListName(string name) => _listName = name;
    }
}
