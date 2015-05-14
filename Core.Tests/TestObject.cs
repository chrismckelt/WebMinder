using System;
using WebMinder.Core.Rules;

namespace WebMinder.Core.Tests
{
    public class TestObject : IRuleRequest
    {
        public int IntegerProperty { get; set; }
        public string StringProperty { get; set; }
        public decimal DecimalProperty { get; set; }

        public Guid Id { get; set; }
        public DateTime CreatedUtcDateTime { get; set; }

        public static TestObject Build()
        {
           var rand = new Random();

           return new TestObject()
           {
            
               CreatedUtcDateTime = DateTime.UtcNow,
               DecimalProperty = Convert.ToDecimal(rand.NextDouble()),
               IntegerProperty = rand.Next(),
               StringProperty = "string_" + rand.Next().ToString()
               
           };
        }
    }
}
