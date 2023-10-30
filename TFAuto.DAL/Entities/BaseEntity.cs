using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Attributes;

namespace TFAuto.DAL.Entities;

[PartitionKeyPath("/partitionKey")]
public class BaseEntity : FullItem
{
    public virtual string PartitionKey { get; set; }

    protected override string GetPartitionKeyValue() => PartitionKey;
}