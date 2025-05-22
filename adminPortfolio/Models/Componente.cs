using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace adminportfolio.Models
{
    public class Componente
    {
        [BsonId]  
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("type")]
        public required string Type { get; set; }

        [BsonElement("identifier")]
        public required string Identifier { get; set; }

        [BsonElement("props_json")]
        public BsonDocument? Props_json { get; set; }
    }
}