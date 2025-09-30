# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

HaWeb is a digital edition website for the Hamann correspondence (Hamann-Ausgabe), built with ASP.NET Core 6 and modern frontend tooling. It renders scholarly editions of historical letters and commentary from XML sources, with features for search, indexing, and editorial workflow management.

The project depends on two sibling projects:
- **HaDocumentV6**: Parses XML files into convenient C# models
- **HaXMLReaderV6**: Forward-parses XML elements (letters, comments, traditions, marginals) into HTML

## Build and Development Commands

### Initial Setup
```bash
npm install                    # Install frontend dependencies
dotnet restore                 # Restore .NET packages
```

### Development (run both in separate terminals)
```bash
npm run dev                    # Watch and rebuild CSS/JS (uses Vite)
dotnet watch run               # Watch and rebuild ASP.NET app
```

Set `DOTNET_ENVIRONMENT=Development` for development features. On Windows PowerShell: `$Env:ASPNETCORE_ENVIRONMENT = "Development"`

### Production Build
```bash
npm run build                  # Build CSS/JS first (required!)
dotnet build HaWeb.csproj      # Build the web application
```

### Production Deployment (Linux)
```bash
npm run build
dotnet publish --runtime linux-x64 -c Release
```

## Architecture

### Frontend Build System
- **Vite**: Bundles JavaScript modules from `wwwroot/js/main.js` → `wwwroot/dist/scripts.js`
- **PostCSS + Tailwind**: Processes CSS from `wwwroot/css/*.css` → `wwwroot/dist/styles.css`
- **Build order matters**: Always run `npm run build` before `dotnet build` for production

PostCSS plugin order in `postcss.config.cjs` is critical: `tailwindcss` → `postcss-import` → `autoprefixer`

### Backend Architecture (ASP.NET Core MVC)

**Core Services** (registered as singletons in Program.cs):
- `XMLTestService`: XML validation and testing
- `XMLInteractionService`: Central XML interaction layer
- `HaDocumentWrapper`: Wraps HaDocumentV6 models
- `GitService`: Git repository management with LibGit2Sharp
- `XMLFileProvider`: File system abstraction for XML files
- `WebSocketMiddleware`: Real-time notifications for XML changes

**Controllers** (main routes):
- `BriefeContoller`: Letter display and navigation
- `RegisterController`: Index/register views (persons, places, etc.)
- `SucheController`: Search functionality
- `EditionController`: Edition-specific views
- `AdminController`: Administrative functions (feature-gated)
- `XMLStateController`: XML state management for editors
- `WebhookController`: Git webhook endpoint for automated pulls
- `CMIF`: CMIF (Correspondence Metadata Interchange Format) export

**Key Architectural Components**:
- `HTMLParser/`: Custom XML→HTML transformation rules and state machines
- `FileHelpers/`: XML file reading and management
- `SearchHelpers/`: Search implementation with state pattern
- `Settings/`: Configuration for XML node parsing, collections, and rules

### Feature Flags
Managed via `Microsoft.FeatureManagement.AspNetCore` in `appsettings.json`:
- `AdminService`: Enables admin routes
- `LocalPublishService`: Local publishing features
- `SyntaxCheck`: XML syntax validation
- `Notifications`: WebSocket notifications

### Configuration
Environment-specific settings in `appsettings.{Environment}.json`. Key settings:
- `FileStoragePath`: Base directory for all data storage (absolute path)
  - Compiled Hamann.xml files stored in `[FileStoragePath]/HAMANN/`
  - Git repository content in `[FileStoragePath]/GIT/`
- `RepositoryBranch`: Git branch to track (e.g., "main")
- `RepositoryURL`: Git repository URL (e.g., "https://github.com/user/repo")
- `WebhookSecret`: Optional HMAC-SHA256 secret for webhook validation (GitHub format)

An external `settings.json` can be loaded from `[FileStoragePath]/GIT/settings.json` for runtime configuration overrides.

### Data Flow
1. Root XML file (`Hamann.xml`) must exist at build output root
2. `HaDocumentV6` parses XML into document models
3. `HaXMLReaderV6` transforms XML elements to HTML
4. Controllers fetch models via services and render Razor views
5. Frontend JavaScript adds interactivity (marginals, search highlighting, themes)

### Frontend JavaScript Modules
Modular ES6 structure in `wwwroot/js/`:
- `main.js`: Entry point, exports startup functions
- `marginals.mjs`: Marginal notes display
- `search.mjs`: Search highlighting with mark.js
- `theme.mjs`: Theme switching (light/dark)
- `websocket.mjs`: WebSocket connection for live updates
- `filelistform.mjs`: XML state management forms
- `htmx.min.js`: HTMX for dynamic interactions

Each module exports a `startup_*` function called during initialization.

## Important Development Notes

### XML Parsing System
The project uses a custom forward-parsing system for XML transformation defined in:
- `Settings/NodeRules/`: Rules for specific XML node types (letters, comments, marginals)
- `Settings/ParsingRules/`: Parsing rules for text, links, edits, comments
- `Settings/ParsingState/`: State machines for parsing contexts
- `HTMLParser/`: Core XML parsing helpers

When modifying XML handling, update both the node rules and corresponding parsing state.

### Working with Views
Views follow standard Razor conventions. Shared layouts in `Views/Shared/`. The project uses tag helpers defined in `HTMLHelpers/TagHelpers.cs` for custom rendering.

### CSS Architecture
- `tailwind.css`: Main Tailwind input
- Component-specific CSS files (`letter.css`, `register.css`, `search.css`, etc.)
- `site.css`: Imported by `main.js`, serves as CSS entry point
- Production builds include autoprefixer and cssnano minification

### WebSocket Notifications
Real-time updates for XML changes via WebSocket. Configured with:
- Keep-alive: 30 minutes
- Connection filtering via `AllowedWebSocketConnections` in appsettings
- Middleware registered before static files in Program.cs

### Git Integration
The application uses **LibGit2Sharp** for Git operations on XML source files:

**GitService** (`FileHelpers/GitService.cs`):
- Manages Git repository at `[FileStoragePath]/GIT/`
- Pulls latest changes from remote on webhook trigger
- Retrieves current commit SHA, branch, and timestamp
- Supports default credentials (SSH agent, credential manager)
- Auto-initializes or clones repository if needed

**Webhook Endpoint** (`/api/webhook/git`):
- POST endpoint for Git webhooks (GitHub, GitLab, etc.)
- Optional signature validation using `WebhookSecret` (HMAC-SHA256)
- Triggers `git pull` operation
- Automatically scans and parses XML files after pull
- Returns status with commit info

**Status Endpoint** (`/api/webhook/status`):
- GET endpoint to check current Git state
- Returns commit SHA, branch, timestamp

**Workflow**:
1. Webhook triggered by push to repository
2. `GitService.Pull()` fetches and merges latest changes
3. `XMLFileProvider.Scan()` detects changed files
4. XML files are collected, validated, and parsed
5. New compiled `Hamann.xml` is generated
6. Website reflects updated content

**Configuration**: Set `WebhookSecret` in appsettings to enable signature validation for webhooks.