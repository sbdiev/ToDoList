using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ToDoList.Models
{
    public class ToDo
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter a description.")]
        public string? Description { get; set; }
        [Required(ErrorMessage = "Pleas entry a due date.")]
        private DateTime? DueDate { get; set; }
        [Required(ErrorMessage = "Please select a category.")]
        public string? CategoryId { get; set; }
        [ValidateNever]
        public Category? Category { get; set; }
        [Required(ErrorMessage = "Please select a status.")]
        public string? StatusId { get; set; }
        [ValidateNever]
        public Status? Status { get; set; }
        public bool Overdue => StatusId == "open" && DueDate < DateTime.Today;
    }
}

