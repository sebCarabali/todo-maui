namespace LoginApplication.Exceptions;

public class AppException : Exception
{
    public string Title { get; }
    public int StatusCode { get; }


    public AppException(string title, string message, int statusCode = 500) : base(message)
    {
        Title = title;
        StatusCode = statusCode;
    }

    public AppException(string message, Exception innerException) 
        : base(message, innerException)
    {
        Title = "Error";
        StatusCode = 500;
    }
}

public class AppUnauthorizedException : AppException
{
    public AppUnauthorizedException(string message) 
        : base("No autorizado", message, 401) { }
}

public class AppValidationException : AppException
{
    public IDictionary<string, string[]> Errors { get; }

    public AppValidationException(IDictionary<string, string[]> errors) 
        : base("Error de validación", "Uno o más errores de validación han ocurrido", 400)
    {
        Errors = errors;
    }
}
