using System.Runtime.Serialization;

namespace Common
{
    [DataContract]
    public class ValidationFault
    {
        string message;
        string fieldName;
        string invalidValue;

        public ValidationFault(string message, string fieldName, string invalidValue)
        {
            this.message = message;
            this.fieldName = fieldName;
            this.invalidValue = invalidValue;
        }

        [DataMember]
        public string Message { get => message; set => message = value; }

        [DataMember]
        public string FieldName { get => fieldName; set => fieldName = value; }

        [DataMember]
        public string InvalidValue { get => invalidValue; set => invalidValue = value; }

        public override string ToString()
        {
            return $"ValidationFault - Field: {fieldName} | " +
                   $"Value: {invalidValue} | Message: {message}";
        }
    }
}