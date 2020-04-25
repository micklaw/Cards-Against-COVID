# Actor Table Entities
A play on Azure Functions Durable Entities without the queuing. Locks a blob behind the scenes to ensure the actor can only be amended once, then free us for the next connection.

[![Build Status](https://dev.azure.com/mlwdltd/Actor%20Table%20Entities/_apis/build/status/micklaw.Actor-Table-Entities?branchName=develop)](https://dev.azure.com/mlwdltd/Actor%20Table%20Entities/_build/latest?definitionId=10&branchName=develop)

## Why not use Durable Entities?
I did, honestly, and yes they are amazing, but for my specific use case they did fit well. I wanted something that was:

* Quick to respond
* Wasn't meant for scale on a single entity (Max 10-20 consumers of an entity)
* Controllable via standard functions
* Cheaper

Where as durableEntities are great, due to the nature of the queuing involved using Orchestrator functions, it meant when release I could wait or a good few seconds anywhere between 2-10
for my request to complete, then if it did, I would generally have to get a status endpoint to monitor my result.

Next up I attempted to go straight to the Entity and its operations, but the lack of responses from the operations without meaningful HTTP responses stopped me.

So, I built this...

## Usage
So this is a typical entity, inheriting from ITableEntity, it will allow you to put complex types as properties, it will also allow for you to interact with the actual entity by claiming a lock
just before reading, if it fails to get a lock, it will retry every Xms for X attempts as defined in your config.

```csharp
public class Counter : ActorTableEntity
{
    public int Count { get; set; }

    public Counter Increment()
    {
        Count = Count + 1;

        return this;
    }
}
```

You can see a sample function in the main project, but it looks a bit like this.

```csharp
[FunctionName("UpdateHttpApi")]
public async Task<IActionResult> Update(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "update/{name}")] HttpRequest req, string name,
    [ActorTableEntity] IActorTableEntityClient entityClient)
{
    await using var state = await entityClient.GetLocked<Counter>("entity", name);

    state.Entity.Increment();

    await state.Flush();

    return new OkObjectResult(state.Entity);
}
```

The code above lets you take a hold of an entity, do some stuff on it, then release the lock, allowing the next punter to take it up.

## Setup
Finally, install the nuget package above, and bootstrap your code like so.

```csharp
public class Startup : IWebJobsStartup
{
    public void Configure(IWebJobsBuilder builder)
    {
        builder.AddActorTableEntities(options =>
        {
            options.StorageConnectionString = "UseDevelopmentStorage=true";
            options.ContainerName = "entitylocks";
            options.WithRetry = true;
            options.RetryIntervalMilliseconds = 100;
        });
    }
}
```

## Built with it

### Cards Against COVID

Have a play and see what you think, I built this with it:

https://stcardshumanity.z33.web.core.windows.net/