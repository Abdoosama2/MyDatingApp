# MyDatingApp

A full-stack dating application built with ASP.NET Core Web API and Angular. The backend supports member profiles, likes, private messaging, photo uploads, admin role management, and real-time presence/chat via SignalR. The Angular client is currently in early development.

## Project Overview

MyDatingApp enables users to register, browse other members, like profiles, exchange messages, upload photos, and see who is online. Administrators can manage user roles. The API follows a layered architecture with clear separation between domain logic, application services, infrastructure, and HTTP/SignalR endpoints.

## Key Features

- User registration and login with JWT + refresh tokens
- Member discovery with pagination, gender filter, and age range
- Profile editing and last-active tracking
- Photo upload, main photo selection, and deletion (Cloudinary)
- Like/unlike members and view liked/liked-by/mutual lists
- Private messaging (REST + SignalR) with inbox/outbox/unread views
- Real-time online presence tracking
- Admin user/role management
- Database seeding with demo users

## Technology Stack

| Layer | Technologies |
|-------|--------------|
| Backend | ASP.NET Core Web API (.NET 10), EF Core, SQLite |
| Auth | ASP.NET Core Identity, JWT Bearer |
| Real-time | SignalR |
| Mapping | AutoMapper |
| Media | Cloudinary |
| Frontend | Angular 21, RxJS, TypeScript |
| Testing | Vitest (Angular) |

## Architecture Overview

**Patterns:** Layered architecture, Repository, Unit of Work, Service Layer, DTO mapping.

## Backend Features

- REST API under `/api/[controller]`
- SignalR hubs at `/hubs/presence` and `/hubs/messages`
- Automatic EF Core migrations on startup
- Seed data from `Infrastructer/Data/seedData.json`
- Global exception handling middleware
- CORS enabled for Angular dev server (`localhost:4200`)

## Frontend Features

- Angular 21 standalone application scaffold
- Root component with basic HTTP integration (in progress)
- Routing configured but no feature routes yet
- SignalR client integration not yet implemented

## Real-Time Features (SignalR)

### Presence Hub (`/hubs/presence`)
- Tracks connected users in memory
- Events: `UserIsOnline`, `userIsOffline`, `GetOnlineUsers`

### Message Hub (`/hubs/messages`)
- Joins user pairs into deterministic group names
- Loads message thread on connect
- Sends live messages via `SendMessage`
- Marks messages read when recipient is in the chat group
- Notifies online recipients via PresenceHub when not in active chat

**Authentication:** Pass JWT as query parameter: `?access_token={token}`

## Installation & Setup

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (for Angular client)
- npm 11+ (project uses npm 11.12.1)

### Clone the repository

```bash
git clone <repository-url>
cd MyDatingApp/Api
