using AttendanceSystem.Data;
using AttendanceSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace AttendanceSystem.Controllers;

public class AccountController : Controller {

    private readonly MongoDbContext _db;
    private readonly PasswordHasher<User> _hasher = new();

    public AccountController(MongoDbContext db) {

        _db = db;
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model) {

        if (!ModelState.IsValid) return View(model);

        var email = model.Email.Trim().ToLower();
        var exist = await _db.Users.Find(u => u.Email == email).AnyAsync();

        if (exist) {

            ModelState.AddModelError("Email", "Email is already registered.");
            return View(model);
        }

        var user = new User {

            FullName = model.FullName.Trim(),
            Email = email,
            StudentNumber = model.StudentNumber,
            Role = "Student"

        };

        user.PasswordHash = _hasher.HashPassword(user, model.Password);

        await _db.Users.InsertOneAsync(user);
        return RedirectToAction("Index", "Home"); 
    }
}