using Komponentowo.Components;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Komponentowo.Pages
{
    public class StatsModel : PageModel
    {
        private readonly StatsComponent _stats;

        public int TotalTasks { get; private set; }
        public int CompletedTasks { get; private set; }
        public double CompletionPercent { get; private set; }
        public string StatsBackgroundColor { get; private set; }

        public StatsModel(StatsComponent stats)
        {
            _stats = stats;
            StatsBackgroundColor = "#ffffff"; // zapobiega b³êdowi null podczas konstrukcji
        }

        public void OnGet()
        {
            TotalTasks = _stats.GetTotalTasks();
            CompletedTasks = _stats.GetCompletedTasks();
            CompletionPercent = _stats.GetCompletionPercentage();
            StatsBackgroundColor = _stats.BackgroundColor;
        }
    }
}
