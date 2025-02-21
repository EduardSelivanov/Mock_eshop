

using CommonPractices.Exceptions;

namespace WareHouse.Domain.WareHouseExceptions
{
    public class RackAlreadyExists:AlreadyExistsExc
    {
        public RackAlreadyExists(object prop,object key):base("Rack", prop , key){}
    }
}
