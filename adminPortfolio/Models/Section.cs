using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace adminportfolio.Models
{
    public class Section
    {

        [BsonElement("Order")]
        public int Order { get; set; }

        [BsonElement("Identifier")]
        public required string Identifier { get; set; }

        [BsonElement("Type")]
        public required string Type { get; set; }

        [BsonElement("Title")]
        public string? Title { get; set; } = null!;

        [BsonElement("Componente_identifier")]
        public string? Componente_identifier { get; set; } = null!;

        [BsonElement("contentData")]
        public BsonDocument? ContentData { get; set; }

        [BsonElement("RenderClient")]
        [BsonRepresentation(BsonType.Boolean)]
        public bool? RenderClient { get; set; } =  false;

        [BsonElement("Content")]
        public List<Content>? Content { get; set; } = null!;

        [BsonElement("IsNavbar")]
        [BsonRepresentation(BsonType.Boolean)]
        public bool? IsNavbar { get; set; } = false;

    }

    public class Content
    {
        [BsonElement("Position")]
        public required string Position { get; set; }

        [BsonElement("IsHidden")]
        [BsonRepresentation(BsonType.Boolean)]
        public bool? IsHidden { get; set; } = false;

        [BsonElement("Stuffed")]
        [BsonRepresentation(BsonType.Boolean)]
        public bool? Stuffed { get; set; } = false;

        [BsonElement("contentData")]
        public BsonDocument? ContentData { get; set; }

        [BsonElement("ComponenteIdentifier")]
        public required string ComponenteIdentifier { get; set; }

        [BsonElement("type")]
        public required string type { get; set; }
    }

}
