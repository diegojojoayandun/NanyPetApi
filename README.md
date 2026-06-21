# NanyPet RestAPI

![alt text](https://i.ibb.co/yST3rtq/nanypet-logo-removebg-preview.png)

![alt text](https://i.ibb.co/s9FwQkt/Screenshot-1.png)

![alt text](https://i.ibb.co/gyqQctq/Screenshot-3.png)

This project is meant for educational purposes only.

NanyPet is a REST API for a pet-sitting platform that connects pet owners with verified caretakers (herders). It implements a 3-tier architecture (API / Business Logic / Data Access), Ardalis Endpoints pattern, Repository pattern, Dependency Injection, AutoMapper, response caching, and pagination.

There are many pet-friendly places, however, some good places still do not allow pets.
NanyPet gives owners the opportunity to find nearby pet lovers who will gladly take care of their pet while they enjoy a restaurant, movies, shopping, or any other activity.
NanyPet locates a sitter in your area who, for an agreed price, will take care of your pet with responsibility and love — with full identity verification for both owners and caretakers.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Language | C# / ASP.NET Core 8 |
| ORM | Entity Framework Core 8 |
| Database | SQL Server or SQLite (configurable via `DB_PROVIDER`) |
| Auth | ASP.NET Identity + JWT Bearer + Google OAuth |
| Real-time | SignalR (chat per appointment) |
| Push notifications | Firebase Cloud Messaging (FCM) |
| File storage | Azure Blob Storage |
| Payments | Wompi (Colombian gateway) with webhook support |
| Geolocation | Haversine formula for proximity search |
| Frontend | React + TypeScript + Vite (in `/Frontend`) |
| Docs | Swagger / OpenAPI (Swashbuckle with annotations) |

---

## Features

- **User roles**: Owner, Herder, Admin (auto-seeded on startup)
- **JWT + Google OAuth** authentication
- **Herder verification flow**: upload photo, ID (front/back), and selfie with ID to Azure Blob Storage; Admin can approve or reject
- **Geo search**: find verified herders within a configurable radius using a bounding-box SQL pre-filter + Haversine in-memory exact calculation
- **Appointments**: full lifecycle with status transitions, service type, notes, and special requirements
- **Payments via Wompi**: generates a hosted checkout URL and processes status updates via webhook
- **Bidirectional reviews**: owners rate herders and herders rate owners, one review per appointment per direction
- **Real-time chat**: SignalR hub scoped to each appointment, with read receipts
- **Push notifications**: Firebase FCM with in-database notification log; FCM token registration endpoint
- **Pagination**: all list endpoints support `pageNumber` / `pageSize`
- **Response caching** configured globally

---

## Architecture

```
NanyPetApi/
├── NanyPetAPI/              # Presentation layer — Ardalis Endpoints, Hubs, Program.cs
│   ├── Endpoints/
│   │   ├── Admin/           # Herder verification approval
│   │   ├── Appointments/    # CRUD + status transitions
│   │   ├── Herders/         # CRUD + nearby search
│   │   ├── Messages/        # List per appointment
│   │   ├── Owners/          # CRUD
│   │   ├── Payments/        # Create + Wompi webhook
│   │   ├── Pets/            # CRUD
│   │   ├── Reviews/         # Create + list herder reviews
│   │   ├── Users/           # Signup, signin, FCM token
│   │   └── Verification/    # Upload identity documents
│   └── Hubs/
│       └── ChatHub.cs       # SignalR real-time chat
├── BusinessLogicLayer/      # Services: User, Owner, Blob, Geo, Notification, Payment
├── DataAccessLayer/         # Entities, DTOs, Repositories, Migrations, DbContext
└── Frontend/                # React + TypeScript + Vite client
```

---

## Data Model

| Entity | Key fields |
|---|---|
| `User` | ASP.NET Identity + `FirstName`, `LastName`, `FcmToken` |
| `Owner` | Email, phone, address, geolocation |
| `Herder` | Email, geolocation (lat/lon/radius), hourly rate, availability, verification status, average rating |
| `Pet` | Name, species, breed, age, gender, owner reference |
| `Appointment` | Pet, herder, status, price, service type, notes, payment reference |
| `Payment` | Amount (COP), Wompi transaction ID, reference, checkout URL, status |
| `Review` | Bidirectional (owner↔herder), rating per appointment |
| `Message` | Per-appointment chat, sender, read status |
| `Notification` | Per-user, type, related entity, read flag |

---

## API Endpoints Summary

| Method | Route | Description |
|---|---|---|
| POST | `/api/user/signup` | Register a new user |
| POST | `/api/user/signin` | Login and get JWT |
| POST | `/api/user/fcm-token` | Register FCM push token |
| GET | `/api/herder` | List all herders (paginated) |
| GET | `/api/herder/{id}` | Get herder by ID |
| GET | `/api/herder/nearby` | Find verified herders by geolocation |
| POST | `/api/herder` | Create herder profile |
| PUT | `/api/herder/{id}` | Update herder profile |
| DELETE | `/api/herder/{id}` | Delete herder |
| POST | `/api/herder/{id}/verification` | Upload verification documents |
| GET | `/api/owner` | List all owners (paginated) |
| GET | `/api/owner/{id}` | Get owner by ID |
| POST | `/api/owner` | Create owner profile |
| DELETE | `/api/owner/{id}` | Delete owner |
| GET | `/api/pet` | List all pets (paginated) |
| GET | `/api/pet/{id}` | Get pet by ID |
| POST | `/api/pet` | Create pet |
| PUT | `/api/pet/{id}` | Update pet |
| DELETE | `/api/pet/{id}` | Delete pet |
| GET | `/api/appointment` | List appointments (paginated) |
| GET | `/api/appointment/{id}` | Get appointment by ID |
| POST | `/api/appointment` | Create appointment |
| PATCH | `/api/appointment/{id}/status` | Update appointment status |
| POST | `/api/payment` | Create Wompi payment + checkout URL |
| POST | `/api/payment/webhook` | Wompi webhook handler |
| GET | `/api/review/herder/{herderId}` | List reviews for a herder |
| POST | `/api/review` | Create review |
| GET | `/api/message/{appointmentId}` | List chat messages |
| POST | `/api/admin/verification/{herderId}` | Approve or reject herder |
| WS | `/hubs/chat` | SignalR chat hub |

---

## Requirements

- .NET SDK 8.0
- SQL Server or SQLite
- (Optional) Azure Storage account — for document/photo uploads
- (Optional) Firebase project — for push notifications
- (Optional) Wompi account — for payment processing
- (Optional) Google Cloud credentials — for Google OAuth

---

## Environment Variables

Create a `.env` file at the project root (loaded via `DotEnv.Core`):

```env
# Database
CONNECTION_STRING=your_connection_string
DB_PROVIDER=sqlserver   # or "sqlite"

# Auth
SECRET_KEY=your_jwt_secret

# Google OAuth (optional)
CLIENT_ID=your_google_client_id
CLIENT_SECRET=your_google_client_secret

# Admin seed (optional)
ADMIN_EMAIL=admin@nanypet.com
ADMIN_PASSWORD=Admin@123

# Azure Blob Storage (optional)
AZURE_BLOB_CONNECTION_STRING=your_azure_connection_string

# Firebase (optional)
FIREBASE_SERVICE_ACCOUNT_JSON={"type":"service_account",...}

# Wompi payments (optional)
WOMPI_PUBLIC_KEY=pub_stagtest_...

# Frontend CORS origin
FRONTEND_URL=http://localhost:5173

# Server port
PORT=5000
```

---

## Running Locally

```bash
# Clone the repo
git clone https://github.com/diegojojoayandun/NanyPetApi.git
cd NanyPetApi

# Set up environment variables (see above)
cp .env.example .env

# Apply migrations
dotnet ef database update --project DataAccessLayer --startup-project NanyPetAPI

# Run the API
dotnet run --project NanyPetAPI

# Swagger UI available at
# http://localhost:5000/swagger
```

---

## NuGet Packages

**NanyPetAPI**
- `Ardalis.ApiEndpoints` 4.1.0
- `AutoMapper` 12.0.1
- `AutoMapper.Extensions.Microsoft.DependencyInjection` 12.0.1
- `Azure.Storage.Blobs` 12.29.0
- `DotEnv.Core` 3.0.0
- `Microsoft.AspNetCore.Authentication.Google` 8.0.11
- `Microsoft.AspNetCore.Authentication.JwtBearer` 8.0.11
- `Microsoft.AspNetCore.OpenApi` 8.0.11
- `Swashbuckle.AspNetCore` 6.4.0
- `Swashbuckle.AspNetCore.Annotations` 6.5.0

**BusinessLogicLayer**
- `Azure.Storage.Blobs` 12.29.0
- `FirebaseAdmin` 3.5.0

**DataAccessLayer**
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` 8.0.11
- `Microsoft.EntityFrameworkCore.SqlServer` 8.0.11
- `Microsoft.EntityFrameworkCore.Sqlite` 8.0.11
- `Microsoft.EntityFrameworkCore.Tools` 8.0.11
- `Microsoft.IdentityModel.Tokens` 7.1.2

---

## Author

[Diego Fernando Jojoa Yandún](https://github.com/diegojojoayandun)
