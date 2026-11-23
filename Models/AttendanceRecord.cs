using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AttendanceSystem.Models;

public class AttendanceRecord {

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id {get;set;}

    [BsonElement("fullName")]
    public string FullName {get;set;} = "";

    [BsonElement("classId")]
    public string ClassId {get;set;} = "";

    [BsonElement("studentId")]
    public string StudentId {get;set;} = "";

    [BsonElement("date")]
    public string Date {get;set;} = "";

    [BsonElement("status")]
    public string Status {get;set;} = "";

    [BsonElement("teacherId")]
    public string TeacherId {get;set;} = "";

    [BsonElement("subjectName")]
    public string SubjectName {get;set;} = "";

}