using Komponentowo.Components;
using Komponentowo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Komponentowo.Pages
{
    public class IndexModel : PageModel
    {
        private readonly TaskListComponent _taskList;
        private readonly TaskDetailComponent _detail;
        private readonly StatsComponent _stats;

        public List<TaskItem> Tasks { get; set; } = new();
        public TaskItem? DraftTask { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public double CompletionPercent { get; set; }
        public string StatsBackgroundColor { get; set; } = "#ffffff";

        public IndexModel(TaskListComponent taskList, StatsComponent stats, TaskDetailComponent detail)
        {
            _taskList = taskList;
            _stats = stats;
            _detail = detail;
        }

        public void OnGet()
        {
            Tasks = _taskList.GetAllTasks();
            DraftTask = _detail.LoadDraft() ?? new TaskItem(); // 👈 ważne
            TotalTasks = _stats.GetTotalTasks();
            CompletedTasks = _stats.GetCompletedTasks();
            CompletionPercent = _stats.GetCompletionPercentage();
            StatsBackgroundColor = _stats.BackgroundColor;
        }

        public IActionResult OnPostAdd(string newTaskTitle, int newTaskPriority, string newTaskCategory, DateTime? newTaskDeadline)
        {
            if (!string.IsNullOrWhiteSpace(newTaskTitle))
            {
                var task = new TaskItem { Title = newTaskTitle, Priority = newTaskPriority, Category = newTaskCategory, Deadline = newTaskDeadline ?? DateTime.MinValue };
                _detail.CreateNewTask(task);
            }
            return RedirectToPage();
        }

        public IActionResult OnPostSaveDraft([FromBody] TaskItem draft)
        {
            _detail.SaveDraft(draft);
            return new JsonResult("OK");
        }

        public IActionResult OnPostToggle(int taskId)
        {
            _detail.ToggleTaskCompletion(taskId);
            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int taskId)
        {
            _taskList.RemoveTask(taskId);
            return RedirectToPage();
        }
    }
}
