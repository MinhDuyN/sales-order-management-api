namespace OrderAPI.Exceptions
{
    public class ConflictDataException : Exception
    {
        public ConflictDataException(string message) : base(message) { }
    }
}
