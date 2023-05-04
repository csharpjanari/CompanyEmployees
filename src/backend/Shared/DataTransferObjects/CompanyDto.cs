using System.Runtime.Serialization;

namespace Shared.DataTransferObjects;

[Serializable, DataContract]
public record CompanyDto
{
    [DataMember]
    public Guid Id { get; init; }
    [DataMember]
    public string Name { get; init; }
    [DataMember]
    public string FullAddress { get; init; }
}