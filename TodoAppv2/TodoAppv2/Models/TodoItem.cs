using System;
using System.ComponentModel.DataAnnotations;

namespace TodoAppv2.Models
{
    public class TodoItem
    {
        public int Id { get; set; }  // Klucz główny (automatycznie generowany)

        [Required(ErrorMessage = "Nazwa zadania jest wymagana.")]
        [StringLength(100, ErrorMessage = "Nazwa jest za długa (max 100 znaków).")]
        public string Name { get; set; }

        [Display(Name = "Termin")]  // Używane do wyświetlania etykiety pola w formularzu
        public DateTime? Deadline { get; set; }  // Nullable - termin wykonania (opcjonalny)

        [Range(1, 5, ErrorMessage = "Priorytet musi być w zakresie 1–5.")]
        public int Priority { get; set; }

        public bool IsDone { get; set; }  // Status zadania: wykonane czy nie
    }
}
