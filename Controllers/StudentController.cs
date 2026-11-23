using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using AttendanceSystem.Data;
using System.Security.Claims;
using AttendanceSystem.Models;

namespace AttendanceSystem.Controllers;

[Authorize(Roles = "Student")]
public class StudentController : Controller {
    private readonly MongoDbContext _db;

    public StudentController(MongoDbContext db) {
        
        _db = db;
    }
    public async Task<IActionResult> Index()
        {
            var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var myClasses = await _db.Classes.Find(c => c.StudentIds.Contains(studentId!)).ToListAsync();
            return View(myClasses);
        }

    [HttpGet]
    public async Task<IActionResult> Attendance(string id) {

        var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var records  = await _db.Attendance.Find(r => r.ClassId == id && r.StudentId == studentId).ToListAsync();

        ViewBag.Absences = records.Count (r => r.Status == "Absent");
        return View(records);
    }  

    [HttpGet]
    public IActionResult SubmitExcuse(string classId, string date) {
       
        ViewBag.ClassId = classId;
        ViewBag.Date = date;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SubmitExcuse(string classId, string date, string reason) {

        var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var myClass = await _db.Classes.Find(c => c.Id == classId).FirstOrDefaultAsync();

        if(myClass == null || !myClass.StudentIds.Contains(studentId!)) return NotFound();
        var excuseLetter = new ExcuseLetter {

            StudentId = studentId!,
            ClassId = classId,
            Date = date,
            SubjectName = myClass.SubjectName,
            Reason = reason.Trim()
        };

        var exist = await _db.ExcuseLetters.Find(l => l.ClassId == classId && l.StudentId == studentId && l.Date == date && l.Status != "Rejected").AnyAsync();

        if (exist) {
            return RedirectToAction("Attendance", new { id = classId });
        }

        var record = await _db.Attendance.Find(r => r.ClassId == classId && r.StudentId == studentId && r.Date == date ).FirstOrDefaultAsync();
        if (record == null || record.Status != "Absent") {
            return RedirectToAction("Attendance", new {id = classId});
        }
        await _db.ExcuseLetters.InsertOneAsync(excuseLetter);
        return RedirectToAction("Attendance", new {id = classId});
    }



}