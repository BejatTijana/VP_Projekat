using System.Runtime.Serialization;

namespace Common
{
    [DataContract]
    public class DataFormatFault
    {
        string message;
        string fieldName;

        public DataFormatFault(string message, string fieldName)
        {
            this.message = message;
            this.fieldName = fieldName;
        }

        [DataMember]
        public string Message { get => message; set => message = value; }

        [DataMember]
        public string FieldName { get => fieldName; set => fieldName = value; }

        public override string ToString()
        {
            return $"DataFormatFault - Field: {fieldName} | Message: {message}";
        }
    }
}