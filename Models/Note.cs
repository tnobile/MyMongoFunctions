using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyNotes.Functions.Models
{
    public class Note
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Content { get; set; }
        public string Word { get; set; }
        //public DateTime CreatedAt{ get; set; }

        [BsonElement("Category")]
        public string Category { get; set; }

        public string Status { get; set; }

    }
}