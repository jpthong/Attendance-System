using System.ComponentModel.DataAnnotations;

namespace AttendanceSystem.Models;

public class RegisterViewModel {

    [Required]
    public String FullName {get; set;} = "";

    [Required, EmailAddress]
    public string Email {get; set;} = "";

    public string? StudentNumber {get; set;} 

    [Required, MinLength(6)]
    public string Password {get; set;} = "";

    [Required, Compare("Password")]
    public string ConfirmPassword {get; set;} = "";
}