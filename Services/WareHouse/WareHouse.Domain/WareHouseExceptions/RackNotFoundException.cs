

using CommonPractices.Exceptions;

namespace WareHouse.Domain.WareHouseExceptions
{
    public class RackNotFoundException:NotFoundExc
    {
        public RackNotFoundException(Guid id) : base("Rack",id) { }
    }
}
