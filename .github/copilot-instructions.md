# GitHub Copilot Instructions - Cards Against COVID

## Project Overview

Cards Against COVID is a web-based implementation of Cards Against Humanity built with Blazor WebAssembly and Azure Functions. The game includes approximately 7,000 black prompt cards and 24,000 white response cards.

**Important:** This project is free and open-source. As per CAH's Terms & Conditions, no one makes any money from this project.

## Architecture

### Project Structure

This solution consists of three main projects:

1. **CardsAgainstHumanity.Application** (.NET Standard 2.1)
   - Shared business logic and models
   - Card services and word list management
   - Common interfaces and extensions

2. **CardsAgainstHumanity.Api** (.NET Core 3.1)
   - Azure Functions v3 serverless backend
   - HTTP triggers for game operations
   - SignalR integration for real-time updates
   - Actor Table Entities for state management

3. **CardsAgainstHumanity.UI** (Blazor WebAssembly)
   - Client-side Blazor application
   - Fluxor state management (Redux pattern)
   - Refit for API client generation
   - SignalR client for real-time features

### Key Technologies

- **.NET Core 3.1** and **.NET Standard 2.1**
- **Azure Functions v3** for serverless API
- **Blazor WebAssembly** for client-side UI
- **Azure Table Storage** with ActorTableEntities pattern
- **SignalR** for real-time communication
- **Fluxor** for Redux-style state management
- **Refit** for typed HTTP clients
- **Polly** for HTTP resilience and retry policies

## Coding Standards

### C# Style Guidelines

- Follow standard C# naming conventions (PascalCase for classes/methods, camelCase for local variables)
- Use interface-based design patterns
- Implement dependency injection for services
- Place using statements at the top of files
- Use explicit access modifiers (public, private, etc.)
- Prefer readonly fields when appropriate

### Architecture Patterns

- **Dependency Injection:** Use constructor injection for all dependencies
- **Repository/Service Pattern:** Services should be injected through interfaces
- **State Management:** Use Fluxor actions, reducers, and effects for UI state
- **Actor Model:** Use ActorTableEntities for Azure Table Storage operations
- **API Design:** Azure Functions should follow RESTful conventions

### File Organization

- Keep related functionality together in appropriate folders
- Separate models, services, interfaces, and extensions into distinct folders
- Use namespaces that match the folder structure

## Building and Testing

### Prerequisites

- .NET Core SDK 3.1.x
- Visual Studio 2019+ or VS Code with C# extension
- Azure Functions Core Tools (for local development)

### Build Commands

```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build --configuration Release

# Build specific project
dotnet build CardsAgainstHumanity.Api/CardsAgainstHumanity.Api.csproj
```

### Running Locally

```bash
# Run Azure Functions locally
cd CardsAgainstHumanity.Api
func start

# Run Blazor UI (in a separate terminal)
cd CardsAgainstHumanity.UI
dotnet run
```

### CI/CD Pipeline

The project uses Azure Pipelines with the following stages:
- **Build Stage:** Restore, build, and publish artifacts
- **Release Stage:** Deploy to Azure (only for master/develop branches)

Build configuration is defined in:
- `azure-pipelines.yml` - Main pipeline definition
- `Build/build.yml` - Build steps template
- `Build/release.yml` - Release steps template

## Azure-Specific Guidance

### Azure Functions

- Use `[FunctionName]` attribute for all function methods
- Support both Anonymous and Function-level authorization
- Use route parameters for resource identifiers (e.g., `{instance}`)
- Implement proper error handling and return appropriate HTTP status codes
- Use `IAsyncCollector<SignalRMessage>` for SignalR message broadcasting

### Azure Table Storage

- Use ActorTableEntities library for actor-based entity management
- Inject `IActorTableEntityClient` for table operations
- Follow partition key/row key patterns for efficient queries
- Entity names should use lowercase for consistency

### Azure SignalR Service

- Hub name is "cah" (Cards Against Humanity)
- Use SignalR binding attributes in Azure Functions
- Implement connection negotiation endpoint for clients
- Handle reconnection logic on the client side

## Security and Best Practices

### Security

- Never commit secrets, connection strings, or API keys to the repository
- Use Azure Key Vault or Application Settings for sensitive configuration
- Validate all user inputs before processing
- Use proper authentication/authorization for production deployments

### Performance

- Use async/await for all I/O operations
- Implement retry policies with Polly for external dependencies
- Cache frequently accessed data when appropriate
- Minimize SignalR message sizes

### Error Handling

- Use try-catch blocks for expected error scenarios
- Return appropriate HTTP status codes (200, 404, 400, 500, etc.)
- Log errors for diagnostics (when logging infrastructure exists)
- Provide meaningful error messages to clients

## Testing Guidelines

- Write unit tests for business logic in the Application project
- Test Azure Functions with integration tests when possible
- Mock external dependencies (Azure Storage, SignalR) in unit tests
- Follow Arrange-Act-Assert pattern in tests

## Dependencies and Packages

### Key NuGet Packages

- **ActorTableEntities** - Custom library for actor-based table entities
- **Fluxor** - Redux-style state management for Blazor
- **Refit** - Automatic type-safe REST library
- **Polly** - Resilience and transient-fault-handling library
- **WindowsAzure.Storage** - Azure Storage SDK
- **Blazored.LocalStorage** - Blazor local storage access

### Version Management

- Use Nerdbank.GitVersioning (NBGV) for automatic semantic versioning
- Version configuration is in `version.json`
- All projects reference versioning through `Directory.Build.props`

## Common Tasks

### Adding a New Game Feature

1. Add models to `CardsAgainstHumanity.Application/Models`
2. Create or update services in `Application/Services`
3. Add Azure Function trigger in `Api/FunctionTriggers.cs`
4. Create Fluxor actions, reducers, and effects in UI project
5. Update Blazor components to use new state/actions

### Adding a New API Endpoint

1. Add method to `Api/FunctionTriggers.cs`
2. Use appropriate HTTP trigger attributes and route
3. Inject required services through constructor
4. Add corresponding method to `UI/Clients/IApiClient.cs` (Refit interface)
5. Create Fluxor effect to call the new endpoint

### Modifying Card Content

- Black cards (prompts) are in `Application/Wordlists/blackwords.txt`
- White cards (responses) are in `Application/Wordlists/whitewords.txt`
- These files are embedded resources in the Application project

## Additional Notes

- The project was built during COVID-19 as a fun side project
- Built using the custom ActorTableEntities library: https://github.com/micklaw/Actor-Table-Entities
- The game is hosted on Azure Static Web Apps
- No tests are currently implemented in the repository
