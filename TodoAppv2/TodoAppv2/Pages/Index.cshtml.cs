using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TodoAppv2.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace TodoAppv2.Pages
{
    public class IndexModel : PageModel
    {
        private readonly TodoContext _context;

        public IndexModel(TodoContext context)
        {
            _context = context;
        }

        // Filtrowanie i stronicowanie 

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? StatusFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;

        public int TotalPages { get; set; }
        public List<TodoItem> Tasks { get; set; }

        // Formularz dodawania zadania 

        [BindProperty]
        public TodoItem NewTask { get; set; }


        // GET: wyœwietlanie listy zadañ

        public async Task OnGetAsync()
        {
            IQueryable<TodoItem> query = _context.TodoItems;

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(t => t.Name.Contains(SearchTerm));
            }

            if (!string.IsNullOrWhiteSpace(StatusFilter) && StatusFilter != "all")
            {
                if (StatusFilter == "done")
                    query = query.Where(t => t.IsDone);
                else if (StatusFilter == "todo")
                    query = query.Where(t => !t.IsDone);
            }

            int pageSize = 10;
            int totalItems = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            if (PageIndex < 1) PageIndex = 1;
            if (TotalPages > 0 && PageIndex > TotalPages) PageIndex = TotalPages;

            Tasks = await query
                .OrderBy(t => t.Id)
                .Skip((PageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // Dodawanie nowego zadania

        public async Task<IActionResult> OnPostAddAsync()
        {
            Console.WriteLine(">>> OnPostAddAsync uruchomione");

            if (!ModelState.IsValid)
            {
                Console.WriteLine(">>> B³¹d walidacji – szczegó³y:");

                foreach (var entry in ModelState)
                {
                    if (entry.Value.Errors.Count > 0)
                    {
                        Console.WriteLine($"Pole: {entry.Key}");
                        foreach (var error in entry.Value.Errors)
                        {
                            Console.WriteLine($" - {error.ErrorMessage}");
                        }
                    }
                }

                await OnGetAsync();
                return Page();
            }

            Console.WriteLine($">>> Dodawanie zadania: {NewTask.Name}, Deadline: {NewTask.Deadline}, Priorytet: {NewTask.Priority}");

            NewTask.IsDone = false;
            _context.TodoItems.Add(NewTask);
            await _context.SaveChangesAsync();

            Console.WriteLine(">>> Zadanie zapisane do bazy danych");

            return RedirectToPage("/Index");
        }


        // Oznaczanie jako zrobione

        public async Task<IActionResult> OnPostCompleteAsync(int id)
        {
            Console.WriteLine($">>> Oznaczanie zadania ID={id} jako zrobione");

            var task = await _context.TodoItems.FindAsync(id);
            if (task != null)
            {
                task.IsDone = true;
                await _context.SaveChangesAsync();
                Console.WriteLine(">>> Zadanie oznaczone jako zrobione");
            }
            else
            {
                Console.WriteLine(">>> Nie znaleziono zadania o podanym ID");
            }

            return RedirectToPage("/Index", new { SearchTerm, StatusFilter, PageIndex });
        }

        // Usuwanie

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            Console.WriteLine($">>> Usuwanie zadania ID={id}");

            var task = await _context.TodoItems.FindAsync(id);
            if (task != null)
            {
                _context.TodoItems.Remove(task);
                await _context.SaveChangesAsync();
                Console.WriteLine(">>> Zadanie usuniête");
            }
            else
            {
                Console.WriteLine(">>> Nie znaleziono zadania do usuniêcia");
            }

            return RedirectToPage("/Index", new { SearchTerm, StatusFilter, PageIndex });
        }
    }
}
