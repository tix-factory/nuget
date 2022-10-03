using System.Runtime.Serialization;

namespace TixFactory.Data.MySql
{
    [DataContract]
    internal class InsertResult<T>
    {
        [DataMember(Name = "ID")]
        public T Id { get; set; }
    }
}
