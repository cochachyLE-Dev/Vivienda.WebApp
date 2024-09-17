namespace Vivienda.WebApp.BackOffice.BFF.Service.Exceptions
{
    public class ApiException: Exception
    {
        public ApiException(string message): base(message) { }
    }
}
