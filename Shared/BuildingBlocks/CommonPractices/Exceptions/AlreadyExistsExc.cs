

namespace CommonPractices.Exceptions
{
    public class AlreadyExistsExc:Exception
    {
        public AlreadyExistsExc(string name):base(name){}

        public AlreadyExistsExc(string name,object prop,object key): base($"Entity {name} with {prop} = {key} was already added") { }
    }
}
