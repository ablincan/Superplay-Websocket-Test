# Angular + .NET WebSocket Application

This is a full-stack project using an **Angular frontend** and a **.NET backend**, communicating via **WebSockets**. The backend uses **SQLite** as the database, which is automatically created on application startup.

---

## A quick note before we begin 😊

> I know the original idea was to use a **.NET console application** for the client, and I completely understand that direction.  
> I ended up using **Angular** instead — mostly because it made handling WebSocket interactions and building a more visual UI a bit smoother for me in this case.  
>  
> I hope that’s alright — and I hope you don't mind 🫣

## Technologies Used

- Angular (frontend)
- ASP.NET Core (.NET 8) (backend)
- WebSockets for real-time communication
- SQLite (auto-generated DB)
- Docker + Docker Compose

---

## Running the App with Docker

#### _Ensure you have Docker installed._

* Access the root folder and run:
   ```sh
   docker compose build
   ```
   ```sh
   docker compose up -d
   ```

## Running the App Locally

#### _Angular_
1. Access the root angular folder:
   ```sh
   cd Game.Client
   ```
2. Install npm:
   ```sh
   npm install
   ```
3. Start server locally:
   ```sh
   npm start
   ```

#### _.NET_
1. Access the root .net folder:
   ```sh
   cd Game.Server
   ```
2. Start the backend server:
   ```sh
   dotnet run --launch-profile "https"
   ```
