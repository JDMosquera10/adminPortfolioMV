namespace adminProfolio.Interfaces
{
    public interface IEmailService
    {
        Task EnviarCodigoVerificacion(string email, string nombre, string codigo);
    }
}