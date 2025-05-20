using adminProfolio.Dtos;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace adminportfolio.Models
{
    public class Configuracion
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public required string UserId { get; set; }


        [BsonElement("sections")]
        public required List<Section> Sections { get; set; }

    }
}
