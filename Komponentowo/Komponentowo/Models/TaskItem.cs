using System.Diagnostics.Eventing.Reader;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Komponentowo.Models
{
    public class TaskItem
    {
        private static readonly Regex ValidTitleRegex = new(@"^[a-zA-Z0-9ąćęłńóśźżĄĆĘŁŃÓŚŹŻ\s]+$", RegexOptions.Compiled);
        private int _id;
        private string _title = string.Empty;
        private bool _isCompleted = false;
        private int _priority = 1;
        private string _category = string.Empty;
        private DateTime _deadline = DateTime.MinValue;
        public static List<TaskItem>? AllTasksSource { get; set; }

        public int Id { get; set; }

        public string Title
        {
            get => _title;
            set
            {
                while (true)
                {
                    if (!IsValidTitle(value))
                    {
                        throw new ArgumentException("Tytuł nie może zawierać znaków specjalnych oraz emotek.");
                    }
                    else
                    {
                        _title = value;
                        break;
                    }
                }
            }
        }

        public bool IsCompleted
        {
            get => _isCompleted;
            set => _isCompleted = value; // pozwalamy zmieniać
        }

        public int Priority
        {
            get => _priority;
            set
            {
                if (value < 1) _priority = 1;
                else if (value > 5) _priority = 5;
                else _priority = value;
            }
        }

        public string Category
        {
            get => _category;
            set => _category = value;
        }

        public DateTime Deadline
        {
            get => _deadline;
            set => _deadline = value;
        }

        private bool IsValidTitle(string title)
        {
            return ValidTitleRegex.IsMatch(title);
        }
    }
}

