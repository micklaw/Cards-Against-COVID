# Cards Against COVID - React Web Frontend

This is the React + TypeScript frontend for Cards Against COVID, built with Vite and Redux Toolkit.

## Local Development

### Quick Start with Azure Static Web Apps CLI

The easiest way to run the app locally is using the Azure Static Web Apps CLI, which handles routing between the frontend and backend automatically:

```bash
# Install dependencies
npm install

# Start the app with SWA CLI (starts both frontend and backend)
npm start
```

The app will be available at `http://localhost:4280` with the API automatically routed through `/api/*`.

### Development with Vite Dev Server

If you prefer to use the Vite dev server directly:

```bash
# Install dependencies
npm install

# Start the dev server
npm run dev
```

The app will be available at `http://localhost:5173`. Make sure the Azure Functions backend is running separately on port 7071.

### Building for Production

```bash
npm run build
```

This will:
1. Type-check with TypeScript
2. Build the app with Vite
3. Automatically copy all files from `public/` (including `staticwebapp.config.json`) to the `dist` folder

The output will be in the `dist` folder.

## Configuration Files

### `public/staticwebapp.config.json`

This file configures Azure Static Web Apps routing in production and is automatically copied to the build output:
- Routes all `/api/*` requests to the Azure Functions backend
- Allows anonymous access to API endpoints
- Sets up CORS headers
- Handles SPA routing with fallback to `index.html`

### `swa-cli.config.json`

This file configures the Azure Static Web Apps CLI for local development:
- Specifies the frontend dev server URL (Vite on port 5173)
- Specifies the backend API URL (Azure Functions on port 7071)
- Configures the SWA CLI proxy to run on port 4280
- Automatically starts the Vite dev server when running `npm start`

## Environment Variables

- `VITE_API_URL`: Override the default API URL (default: `http://localhost:7071/api`)

## Scripts

- `npm run dev`: Start Vite dev server
- `npm run build`: Build for production
- `npm run lint`: Run ESLint
- `npm run preview`: Preview production build
- `npm start`: Start with Azure Static Web Apps CLI (recommended)
- `npm run start:swa`: Start SWA CLI with pre-built app

## Technology Stack

- **React 19**: UI library
- **TypeScript**: Type safety
- **Vite 7**: Fast build tool and dev server
- **Redux Toolkit**: State management
- **Axios**: HTTP client
- **React Router**: Client-side routing
- **OpenTelemetry**: Observability and tracing

---

## Original Vite + React Template Information

This template provides a minimal setup to get React working in Vite with HMR and some ESLint rules.

Currently, two official plugins are available:

- [@vitejs/plugin-react](https://github.com/vitejs/vite-plugin-react/blob/main/packages/plugin-react) uses [Babel](https://babeljs.io/) (or [oxc](https://oxc.rs) when used in [rolldown-vite](https://vite.dev/guide/rolldown)) for Fast Refresh
- [@vitejs/plugin-react-swc](https://github.com/vitejs/vite-plugin-react/blob/main/packages/plugin-react-swc) uses [SWC](https://swc.rs/) for Fast Refresh
