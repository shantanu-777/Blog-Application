# BlogApp – The Future of Blogging

A modern, AI-powered, full-stack blogging platform built with **ASP.NET Core**, **Blazor WebAssembly + MudBlazor**, and **Supabase**.  
Designed for **multi-role publishing workflows**, **AI-assisted content creation**, and a rich **administrative experience** – with unique capabilities not commonly found in other blog platforms.

---

## Table of Contents

- [Architecture](#architecture)
- [Key Features](#key-features)
  - [Core Features](#core-features)
  - [AI-Powered Features](#ai-powered-features)
  - [Unique Features](#unique-features-not-found-in-other-blog-platforms)
  - [UI/UX & Reader Experience](#uiux--reader-experience)
  - [Security](#security)
- [Administrative Suite](#administrative-suite)
- [Authentication & User Management](#authentication--user-management)
- [Content Management](#content-management)
- [Content Discovery & Reader Experience](#content-discovery--reader-experience)
- [Collaboration & Interaction](#collaboration--interaction)
- [AI Augmentation](#ai-augmentation)
- [Supabase Schema Summary](#supabase-schema-summary)
- [API Documentation](#api-documentation)
- [Getting Started](#getting-started)
- [Environment Configuration](#environment-configuration)
- [Deployment](#deployment)
- [Roadmap / Planned Enhancements](#roadmap--planned-enhancements)
- [Project Structure](#project-structure)
- [Contributing](#contributing)
- [License](#license)
- [Acknowledgments](#acknowledgments)
- [Support](#support)

---

## Architecture

- **Backend**: ASP.NET Core Web API (`BlogApp.Backend`)
  - Routes for posts, comments, users, AI helpers, admin orchestration
  - Supabase REST client for persistence (posts, comments, likes, settings)
  - ML.NET & ONNX Runtime hooks for AI/ML-based features
  - SignalR hubs for real-time notifications and collaboration

- **Frontend**: Blazor WebAssembly (`BlogApp.Frontend`)
  - MudBlazor component library for a modern, Material-inspired UI
  - Blazored.LocalStorage for session tokens and client state
  - SignalR client for real-time updates

- **Shared**: `.NET` shared models & contracts (`BlogApp.Shared`)
  - DTOs, enums, validation contracts shared between frontend and backend

- **Data**: Supabase (Postgres + Auth)
  - Custom schema & SQL seed in `Supabase/schema.sql`
  - Tables for posts, comments, likes, site settings, and more

- **Real-time / Notifications**:
  - SignalR hub scaffolded for future real-time updates (notifications, collaboration)

---

## Key Features

### Core Features

- **User Authentication & Management**
  - Email/password registration & login (Supabase Auth)
  - Email verification & password reset flows
  - Profile management (avatar, bio, social links)

- **Role-based Access Control**
  - Roles: **Admin**, **Editor**, **Author**, **Reader**
  - Metadata-based role checks on both frontend and backend

- **Blog Content Management**
  - Create, edit, publish, schedule, and archive posts
  - Rich authoring experience (Markdown / rich text, SEO fields)
  - Featured images, tags, categories, slugs

- **Real-time Notifications**
  - SignalR-powered infrastructure for live notifications
  - Ready for future UI toast integrations

- **Reading Time & Engagement**
  - Automatic reading time estimation
  - Likes, comments, and trending feeds

---

### AI-Powered Features

- **AI Content Generation**
  - Generate high-quality blog post drafts from prompts
  - Multiple writing styles and tone presets (planned/stubbed)

- **Smart Content Optimization**
  - SEO optimization suggestions
  - Readability and style improvements

- **Image Suggestions**
  - AI-generated or AI-recommended images (via integration layer)

- **Writing Style Enhancement**
  - Adjust tone based on target audience or style
  - Grammar and readability checks (via AI services)

- **Plagiarism Detection**
  - AI-assisted originality checks to keep content unique

- **Reading Time Calculation & Metadata**
  - AI-assisted reading time and excerpt generation
  - Keyword extraction for better tagging and discovery

---

### Unique Features (Not found in other blog platforms)

- **Smart Reading Lists**
  - AI-curated reading lists based on user interests and history

- **Writing Challenges**
  - Gamified writing challenges and competitions to motivate authors

- **Collaboration Spaces**
  - Team-based content creation and shared drafts

- **Reading Insights**
  - Deep analytics around reader behavior and engagement

- **Content Series**
  - Create and manage multi-part series with clear navigation

- **Real-time Collaboration**
  - Live editing, commenting, and feedback (backed by SignalR, progressively enhanced)

These unique features are modeled via dedicated services (e.g., `IUniqueFeaturesService`) and are progressively implemented across iterations.

---

### UI/UX & Reader Experience

- **Medium-Inspired Design**
  - Clean, minimalist, content-first interface
  - Generous whitespace, focus on readability

- **Responsive Design**
  - Fully responsive on desktop, tablet, and mobile

- **Dark Mode Support**
  - Theme toggling between light and dark modes

- **Interactive Elements**
  - Real-time notifications (planned UI integration)
  - Smooth animations and transitions
  - Live content editing experiences

---

### Security

- **Authentication & Authorization**
  - JWT-based authentication
  - Role-based authorization on protected endpoints

- **Best Practices**
  - Input validation and sanitization
  - CORS configuration
  - Secure password handling (via Supabase Auth)
  - Logging with Serilog (console + rolling files)

---

## Administrative Suite

All admin features live under `/admin` and are restricted to **Admin** (or allowlisted emails).

1. **Dashboard Overview**
   - Total posts, drafts, pending approvals, scheduled posts
   - Engagement KPIs: views, likes, comments, engagement rate
   - Pending/flagged comment counts
   - New users this week, active readers
   - Post trends (last 14–30 days), top posts, recent comment flags, newest members

2. **Content Moderation**
   - Filterable queues for posts (by status, search) & comments (flagged, pending)
   - Moderation actions:
     - Publish, send back, add review notes
     - Approve, delete, or retain flagged comments

3. **User Management**
   - Search & filter by role, status
   - Inline role changes and activate/deactivate toggle
   - Tracked metadata: created date, last login, post count, follower counts

4. **Analytics**
   - Date-range-based post analytics (7–90 days)
   - Views trends, top engaged posts, category distribution

5. **Site Settings**
   - Title, description, base URL, branding colors, logo
   - Contact email, analytics IDs (Google Analytics, Facebook Pixel)
   - Feature flags: comments, approvals, newsletter, search
   - All stored centrally in Supabase `site_settings` table

---

## Authentication & User Management

- **Supabase Auth**
  - Registration, login, logout, password reset
  - Email verification handled via Supabase native flows

- **Roles**
  - Reader, Author, Editor, Admin (stored as metadata/claims)
  - Role checks enforced both client-side and server-side

- **Profiles**
  - Avatar, bio, social links
  - Public profile pages

- **Social Graph (Planned / Partial)**
  - Follow/unfollow relationships
  - Follower and following counts
  - Suggested users with similar interests

---

## Content Management

- Rich text / Markdown editor with AI-assisted prompts
- Post lifecycle:
  - **Draft → Pending Review → Published → Archived**
- Support for:
  - Featured images
  - Tags & categories
  - Meta keywords & descriptions
  - Custom slugs (with uniqueness enforcement)
- Scheduling:
  - Schedule future publish dates
- Auto-generated metadata:
  - Reading time calculation
  - Smart excerpt generation
- SEO fields:
  - Customizable title, description, slug, and OG fields (planned)

---

## Content Discovery & Reader Experience

- Homepage feed with pagination
- Trending posts by engagement (views + likes + comments)
- Category and tag pages
- Post detail pages:
  - Reading time
  - Related posts by tags/categories
- Full-text search (via Supabase/Postgres)
- Archive by month/year (planned)
- Recommended posts based on reader interests and tag affinity

---

## Collaboration & Interaction

- **Comments**
  - Nested comments with parent-child relationships
  - Flagging & moderation controls

- **Reactions**
  - Likes/reactions per post
  - Trending ranking based on reaction and engagement signals

- **Social & Email**
  - Social sharing placeholders (Twitter/X, LinkedIn, etc.)
  - Newsletter subscriptions (planned Supabase table)

- **Communication**
  - Contact form submissions with admin notes
  - Real-time notifications via SignalR (queued for future UI enhancements)

---

## AI Augmentation

Backend + frontend services support:

- SEO optimization suggestions
- Reading time & excerpt generation
- Keyword extraction for better categorization
- Related topic & related post recommendations
- AI-generated draft scaffolding
- Writing style and tone adjustment
- Plagiarism detection hooks

Actual AI provider (OpenAI, Azure OpenAI, local ONNX models, etc.) is pluggable via **Custom AI Services**; interfaces are stubbed so you can plug in your provider of choice.

---

## Supabase Schema Summary

`Supabase/schema.sql` defines the key tables:

- **`posts`**
  - Statuses, scheduling fields
  - Arrays for tags & categories
  - AI metadata and review notes
  - Slug, SEO fields, read time, excerpts

- **`comments`**
  - Approval and flag fields
  - Parent-child (nested) relationships

- **`post_likes`**
  - Per-user like/reaction tracking

- **`site_settings`**
  - Singleton/global site configuration

- **Planned Tables**
  - Newsletter subscribers
  - Contact submissions
  - Analytics / event tracking

---

## API Documentation

### Authentication Endpoints

- `POST /api/auth/register` – User registration  
- `POST /api/auth/login` – User login  
- `POST /api/auth/logout` – User logout  
- `POST /api/auth/reset-password` – Password reset  
- `GET  /api/auth/me` – Get current authenticated user  

### Blog Endpoints

- `GET    /api/blog` – Get posts with filtering (status, tags, search, pagination)  
- `POST   /api/blog` – Create new post  
- `GET    /api/blog/{id}` – Get specific post by ID  
- `PUT    /api/blog/{id}` – Update post  
- `DELETE /api/blog/{id}` – Delete post  
- `POST   /api/blog/{id}/like` – Like/unlike a post  
- `GET    /api/blog/trending` – Get trending posts  

### AI Content Endpoints

- `POST /api/aicontent/generate` – Generate blog content from a prompt  
- `POST /api/aicontent/optimize-seo` – SEO optimization suggestions  
- `POST /api/aicontent/suggest-tags` – Tag/keyword suggestions  
- `POST /api/aicontent/detect-plagiarism` – Plagiarism check  

*(Routes and payloads can be extended as the AI provider integration evolves.)*

---

## Getting Started

### 1. Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/)
- Visual Studio 2022 or VS Code
- Supabase account & project
- Optional: Node.js (if you extend build tooling)
- Optional: `dotnet-ef` tool for migrations  
  ```bash
  dotnet tool install --global dotnet-ef


### 2. Clone & Restore

```bash
git clone https://github.com/yourusername/BlogApp.git
cd BlogApp
dotnet restore
```

### 3. Configure Supabase

#### Backend (`BlogApp.Backend/appsettings.json`)

```json
{
  "Supabase": {
    "Url": "YOUR_SUPABASE_URL",
    "AnonKey": "YOUR_SUPABASE_ANON_KEY"
  },
  "Jwt": {
    "Key": "YOUR_JWT_KEY"
  },
  "ConnectionStrings": {
    "DefaultConnection": "YOUR_SUPABASE_DB_CONNECTION_STRING"
  }
}
```

You can also override these via environment variables:

* `SUPABASE_URL`
* `SUPABASE_ANON_KEY`
* `JWT_KEY`
* `CONNECTION_STRING`

#### Frontend (`BlogApp.Frontend/wwwroot/appsettings.json`)

```json
{
  "Supabase": {
    "Url": "YOUR_SUPABASE_URL",
    "AnonKey": "YOUR_SUPABASE_ANON_KEY"
  },
  "Admin": {
    "AllowedEmails": [
      "admin@example.com"
    ]
  }
}
```

### 4. Run Backend

```bash
cd BlogApp.Backend
dotnet run
```

Default dev URL (example): `https://localhost:7000` (or as configured in `launchSettings.json`).

### 5. Run Frontend

```bash
cd BlogApp.Frontend
dotnet watch run
```

Default dev URL (example): `https://localhost:7001` or `http://localhost:5000` depending on `launchSettings.json`.

### 6. Supabase Setup

1. Create a new Supabase project.
2. Run SQL from `Supabase/schema.sql` in the Supabase SQL editor.
3. Configure Storage buckets if you plan to upload images.
4. Add Row Level Security (RLS) policies for:

   * `posts`
   * `comments`
   * `post_likes`
   * `site_settings`
5. Ensure auth rules align with your roles and app expectations.

---

## Environment Configuration

* **Frontend**
  `BlogApp.Frontend/wwwroot/appsettings.json`

  * Supabase client configuration
  * Admin email allowlist (`Admin:AllowedEmails`)

* **Backend**
  `BlogApp.Backend/appsettings.json`

  * Supabase URL and Anon Key
  * Connection strings
  * JWT secret key
  * Serilog logging configuration

* **Launch Settings**
  `Properties/launchSettings.json` in each project defines ports and HTTPS settings.

* **Logging**

  * `Serilog` configured for console and rolling file logs
  * Log files typically under `BlogApp.Backend/logs/`

---

## Deployment

### Docker

The project includes Docker support (optional).

```bash
docker-compose up -d
```

Update your environment variables and appsettings for production before building images.

### Azure Deployment

1. Create an **Azure App Service** (for backend and/or frontend).
2. Configure environment variables and connection strings in Azure.
3. Deploy using:

   * Azure DevOps pipelines, or
   * GitHub Actions, or
   * Direct publish from Visual Studio.

### Manual Deployment

1. Build the solution:

   ```bash
   dotnet build --configuration Release
   ```
2. Publish Backend & Frontend:

   ```bash
   cd BlogApp.Backend
   dotnet publish -c Release -o out

   cd ../BlogApp.Frontend
   dotnet publish -c Release -o out
   ```
3. Deploy the published artifacts to your preferred hosting provider.
4. Ensure `appsettings.Production.json` and environment variables are correctly configured.

---

## Roadmap / Planned Enhancements

* Full archive browsing & reader history
* Newsletter + contact management with Supabase tables
* Comment webhooks / real-time moderation feed
* End-to-end notifications (SignalR + UI toasts)
* Media uploads to Supabase Storage with signed URLs
* Theme customization (from Site Settings, including dark/light presets)
* Automated tests (unit + integration)
* CI/CD pipeline and full containerization
* Accessibility & localization audits
* Advanced AI features:

  * Writing challenges & streaks
  * Reading insights dashboards
  * Smarter recommendations and series analytics

---

## Project Structure

```bash
BlogApp/
├─ BlogApp.sln                    # Solution file
├─ BlogApp.Backend/               # ASP.NET Core API backend
│  ├─ Controllers/                # Blog, User, Admin, AI, Auth controllers
│  ├─ Services/                   # Business logic, Supabase repos, admin/site settings
│  ├─ Hubs/                       # SignalR hub(s) for realtime features
│  ├─ Models/                     # Backend-specific models (if any)
│  └─ Program.cs                  # DI + middleware setup (entry point)
├─ BlogApp.Frontend/              # Blazor WebAssembly frontend
│  ├─ Pages/                      # Views (Index, Post, CreatePost, Admin, Auth pages, etc.)
│  ├─ Services/                   # HttpClient APIs, Auth, Notifications, AI helpers
│  ├─ Shared/                     # Layouts, shared components
│  └─ wwwroot/                    # Static assets & appsettings
├─ BlogApp.Shared/                # Shared DTOs, enums, contracts
│  └─ Models/                     # Shared data models
├─ Supabase/
│  └─ schema.sql                  # Database schema and seed
├─ docs/                          # Additional documentation
│  ├─ oauth-setup.md              # OAuth setup (planned)
│  ├─ login-troubleshooting.md    # Auth troubleshooting (planned)
│  └─ project-summary.md          # High-level summary (planned)
└─ README.md                      # This file
```

---

## Contributing

1. **Fork** the repository.
2. **Create a feature branch**:

   ```bash
   git checkout -b feature/your-feature-name
   ```
3. **Implement your changes** (code + tests where applicable).
4. **Run checks**:

   ```bash
   dotnet build
   # optionally
   dotnet format
   ```
5. **Document any schema or API changes**:

   * Update `README.md`
   * Update `Supabase/schema.sql` (if DB changes)
6. Open a **Pull Request**:

   * Include screenshots for UI changes (especially admin pages).
   * Describe the changes clearly.

---

## License

This project is licensed under the **MIT License** (or your preferred license).
Update this section and add a `LICENSE` file as needed.

---

## Acknowledgments

* **Supabase** – Authentication and database services
* **MudBlazor** – UI component library for Blazor
* **ML.NET** – Machine learning capabilities
* **ONNX Runtime** – Model inference support
* The **open-source community** for inspiration, tools, and best practices


**BlogApp** – Revolutionizing the way we create and consume content online.

```
```

