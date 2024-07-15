namespace ToDoList.Models
{
    public class ToDo
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        private DateTime? DueDate { get; set; }
        public string? CategoryId { get; set; }
        //public string CategoryId { get; set; } = string.Empty;
        public Category? Category { get; set; }
        public string? StatusId { get; set; }
        public Status? Status { get; set; }
        public bool Overdue => StatusId == "open" && DueDate < DateTime.Today;
    }
}

