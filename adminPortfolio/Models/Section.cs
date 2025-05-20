using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace adminportfolio.Models
{
    public class Section
    {

        [BsonElement("Order")]
        public int Order { get; set; }

        [BsonElement("identifier")]
        public required string Identifier { get; set; }

        [BsonElement("type")]
        public required string Type { get; set; }

        [BsonElement("title")]
        public string? Title { get; set; } = null!;

        [BsonElement("idecomponente_identifierntifier")]
        public string? Componente_identifier { get; set; } = null!;


        [BsonElement("renderClient")]
        public bool? RenderClient { get; set; } =  null!;

        [BsonElement("content")]
        public Content? Content { get; set; } = null!;


        [BsonElement("isNavbar")]
        public bool? IsNavbar { get; set; }

    }

    public class Content
    {
        [BsonElement("position")]
        public required string Position { get; set; }

        [BsonElement("isHidden")]
        public bool? IsHidden { get; set; }

        [BsonElement("stuffed")]
        public bool? Stuffed { get; set; }

        [BsonElement("componente_identifier")]
        public required string ComponenteIdentifier { get; set; }
    }

}
