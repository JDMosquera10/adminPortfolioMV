using adminProfolio.Dtos;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace adminProfolio.Models

{
    public class Usuario
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; } = null!;

        [BsonElement("email")]
        public string email { get; set; } = null!;

        [BsonElement("Password")]
        public string password { get; set; } = null!;

        [BsonElement("refresh_token")]
        public string refresh_token { get; set; } = null!;

        [BsonElement("phone_number")]
        public double? phone_number { get; set; } = null;

        [BsonElement("fullname")]
        public string fullname { get; set; } = null!;

        [BsonElement("token")]
        public string token { get; set; } = null!;

        public string? VerificationCode { get; set; }
        public bool IsVerified { get; set; } =  false;
        public DateTime? VerificationCodeExpires { get; set; }

        public Usuario(CreateUserDto parameter)
        {
           this.fullname = parameter.fullname;
            this.email = parameter.email;
            this.password = parameter.password;
            this.phone_number = parameter.phone_number;
        }
    }
}

