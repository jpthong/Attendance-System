using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AttendanceSystem.Models;

public class ClassRoom {

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id {get; set;}

    [BsonElement("subjectName")]
    public string SubjectName {get;set;} = "";

    [BsonElement("section")]
    public string Section {get;set;} = "";

    [BsonElement("teacherId")]
    public string TeacherId {get;set;} = "";

    [BsonElement("studentIds")]
    public List<string> StudentIds {get;set;} = new(); 
}
