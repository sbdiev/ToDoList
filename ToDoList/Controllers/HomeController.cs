﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Data;
using ToDoList.Models;
using ToDoList.UseCases;

namespace ToDoList.Controllers;

public class HomeController : Controller
{
    private ToDoListContext _context;

    public HomeController(ToDoListContext ctx) => _context = ctx;
    public IActionResult Index(string id)
    {
        var filters = new Filters(id);
        ViewBag.Filters = filters;
        ViewBag.Categories = _context.Categories.ToList();
        ViewBag.Statuses = _context.Statuses.ToList();
        ViewBag.DueFilters = Filters.DueFilterValues;

        IQueryable<ToDo> query = _context.ToDos
            .Include(t => t.Category)
            .Include(t => t.Status);
        
        if (filters.HasCategory)
        {
            query = query.Where(t => t.CategoryId == filters.CategoryId);
        }
        
        if (filters.HasStatus)
        {
            query = query.Where(t => t.StatusId == filters.StatusId);
        }
        
        if (filters.HasDue)
        {
            var toDay = DateTime.UtcNow;
            if (filters.IsPast)
            {
                query = query.Where(t => t.DueDate < toDay); 
            } else if (filters.IsFuture)
            {
                query = query.Where(t => t.DueDate > toDay);
            } else if (filters.IsToday)
            {
                query = query.Where(t => t.DueDate == toDay);
            }
        }
        var tasks = query.OrderBy(t => t.DueDate).ToList();
        return View(tasks);
    }

    [HttpGet]
    public IActionResult Add()
    {
        ViewBag.Categories = _context.Categories.ToList();
        ViewBag.Statuses = _context.Statuses.ToList();
        var task = new ToDo { StatusId = "open" };

        return View(task);
    }

    [HttpPost]
    public IActionResult Add(ToDo task)
    {
        if (ModelState.IsValid)
        {
            task.DueDate = task.DueDate?.ToUniversalTime();

            _context.ToDos.Add(task);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        else
        {
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Statuses = _context.Statuses.ToList();
            return View(task);
        }
    }

    [HttpPost]
    public IActionResult Filter(string[] filter)
    {
        string id = string.Join('-', filter);
        return RedirectToAction("Index", new { ID = id });
    }

    [HttpPost]
    public IActionResult MarkComplete([FromRoute] string id, ToDo selected)
    {
        selected = _context.ToDos.Find(selected.Id)!;

        selected.StatusId = "closed";
        _context.SaveChanges();

        return RedirectToAction("Index", new { ID = id });
    }

    [HttpPost]
    public IActionResult DeleteComplete(string id)
    {
        var toDelete = _context.ToDos.Where(t => t.StatusId == "closed").ToList();

        foreach (var task in toDelete)
        {
            _context.ToDos.Remove(task);
        }

        _context.SaveChanges();

        return RedirectToAction("Index", new { ID = id });
    }
}