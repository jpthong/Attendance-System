using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AttendanceSystem.Models;

public class User {

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id {get; set;}

    [BsonElement("fullName")] 
    public string FullName {get; set;} = "";

    [BsonElement("email")]
    public string Email {get; set;} = "";

    [BsonElement("passwordHash")]
    public string PasswordHash {get; set;} = "";

    [BsonElement("role")]
    public string Role {get; set;} = "Student";

    [BsonElement("isActive")]
    public bool IsActive {get; set;} = true; 

    [BsonElement("studentNumber")]
    public string? StudentNumber {get; set;}
}