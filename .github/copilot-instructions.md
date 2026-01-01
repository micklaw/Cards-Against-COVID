# GitHub Copilot Instructions - Cards Against COVID

## Project Overview

Cards Against COVID is a web-based implementation of Cards Against Humanity built with React and Azure Functions. The game includes approximately 7,000 black prompt cards and 24,000 white response cards.

**Important:** This project is free and open-source. As per CAH's Terms & Conditions, no one makes any money from this project.

## Architecture

### Project Structure

This solution consists of three main projects:

1. **CardsAgainstHumanity.Application** (.NET 8.0)
   - Shared business logic and models
   - Card services and word list management
   - Common interfaces and extensions

2. **CardsAgainstHumanity.Api** (.NET 8.0)
   - Azure Functions v4 serverless backend (Isolated Worker Model)
   - HTTP triggers for game operations
   - Long polling for real-time state updates
   - Actor Table Entities for state management
   - OpenTelemetry instrumentation

3. **CardsAgainstHumanity.Web** (React + TypeScript)
   - React client-side application with Vite
   - Redux Toolkit for state management
   - Axios for API communication
   - Long polling for real-time updates
   - OpenTelemetry instrumentation

### Key Technologies

- **.NET 8.0** for backend
- **Azure Functions v4** (Isolated Worker Model) for serverless API
- **React + TypeScript** with Vite for frontend
- **Redux Toolkit** for state management
- **Azure Table Storage** with ActorTableEntities pattern
- **Long Polling** for real-time state synchronization
- **OpenTelemetry** for observability and logging
- **Axios** for HTTP requests
- **Polly** for HTTP resilience and retry policies

## Coding Standards

### C# Style Guidelines

- Follow standard C# naming conventions (PascalCase for classes/methods, camelCase for local variables)
- Use interface-based design patterns
- Implement dependency injection for services
- Place using statements at the top of files
- Use explicit access modifiers (public, private, etc.)
- Prefer readonly fields when appropriate

### TypeScript/React Style Guidelines

- Use functional components with hooks
- Use TypeScript for all components and utilities
- Follow React best practices (composition, hooks, etc.)
- Use Redux Toolkit for state management (slices, createAsyncThunk)
- Use arrow functions for component definitions
- Prefer const over let when possible

### Architecture Patterns

- **Dependency Injection:** Use constructor injection for all backend dependencies
- **Repository/Service Pattern:** Services should be injected through interfaces
- **State Management:** Use Redux Toolkit slices and async thunks for React state
- **Actor Model:** Use ActorTableEntities for Azure Table Storage operations
- **API Design:** Azure Functions should follow RESTful conventions
- **Long Polling:** Frontend polls backend for state updates using version tracking

### File Organization

- Keep related functionality together in appropriate folders
- Separate models, services, interfaces, and extensions into distinct folders
- Use namespaces that match the folder structure (C#)
- Use index.ts for barrel exports (TypeScript)

## Building and Testing

### Prerequisites

- .NET SDK 8.0.x
- Node.js 20.x+
- Azure Functions Core Tools v4 (for local development)
- Visual Studio 2022+ or VS Code with C# and ESLint extensions

### Build Commands

```bash
# Backend - Restore dependencies
dotnet restore

# Backend - Build the solution
dotnet build --configuration Release

# Backend - Build specific project
dotnet build CardsAgainstHumanity.Api/CardsAgainstHumanity.Api.csproj

# Frontend - Install dependencies
cd CardsAgainstHumanity.Web
npm install

# Frontend - Build
npm run build

# Frontend - Development server
npm run dev
```

### Running Locally

```bash
# Run Azure Functions locally
cd CardsAgainstHumanity.Api
func start

# Run React dev server (in a separate terminal)
cd CardsAgainstHumanity.Web
npm run dev
```

### CI/CD Pipeline

The project uses GitHub Actions with the following pipelines:

- **pr-build.yml** - Build and validate on pull requests
- **release.yml** - Deploy to dev/staging on branch push
- **tag-release.yml** - Deploy to production on version tags (v*.*.*)

Pipeline stages:
- **Build Stage:** Restore, build, and publish artifacts (both backend and frontend)
- **Deploy Infrastructure:** Deploy Azure resources using Bicep
- **Deploy Function App:** Deploy backend to Azure Functions
- **Deploy Static Web App:** Deploy React frontend to Azure Static Web Apps

## Azure-Specific Guidance

### Azure Functions (Isolated Worker Model)

- Use `[Function]` attribute for all function methods
- Support Anonymous authorization level
- Use route parameters for resource identifiers (e.g., `{instance}`)
- Implement proper error handling and return appropriate HTTP status codes
- Use constructor injection for dependencies
- Return `HttpResponseData` from function methods
- Use OpenTelemetry ActivitySource for tracing

### Azure Table Storage

- Use ActorTableEntities library for actor-based entity management
- Inject `IActorTableEntityClient` for table operations
- Follow partition key/row key patterns for efficient queries
- Entity names should use lowercase for consistency
- Use Version property for optimistic concurrency and change tracking

### Long Polling Pattern

- Backend: Implement polling endpoint that waits for state changes
- Backend: Use GameStateService to track and notify state changes
- Backend: Increment Version on every state mutation
- Frontend: Poll with current version, backend returns when version changes
- Frontend: Use 30-second timeout for polling requests
- Frontend: Reconnect automatically on failures with exponential backoff

### OpenTelemetry

- Backend: Use ActivitySource for distributed tracing
- Backend: Instrument HTTP calls and database operations
- Frontend: Use Web SDK for browser telemetry
- Frontend: Track user interactions and API calls
- Both: Export to Application Insights using connection string

## Security and Best Practices

### Security

- Never commit secrets, connection strings, or API keys to the repository
- Use Azure Key Vault or Application Settings for sensitive configuration
- Validate all user inputs before processing
- Use proper authentication/authorization for production deployments
- Enable HTTPS only for all endpoints
- Use CORS appropriately

### Performance

- Use async/await for all I/O operations
- Implement retry policies with Polly for external dependencies
- Cache frequently accessed data when appropriate
- Minimize polling payload sizes
- Use connection pooling for database operations

### Error Handling

- Use try-catch blocks for expected error scenarios
- Return appropriate HTTP status codes (200, 404, 400, 500, etc.)
- Log errors with OpenTelemetry for diagnostics
- Provide meaningful error messages to clients
- Handle polling timeouts gracefully

## Testing Guidelines

- Write unit tests for business logic in the Application project
- Test Azure Functions with integration tests when possible
- Mock external dependencies (Azure Storage, HTTP clients) in unit tests
- Follow Arrange-Act-Assert pattern in tests
- Use React Testing Library for component tests
- Test Redux slices and async thunks independently

## Dependencies and Packages

### Backend NuGet Packages

- **ActorTableEntities** - Custom library for actor-based table entities
- **Microsoft.Azure.Functions.Worker** - Isolated worker model runtime
- **OpenTelemetry** - Observability and tracing
- **Polly** - Resilience and transient-fault-handling library
- **Azure.Data.Tables** - Azure Table Storage SDK

### Frontend NPM Packages

- **React** - UI library
- **Redux Toolkit** - State management
- **Axios** - HTTP client
- **@opentelemetry/sdk-trace-web** - OpenTelemetry for browsers
- **react-router-dom** - Routing

### Version Management

- Use semantic versioning (v1.2.3) for releases
- Tag releases trigger production deployment
- Version numbers in project files should be synced

## Common Tasks

### Adding a New Game Feature

1. Add models to `CardsAgainstHumanity.Application/Models`
2. Create or update services in `Application/Services`
3. Add Azure Function trigger in `Api/FunctionTriggers.cs`
4. Create Redux slice in `Web/src/store/slices`
5. Update React components to use new state/actions
6. Increment Version in Game entity when state changes

### Adding a New API Endpoint

1. Add method to `Api/FunctionTriggers.cs` with `[Function]` attribute
2. Use appropriate HTTP trigger attributes and route
3. Inject required services through constructor
4. Add corresponding API call in `Web/src/api/gameApi.ts`
5. Create Redux async thunk to call the new endpoint

### Implementing Long Polling

1. Backend: Add polling endpoint that accepts current version
2. Backend: Use GameStateService.WaitForUpdate() to wait for changes
3. Frontend: Create polling service with version tracking
4. Frontend: Implement retry logic with exponential backoff
5. Frontend: Handle timeout and reconnection scenarios

### Modifying Card Content

- Black cards (prompts) are in `Application/Wordlists/blackwords.txt`
- White cards (responses) are in `Application/Wordlists/whitewords.txt`
- These files are embedded resources in the Application project

## Additional Notes

- The project was built during COVID-19 as a fun side project
- Built using the custom ActorTableEntities library
- The game is hosted on Azure Static Web Apps and Azure Functions
- OpenTelemetry provides end-to-end observability
- Long polling replaces SignalR for simplicity and cost-effectiveness
- No tests are currently implemented in the repository