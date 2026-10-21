namespace JewelryInventory.Api.Exceptions;

public class DuplicateBarcodeException : Exception
{
    public DuplicateBarcodeException(string barcode)
        : base(barcode) { }
}