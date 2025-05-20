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

        [BsonElement("title")]
        public required string Title { get; set; }

        [BsonElement("description")]
        public required string Description { get; set; }

        [BsonElement("component")]
        public required string Component { get; set; }

        [BsonElement("componentIdentifier")]
        public required string ComponentIdentifier { get; set; }

        [BsonElement("componentType")]
        public required string ComponentType { get; set; }

        [BsonElement("componentVersion")]
        public required string ComponentVersion { get; set; }


        [BsonElement("loadDataAction")]
        public string? LoadDataAction { get; set; }

        [BsonElement("actions")]
        public List<string>? Actions { get; set; }
    }
}