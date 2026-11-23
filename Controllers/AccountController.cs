using AttendanceSystem.Data;
using AttendanceSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

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

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost] 
    public async Task<IActionResult> Login(LoginViewModel model) {

        if(!ModelState.IsValid) return View(model);

        var email = model.Email.Trim().ToLower();
        var user = await _db.Users.Find(u => u.Email == email).FirstOrDefaultAsync(); 

        if(user == null || !user.IsActive || _hasher.VerifyHashedPassword(user, user.PasswordHash, model.Password) == PasswordVerificationResult.Failed) 
        {
            ModelState.AddModelError("", "Invalid Email or Password.");
            return View(model);
        } 

        var claims = new List<Claim> {

            new(ClaimTypes.NameIdentifier, user.Id!),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Role, user.Role)
        };

        var identity = new ClaimsIdentity(claims, "Cookies");
        await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(identity));

        
        switch(user.Role) {

            case "Teacher":
                return RedirectToAction("Index", "Teacher");
                  
            case "Admin":
                return RedirectToAction("Index", "Admin");
                   
            case "Student":
                return RedirectToAction("Index", "Student");
                     
            default:
                return RedirectToAction("Index", "Home");               
        }
    }

    [HttpPost]
    public async Task<IActionResult> Logout() {
        
        await HttpContext.SignOutAsync("Cookies");
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult AccessDenied() => View();
}