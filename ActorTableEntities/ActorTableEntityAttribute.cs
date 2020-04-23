using System;
using Microsoft.Azure.WebJobs.Description;

namespace ActorTableEntities
{
    [Binding]
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    public class ActorTableEntityAttribute : Attribute
    {
    }
}
