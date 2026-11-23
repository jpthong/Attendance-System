using System.ComponentModel.DataAnnotations;

namespace AttendanceSystem.Models;

public class CreateClassViewModel {

    [Required]
    public string SubjectName {get;set;} = "";

    [Required]
    public string Section {get;set;} = "";

    [Required]
    public string TeacherId {get;set;} = "";
    
}