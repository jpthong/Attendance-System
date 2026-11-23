using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using AttendanceSystem.Data;
using System.Security.Claims;
using System.Collections.Generic;
using AttendanceSystem.Models;
using System.Linq;

namespace AttendanceSystem.Controllers;

[Authorize(Roles = "Teacher")]
public class TeacherController : Controller {

    private readonly MongoDbContext _db;

    public TeacherController(MongoDbContext db){
        _db = db;
    }
    public async Task<IActionResult> Index()
        {
            var teacherId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var myClasses = await _db.Classes.Find(c => c.TeacherId == teacherId).ToListAsync();        
            return View(myClasses);
        }

    [HttpGet]
    public async Task<IActionResult> TakeAttendance(string id) {

        var teacherId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var theClass = await _db.Classes.Find(c => c.Id == id).FirstOrDefaultAsync();

        if(theClass == null || theClass.TeacherId != teacherId) return NotFound();

        ViewBag.ClassId = id;
        ViewBag.ClassName = theClass.SubjectName;
        ViewBag.Students = await _db.Users.Find(u => theClass.StudentIds.Contains(u.Id!)).ToListAsync();  
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> TakeAttendance(string id, Dictionary<string, string> statuses) {
       
        var teacherId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var today = DateTime.Now.ToString("yyyy-MM-dd");
        var theClass = await _db.Classes.Find(c => c.Id == id).FirstOrDefaultAsync();

        if(theClass == null || theClass.TeacherId != teacherId) return NotFound();

        foreach (var item in statuses) {

            var update = Builders<AttendanceRecord>.Update.Set(r => r.Status, item.Value)
            .Set(r => r.TeacherId, teacherId!).Set(r => r.SubjectName, theClass.SubjectName);

            await _db.Attendance.UpdateOneAsync(r => r.ClassId == id && r.StudentId == item.Key && r.Date == today, update,
            new UpdateOptions { IsUpsert = true});
            
        }

        return RedirectToAction("Index");
    }   

    [HttpGet]
    public async Task<IActionResult> ExcuseLetters() {
        var teacherId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var myClasses = await _db.Classes.Find(c => c.TeacherId == teacherId).ToListAsync();
        var classIds = myClasses.Select(c => c.Id).ToList();
        var letters = await _db.ExcuseLetters.Find(l => classIds.Contains(l.ClassId) && l.Status == "Pending").ToListAsync(); 

        return View(letters);
    }

    [HttpPost]
    public async Task<IActionResult> Review(string id, string decision) {

        var teacherId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var letter = await _db.ExcuseLetters.Find(l => l.Id == id).FirstOrDefaultAsync();
        if(letter == null) return NotFound();

        
        var myClasses = await _db.Classes.Find(c => c.Id == letter.ClassId).FirstOrDefaultAsync();
        if(myClasses.TeacherId != teacherId) return NotFound();
        if(myClasses == null) return NotFound();

        var update = Builders<ExcuseLetter>.Update.Set(l => l.Status, decision);
        await _db.ExcuseLetters.UpdateOneAsync(l => l.Id == id, update);

        if (decision == "Approved") {
            var excused = Builders<AttendanceRecord>.Update.Set(l => l.Status, "Excused");     
            await _db.Attendance.UpdateOneAsync(r => r.ClassId == letter.ClassId && 
            r.StudentId == letter.StudentId && r.Date == letter.Date, excused);
        }

        return RedirectToAction("ExcuseLetters"); 
    }

    [HttpGet]
    public async Task<IActionResult> Report(string id) {
        
        var teacherId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var theClass = await _db.Classes.Find(c => c.Id == id).FirstOrDefaultAsync();

        ViewBag.ClassName = theClass.SubjectName;
        ViewBag.Students = await _db.Users.Find(u => theClass.StudentIds.Contains(u.Id!)).ToListAsync();
        ViewBag.Records = await _db.Attendance.Find(r => r.ClassId == id).ToListAsync();
        return View();
    }

}