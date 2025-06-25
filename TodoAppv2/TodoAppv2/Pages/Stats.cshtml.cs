using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TodoAppv2.Models;
using System.Threading.Tasks;

public class StatsModel : PageModel
{
    private readonly TodoContext _context;
    public StatsModel(TodoContext context)
    {
        _context = context;
    }

    public int DoneCount { get; set; }    // liczba zadañ ukoñczonych
    public int TodoCount { get; set; }    // liczba zadañ do zrobienia

    public async Task OnGetAsync()
    {
        DoneCount = await _context.TodoItems.CountAsync(t => t.IsDone == true);
        TodoCount = await _context.TodoItems.CountAsync(t => t.IsDone == false);
    }
}
