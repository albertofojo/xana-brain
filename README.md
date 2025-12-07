# XanaBrain

**XanaBrain** is the robust backend system powering **Pensa**, a biofeedback and instrument management application. Built with **.NET 8** and adhering to **Clean Architecture** principles, it serves as the "Slow Loop" of the system—handling persistence, heavy analysis, inventory management, and user identity.

## 🏗 Architecture

The solution follows strict **Onion Architecture** (Clean Architecture) rules, ensuring dependencies flow inwards only.

*   **Xana.Domain**: The core. Contains Enterprise Logic, Entities (`User`, etc.), Enums, and Constants. No dependencies on other layers.
*   **Xana.Application**: Application Business Logic. Contains Interfaces, DTOs, and Commands/Queries (CQRS pattern ready). Depends on *Domain*.
*   **Xana.Infrastructure**: Implementation logic. Database access (EF Core), External Services (Firebase), Middleware. Depends on *Application* and *Domain*.
*   **Xana.API**: The entry point. RESTful endpoints, DI configuration, and Hosting. Depends on all layers.

## 🚀 Tech Stack

*   **Framework**: .NET 8 (C#)
*   **Database**: PostgreSQL 16
*   **ORM**: Entity Framework Core 8
*   **Authentication**: Firebase Admin SDK (JWT Validation)
*   **Containerization**: Docker & Docker Compose (Chiseled Ubuntu images for production)
*   **Testing**: xUnit, FluentAssertions, Moq

## 🛠 Getting Started

### Prerequisites

*   [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
*   [Docker Desktop](https://www.docker.com/products/docker-desktop)
*   PostgreSQL (or use the Docker container provided)
*   Firebase Service Account (for authentication)

### Configuration

1.  **Database**: The `docker-compose.yml` is pre-configured for local development.
2.  **Firebase**: Place your Firebase service account JSON in the root if acting as Admin, or rely on client-side tokens for standard API usage.
    *   *Note*: The current `FirebaseUserMiddleware` validates bearer tokens from the client.

### 🐳 Running with Docker (Recommended)

To spin up both the API and the PostgreSQL database:

```bash
docker-compose up --build
```

*   **API**: `http://localhost:8080`
*   **Swagger UI**: `http://localhost:8080/swagger`
*   **PostgreSQL**: Port `5432`

### 💻 Running Locally (Manual)

1.  Start the database (e.g., via Docker):
    ```bash
    docker-compose up -d xana-db
    ```
2.  Run the API:
    ```bash
    dotnet run --project Xana.API
    ```
    *   *Note*: Ensure your `appsettings.Development.json` points to the running local database.

### 🧪 Testing

Execute the unit test suite:

```bash
dotnet test
```

## 🗺 Roadmap

- [x] **Phase 1: Foundation**: Identity, Auth, Architecture, Basic Tooling.
- [ ] **Phase 2: Inventory**: Instrument entities, searching, and filtering.
- [ ] **Phase 3: Maintenance**: Session logging and usage analysis ("The Brain").
- [ ] **Phase 4: Cleanup**: Migration of legacy Flutter logic.

---
*Built with ❤️ for Pensa*
