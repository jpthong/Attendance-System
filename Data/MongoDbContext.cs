using AttendanceSystem.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AttendanceSystem.Data;

public class MongoDbContext {

    private readonly IMongoDatabase _db;

    public MongoDbContext(IOptions<MongoDbSettings> settings) {

        var client = new MongoClient(settings.Value.ConnectionString);
        _db = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<User> Users => _db.GetCollection<User>("users");
}