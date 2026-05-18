using System.Runtime.Serialization;

namespace Common
{
    [DataContract]
    public enum ResponseStatus
    {
        [EnumMember] ACK,
        [EnumMember] NACK
    }

    [DataContract]
    public enum SessionStatus
    {
        [EnumMember] IN_PROGRESS,
        [EnumMember] COMPLETED
    }
}