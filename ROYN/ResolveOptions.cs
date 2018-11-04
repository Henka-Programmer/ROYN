using System.Runtime.Serialization;

namespace ROYN
{
    [DataContract]
    public enum ResolveOptions
    {
        [EnumMember]
        Name,

        [EnumMember]
        FullName,

        [EnumMember]
        AssemblyQualifiedName
    }
}