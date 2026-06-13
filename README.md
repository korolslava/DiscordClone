<div align="center">

# DiscordClone

![.NET](https://img.shields.io/badge/.NET-10.0-blueviolet)
![C#](https://img.shields.io/badge/C%23-13.0-239120)
![EF Core](https://img.shields.io/badge/ORM-EF%20Core-blue)
![SignalR](https://img.shields.io/badge/Realtime-SignalR-orange)
![PostgreSQL](https://img.shields.io/badge/DB-PostgreSQL-336791)
![Redis](https://img.shields.io/badge/Cache-Redis-DC382D)
![Railway](https://img.shields.io/badge/Deploy-Railway-black)
![JWT](https://img.shields.io/badge/Auth-JWT-yellow)
![xUnit](https://img.shields.io/badge/Tests-xUnit-brightgreen)

**Full-stack real-time chat application** — servers, channels, direct messages, friend system, file uploads, and live presence powered by SignalR.  
Inspired by Discord. Built with Clean Architecture on ASP.NET Core 10.

[**Live Swagger →**](https://discordclone1.up.railway.app/swagger)

</div>

---

## Stack

**ASP.NET Core 10** · **Entity Framework Core** (PostgreSQL) · **SignalR** (real-time) · **Redis** (presence & caching) · **MediatR** (CQRS) · **FluentValidation** · **BCrypt** · **Serilog** · **Docker** · **Railway**

---

## Architecture

Clean Architecture with strict layer separation. Domain has zero external dependencies.

```
DiscordClone.API            → Controllers, Middleware, SignalR Hub mapping, Swagger
DiscordClone.Application    → CQRS Commands/Queries (MediatR), FluentValidation pipeline, Interfaces
DiscordClone.Infrastructure → AppDbContext (EF Core), Redis, JWT, SignalR Hub, Storage, Seeders
DiscordClone.Domain         → Entities, Enums, Domain Exceptions — no external deps
DiscordClone.Shared         → DTOs, Request/Response contracts, Realtime event constants
```

**Key decisions:**

* **CQRS via MediatR** — Commands and Queries fully separated, `ValidationBehavior` and `LoggingBehavior` in pipeline
* **Domain-driven entities** — private setters, static factory methods (`User.Create`, `Server.Create`), domain exceptions
* **Real-time via SignalR** — `ChatHub` groups connections by server and channel, broadcasts typed events
* **Presence via Redis Sets** — online users tracked per connection, `SetUserOnlineAsync` / `SetUserOfflineAsync`
* **JWT + Refresh Token Rotation** — access token (15 min) + refresh token (30 days) stored in DB, revoked on logout
* **Soft delete for messages** — content replaced, record preserved for reply chain integrity
* **EF Fluent API** — all constraints, indexes, cascade rules configured explicitly in `IEntityTypeConfiguration<T>`

---

## Features

|                      |                                                                                                      |
| -------------------- | ---------------------------------------------------------------------------------------------------- |
| **Auth**             | Register, Login, Refresh Token rotation, Logout, Get current user                                   |
| **Servers**          | Create, update, delete servers; join by invite code; leave server                                    |
| **Channels**         | Text, Voice, Announcement channels; per-server permission checks                                     |
| **Real-time Chat**   | Send, edit, soft-delete messages; reply threads; SignalR broadcast to channel groups                 |
| **Reactions**        | Add/remove emoji reactions with aggregation and current-user flag                                    |
| **Friends**          | Send/accept/decline friend requests; remove friends; realtime notifications                          |
| **Direct Messages**  | Auto-created DM channel on friend accept; message history with pagination                            |
| **Presence**         | Online/offline status via Redis; per-server member list with live status                             |
| **File Uploads**     | Avatar upload (8MB), server icon upload, message attachments (25MB)                                  |
| **Permissions**      | `ServerPermission` flags enum — `ReadMessages`, `SendMessages`, `Administrator`, etc.                |
| **Pagination**       | Cursor-based message history (`PagedResponse<T>`) — configurable page size up to 100                |
| **Health Checks**    | `/health/live` (app) and `/health/ready` (PostgreSQL + Redis)                                        |

---

## Database

PostgreSQL via **EF Core Fluent API**. All configurations in `IEntityTypeConfiguration<T>`:

* Unique indexes: `Username`, `Email`, `InviteCode`, `(UserId, ServerId)`, `(SenderId, ReceiverId)`
* Composite indexes: `(ChannelId, CreatedAt)`, `(DirectMessageId, CreatedAt)` for pagination performance
* `DeleteBehavior.Cascade` across Server → Members, Channels, Roles; Message → Attachments, Reactions
* Enums stored as `string` for readability (`UserStatus`, `ChannelType`, `FriendRequestStatus`)
* `ServerPermission` stored as `long` (bitfield flags)

---

## Git Strategy

**Git Flow** with branch protection on `main` and `develop`:

```
main
  └── develop
        ├── feature/auth
        ├── feature/servers
        ├── feature/channels
        ├── feature/messages
        ├── feature/friends
        ├── feature/media
        └── feature/infrastructure-layer
```

**Conventional Commits:**
```
feat(auth): add JWT refresh token rotation
fix(docker): remove addgroup/adduser incompatible with aspnet:10.0
chore(init): initialize solution with Clean Architecture projects
```

---

## API

```
POST   /api/auth/register                              — register new user
POST   /api/auth/login                                 — login, receive JWT + refresh token
POST   /api/auth/refresh                               — rotate refresh token
GET    /api/auth/me                                    — get current user [Authorize]
POST   /api/auth/logout                                — revoke refresh token [Authorize]

GET    /api/servers                                    — get my servers [Authorize]
POST   /api/servers                                    — create server [Authorize]
GET    /api/servers/{id}                               — server detail with channels & members
PUT    /api/servers/{id}                               — update server (owner only)
DELETE /api/servers/{id}                               — delete server (owner only)
POST   /api/servers/join/{inviteCode}                  — join server by invite
POST   /api/servers/{id}/leave                         — leave server

POST   /api/servers/{id}/channels                      — create channel (owner only)
PUT    /api/servers/{id}/channels/{channelId}          — update channel
DELETE /api/servers/{id}/channels/{channelId}          — delete channel

GET    /api/channels/{id}/messages                     — paginated message history
POST   /api/channels/{id}/messages                     — send message
PUT    /api/messages/{id}                              — edit message (author only)
DELETE /api/messages/{id}                              — soft delete (author or owner)
POST   /api/messages/{id}/reactions/{emoji}            — add reaction
DELETE /api/messages/{id}/reactions/{emoji}            — remove reaction

GET    /api/friends                                    — get friends with online status
GET    /api/friends/requests/pending                   — incoming friend requests
POST   /api/friends/requests/{username}                — send friend request
PUT    /api/friends/requests/{id}?accept=true          — accept/decline request
DELETE /api/friends/{friendUserId}                     — remove friend

POST   /api/media/avatar                               — upload avatar (8MB, image only)
POST   /api/media/servers/{id}/icon                    — upload server icon (owner only)
POST   /api/media/messages/{id}/attachments            — upload message attachment (25MB)

GET    /health/live                                    — liveness check
GET    /health/ready                                   — readiness check (DB + Redis)
```

---

## SignalR

```
wss://.../hubs/chat?access_token={jwt}
```

Client methods:
- `JoinChannel(channelId)` — subscribe to channel events
- `LeaveChannel(channelId)` — unsubscribe
- `SendTyping(channelId)` — broadcast typing indicator

Server events:
- `MessageReceived` · `MessageEdited` · `MessageDeleted`
- `ReactionAdded` · `ReactionRemoved`
- `UserOnline` · `UserOffline` · `UserTyping`
- `MemberJoined` · `MemberLeft`
- `FriendRequestReceived` · `FriendRequestAccepted`

---

## Run locally

```bash
# Start PostgreSQL and Redis
docker-compose up db redis -d

# Apply migrations
dotnet ef database update \
  --project DiscordClone.Infrastructure \
  --startup-project DiscordClone.API

# Run API
cd DiscordClone.API
dotnet run

# Swagger: http://localhost:5000/swagger
```

Create `.env` from `.env.example` and fill in your values before running.

---

## Tests

```bash
dotnet test DiscordClone.UnitTests
dotnet test DiscordClone.IntegrationTests
```
