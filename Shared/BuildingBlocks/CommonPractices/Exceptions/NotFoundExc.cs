﻿
namespace CommonPractices.Exceptions
{
    public class NotFoundExc:Exception
    {
        
        public NotFoundExc(string name):base(name) 
        {
        }
        public NotFoundExc(string name,object key):base($"Entity {name} with key: {key} was not found...")
        {
        }
    }
}
