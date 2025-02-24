using CommonPractices.Exceptions;


namespace Catalog.Domain.Exceptions
{
    public class ProductNotFound:NotFoundExc
    {
        public ProductNotFound(string SKU):base("Product",SKU){}
    }
}
