namespace JewelryInventory.Api.Exceptions;

public class UniqueConstraintViolationException : Exception
{
    public UniqueConstraintViolationException(Exception innerException)
        : base("A database unique constraint was violated.", innerException)
    {
    }
}