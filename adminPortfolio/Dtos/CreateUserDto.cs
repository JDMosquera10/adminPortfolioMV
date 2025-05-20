using Swashbuckle.AspNetCore.Annotations;

namespace adminProfolio.Dtos
{
    public class CreateUserDto
    {
        [SwaggerIgnore]
        public string Id { get; set; } = string.Empty;
        public string fullname { get; set; } = string.Empty;
        public long phone_number { get; set; }
        public string password { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
    }
}
