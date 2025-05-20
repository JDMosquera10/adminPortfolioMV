namespace adminProfolio.Dtos
{
    public class CreateUserDto
    {
        public string Id { get; set; } = string.Empty;
        public string fullname { get; set; } = string.Empty;
        public double phone_number { get; set; } = double.NaN;
        public string password { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
    }
}
