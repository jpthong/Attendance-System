using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AttendanceSystem.Models;

public class ExcuseLetter {

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id {get;set;}

    [BsonElement("studentId")]
    public string StudentId {get;set;} = "";

    [BsonElement("classId")]
    public string ClassId {get;set;} = "";

    [BsonElement("subjectName")]
    public string SubjectName {get;set;} = "";

    [BsonElement("date")]
    public string Date {get;set;} = "";

    [BsonElement("reason")]
    public string Reason {get;set;} = "";

    [BsonElement("status")]
    public string Status {get;set;} = "Pending";
}