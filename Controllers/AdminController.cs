using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AttendanceSystem.Data;
using AttendanceSystem.Models;
using MongoDB.Driver;
using Microsoft.AspNetCore.Identity;

namespace AttendanceSystem.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller {
    private readonly MongoDbContext _db;
    private readonly PasswordHasher<User> _hash = new();
    public async Task<IActionResult> Index()
        {
            var existingClasses = await _db.Classes.Find(_ => true).ToListAsync();
            return View(existingClasses);
        }
        
    public AdminController(MongoDbContext db) {

        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> CreateClass() {
        
        var teachers = await _db.Users.Find(u => u.Role == "Teacher").ToListAsync();
        ViewBag.Teachers = teachers;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateClass(CreateClassViewModel model) {

        if (!ModelState.IsValid) {
            ViewBag.Teachers = await _db.Users.Find(u => u.Role == "Teacher").ToListAsync();
             return View(model);
        }

        var newClass = new ClassRoom {

            SubjectName = model.SubjectName.Trim(),
            Section = model.Section.Trim(),
            TeacherId = model.TeacherId
        };

        await _db.Classes.InsertOneAsync(newClass);
        return RedirectToAction("Index"); 
    }
    [HttpGet]
    public async Task<ActionResult> Enroll(string id) {

        ViewBag.ClassId = id;
        ViewBag.Students = await _db.Users.Find(u => u.Role == "Student").ToListAsync();
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Enroll(string id ,string studentId) {

        if(string.IsNullOrEmpty(studentId)) {
            ViewBag.ClassId = id;
            ViewBag.Students = await _db.Users.Find(u => u.Role == "Student").ToListAsync();
            return View();
        }

        var update = Builders<ClassRoom>.Update.AddToSet(c => c.StudentIds, studentId);
        await _db.Classes.UpdateOneAsync(c => c.Id == id, update);

        
        return RedirectToAction("Index"); 
    }

    [HttpGet]
    public IActionResult CreateTeacher() => View();

    [HttpPost]
    public async Task<IActionResult> CreateTeacher(RegisterViewModel model) {

        if (!ModelState.IsValid) return View(model);

        var email = model.Email.Trim().ToLower();
        var exist = await _db.Users.Find(u => u. Email == email).AnyAsync();

        if(exist) {

            ModelState.AddModelError("Email", "Email is already registered.");
            return View(model);
        }

        var user = new User {

            FullName = model.FullName.Trim(),
            Email = email,
            Role = "Teacher"
        };

        user.PasswordHash = _hash.HashPassword(user, model.Password);

        await _db.Users.InsertOneAsync(user);
        return RedirectToAction("Index");
    } 

}