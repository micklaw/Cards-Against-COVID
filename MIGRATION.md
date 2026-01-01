# Migration Summary: Blazor to React with Isolated Functions

## Overview

This document summarizes the comprehensive migration of Cards Against COVID from a Blazor WebAssembly + SignalR architecture to a React + Long Polling architecture with Azure Functions Isolated Worker Model.

## What Was Accomplished

### ✅ Backend Modernization

1. **Azure Functions Isolated Worker Model (.NET 8)**
   - Migrated from in-process model to isolated worker model
   - Updated to .NET 8.0 for improved performance and features
   - Modified all function triggers to use `HttpRequestData` and `HttpResponseData`
   - Updated dependency injection configuration in `Program.cs`

2. **Removed SignalR Dependency**
   - Deleted `FunctionSignalr.cs` (SignalR negotiation and group management)
   - Removed SignalR NuGet packages
   - Eliminated `IAsyncCollector<SignalRMessage>` from all functions

3. **Implemented Long Polling**
   - Created `GameStateService` for tracking game state versions
   - Created `PollingService` for managing long-poll requests
   - Added `/poll` endpoint that waits up to 30 seconds for state changes
   - Implemented version-based change detection using `Game.Version` property

4. **Version Tracking**
   - Added `Version` property to `Game` entity
   - Modified `IVersionable` interface to use `int Version` instead of `string ETag`
   - Added `IncrementVersion()` calls to all state-changing operations
   - Version is returned with every API response for client synchronization

5. **OpenTelemetry Integration**
   - Added OpenTelemetry packages for distributed tracing
   - Configured `ActivitySource` for custom spans
   - Integrated with Application Insights for cloud telemetry
   - Added trace logging to all function endpoints

6. **ActorTableEntities Configuration**
   - Kept version 0.1.0-alpha000 (v2.0.0 API not compatible)
   - Maintained existing actor-based concurrency pattern
   - Works with isolated worker model through constructor injection

### ✅ Frontend Foundation (React + TypeScript)

1. **Created React Application**
   - Bootstrapped with Vite for fast development and builds
   - Configured TypeScript for type safety
   - Set up modern React project structure

2. **State Management Setup**
   - Installed Redux Toolkit for predictable state management
   - Created type definitions for game models (`Game`, `Player`, `Round`)
   - Prepared for Redux slices implementation

3. **API Client**
   - Created `gameApi.ts` with axios for all backend communication
   - Implemented methods for all game operations (create, join, play, vote, etc.)
   - Configured 35-second timeout to accommodate long polling

4. **Long Polling Implementation**
   - Created `useLongPolling` hook for automatic state synchronization
   - Implements exponential backoff on errors
   - Manages version tracking and update callbacks
   - Handles connection lifecycle and cleanup

5. **OpenTelemetry Browser Instrumentation**
   - Created `telemetry.ts` with Web SDK configuration
   - Auto-instruments fetch and XHR requests
   - Configured Application Insights export
   - Enables end-to-end distributed tracing

### ✅ Infrastructure Updates

1. **Bicep Templates**
   - **Removed:** SignalR Service resource
   - **Removed:** SignalR connection string from Function App settings
   - **Added:** Enhanced Application Insights configuration with OpenTelemetry
   - **Updated:** Function App to use `DOTNET-ISOLATED|8.0` runtime
   - **Updated:** Static Web App output location to `dist` (React/Vite)
   - **Updated:** Function App worker runtime to `dotnet-isolated`

2. **GitHub Actions Pipelines**
   - **Created:** `tag-release.yml` for production deployments on version tags
   - **Updated:** Build steps to include both .NET 8 and Node.js 20
   - **Configured:** Separate build jobs for backend and frontend
   - **Added:** Infrastructure deployment before application deployment
   - **Included:** GitHub Release creation on successful deployment

### ✅ Documentation

1. **GitHub Copilot Instructions**
   - Completely rewrote to reflect new architecture
   - Updated all technology references (React, Redux, Long Polling)
   - Added long polling pattern documentation
   - Included OpenTelemetry guidance
   - Updated build and deployment instructions

## Architecture Comparison

### Before (Blazor + SignalR)

```
[Blazor WebAssembly] 
      ↓ (SignalR)
[Azure SignalR Service] 
      ↓
[Azure Functions (In-Process .NET 6)]
      ↓
[Azure Table Storage]
```

### After (React + Long Polling)

```
[React + TypeScript]
      ↓ (HTTP Long Polling)
[Azure Functions (Isolated .NET 8)]
      ↓
[Azure Table Storage]
```

## Key Benefits

1. **Cost Reduction**
   - Eliminated Azure SignalR Service ($49-490/month)
   - Long polling uses existing Function App infrastructure

2. **Simpler Architecture**
   - Removed complex SignalR connection management
   - Standard HTTP requests instead of WebSocket connections
   - Easier to debug and monitor

3. **Better Performance**
   - .NET 8 performance improvements
   - Isolated worker model for better resource isolation
   - Reduced network overhead

4. **Modern Frontend**
   - React's large ecosystem and community
   - Better tooling with Vite
   - TypeScript for type safety
   - More flexible component model

5. **Enhanced Observability**
   - End-to-end tracing with OpenTelemetry
   - Better insights into user interactions
   - Distributed trace correlation

6. **Production-Ready CI/CD**
   - Tag-based releases for semantic versioning
   - Automated infrastructure deployment
   - Separate environments (dev/prod)

## Files Modified

### Backend
- `CardsAgainstHumanity.Api/CardsAgainstHumanity.Api.csproj` - Updated packages and target framework
- `CardsAgainstHumanity.Api/Program.cs` - New isolated worker configuration
- `CardsAgainstHumanity.Api/FunctionTriggers.cs` - Updated all functions for isolated model
- `CardsAgainstHumanity.Api/Extensions/FunctionX.cs` - New extension methods for isolated model
- `CardsAgainstHumanity.Api/Entities/Game.cs` - Added Version property and increment logic
- `CardsAgainstHumanity.Api/Services/GameStateService.cs` - New service for state change notifications
- `CardsAgainstHumanity.Api/Services/PollingService.cs` - New service for long polling
- `CardsAgainstHumanity.Api/host.json` - Updated configuration
- `CardsAgainstHumanity.Application/Interfaces/IVersionable.cs` - Changed to int Version
- `CardsAgainstHumanity.Application/Interfaces/IGame.cs` - Updated CurrentRound to nullable

### Frontend (New)
- `CardsAgainstHumanity.Web/` - Complete new React application
- `CardsAgainstHumanity.Web/src/api/gameApi.ts` - API client
- `CardsAgainstHumanity.Web/src/hooks/useLongPolling.ts` - Long polling hook
- `CardsAgainstHumanity.Web/src/types/game.ts` - TypeScript definitions
- `CardsAgainstHumanity.Web/src/telemetry/telemetry.ts` - OpenTelemetry setup

### Infrastructure
- `infrastructure/main.bicep` - Removed SignalR, updated for isolated worker
- `.github/workflows/tag-release.yml` - New production deployment pipeline
- `.github/copilot-instructions.md` - Completely rewritten

### Deleted
- `CardsAgainstHumanity.Api/FunctionSignalr.cs`
- `CardsAgainstHumanity.Api/Startup.cs`
- `CardsAgainstHumanity.Api/WebJobStartup.cs`
- `CardsAgainstHumanity.Api/FunctionBase.cs`

## Remaining Work

To complete the migration, the following tasks are still needed:

1. **React UI Components**
   - Port all Blazor components to React
   - Implement game screens (Home, Lobby, Game, Leaderboard)
   - Add card selection and voting UI
   - Style with CSS/Tailwind

2. **Redux Store**
   - Create slices for game state, player state, UI state
   - Implement async thunks for all API calls
   - Add selectors for computed state

3. **Remove Blazor Project**
   - Delete `CardsAgainstHumanity.UI` directory
   - Update solution file
   - Remove from deployment pipelines

4. **Testing**
   - Unit tests for Redux slices
   - Integration tests for API client
   - End-to-end tests for game flow
   - Performance testing of long polling

5. **Documentation**
   - Update README.md with new instructions
   - Add migration guide for contributors
   - Document long polling behavior
   - Add troubleshooting guide

## Testing the Changes

### Local Development

1. **Backend:**
```bash
cd CardsAgainstHumanity.Api
func start
```

2. **Frontend:**
```bash
cd CardsAgainstHumanity.Web
npm install
npm run dev
```

### Deployment

1. **Create a tag:**
```bash
git tag v1.0.0
git push origin v1.0.0
```

2. **Pipeline automatically:**
   - Builds backend and frontend
   - Deploys infrastructure
   - Deploys applications
   - Creates GitHub release

## Migration Strategy Recommendation

1. **Phase 1 (Complete):** Backend + Infrastructure
2. **Phase 2 (Next):** Complete React UI with all components
3. **Phase 3:** Side-by-side testing (keep Blazor temporarily)
4. **Phase 4:** Cut over to React, deprecate Blazor
5. **Phase 5:** Remove Blazor project entirely

## Notes

- ActorTableEntities v2.0.0 exists but has incompatible API, staying with v0.1.0-alpha000
- Long polling timeout is 30 seconds, clients should reconnect immediately
- OpenTelemetry requires environment variable `VITE_APPINSIGHTS_KEY` in frontend
- Backend builds successfully with only nullable reference warnings (expected)

## Author

Migration performed by GitHub Copilot
Date: 2026-01-01
