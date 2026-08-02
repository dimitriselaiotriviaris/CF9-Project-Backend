namespace CF9Project.Exceptions
{
    public class ServerException : AppException
    {
        public ServerException(string code, string message)
            : base(code, message)
        {
        }


    }
}
