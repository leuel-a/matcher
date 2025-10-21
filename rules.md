# Matcher Project - Getting Started Guide

## Step-by-Step Starting Plan

Start from the bottom (infrastructure) and move up, ensuring each layer works before starting the next.

---

### Phase 1: Foundation & Infrastructure
**Goal:** Set up the skeleton of your project and the core messaging system.

1.  **Create the Solution and Project Structure:**
    - Create a new root directory for the project (e.g., `Matcher`).
    - Inside, create a `.sln` file (e.g., `Matcher.sln`).
    - Create three subdirectories for your services: `PlayerService`, `MatchmakingService`, `NotificationService`.
    - Inside each, create a new C# "Worker Service" or "Console App" project. The Worker Service template is perfect for long-running services that listen for messages.
    - Add these projects to your solution.

2.  **Dockerize Everything:**
    - Create a `Dockerfile` for each of the three services. They will be very similar to start: use the .NET SDK base image, copy the project files, restore dependencies, and publish.
    - Create a `docker-compose.yml` file at the solution root.
    - Define four services in `docker-compose.yml`:
        - `rabbitmq`: Use the official `rabbitmq:3-management` image. This gives you the broker and a web UI.
        - `player-service`: Build from the `PlayerService` Dockerfile.
        - `matchmaking-service`: Build from the `MatchmakingService` Dockerfile.
        - `notification-service`: *Hold on this one for now (see Phase 2).*
    - Use `depends_on` to ensure RabbitMQ starts before your C# services.

3.  **Verify the "Shell" Works:**
    - Run `docker-compose up --build` for just RabbitMQ, PlayerService, and MatchmakingService.
    - Your goal here is **not** functionality, but to confirm all containers can be built and start without errors. The C# apps will crash because they can't connect to RabbitMQ yet, and that's expected for now.

---

### Phase 2: The Notification Service (Your Core Library)
**Goal:** Create the shared communication layer. This isn't a running service itself, but a library used by the other two.

1.  **Create a Class Library Project:**
    - Create a new project of type "Class Library" inside your `NotificationService` folder. Name it something like `Matcher.MessageBroker`.
    - This project will be referenced by both `PlayerService` and `MatchmakingService`.

2.  **Define the Contracts (Events):**
    - Inside this library, create classes for your event schemas. For example:
        - `PlayerJoinedEvent`
        - `RoomCreatedEvent`
        - `PlayerAssignedEvent`
        - `PlayerLeftEvent`
    - These classes are simple POCOs (Plain Old C# Objects) with properties matching your JSON schema (`Event`, `PlayerId`, `RoomId`, `Timestamp`).

3.  **Implement the RabbitMQ Client Helper:**
    - In the same library, create a class (e.g., `RabbitMqClient`) to encapsulate the logic for connecting to RabbitMQ, declaring queues/exchanges, and publishing/consuming messages.
    - This class will be injected into `PlayerService` and `MatchmakingService`. Its methods might look like `PublishEvent<T>(T event)` and `SubscribeToEvent<T>(string eventType, Action<T> handler)`.

4.  **Integrate the Library:**
    - Update your `PlayerService` and `MatchmakingService` projects to add a project reference to the new `Matcher.MessageBroker` library.
    - Update their `Dockerfile`s to ensure this dependency is copied correctly.

---

### Phase 3: Develop the Services (One at a Time)
**Goal:** Implement the business logic for each service, testing them in isolation.

1.  **Start with the Matchmaking Service:**
    - It's the brain of the operation. Implement the queue logic (`Queue<Player>`).
    - Use a `BackgroundService` or `Timer` to periodically check if the queue has 6 players.
    - When it does, create a unique `roomId`, and use the `RabbitMqClient` to publish a `RoomCreatedEvent` and six `PlayerAssignedEvents`.
    - **Test it:** You can temporarily add a console input or a dummy method to simulate "player joined" events. Publish a `PlayerJoinedEvent` manually and see if the service correctly forms a room and publishes the response events.

2.  **Then, build the Player Service:**
    - Implement the terminal interface with a simple menu: "Press 'J' to Join a game."
    - When the user presses 'J', the service should use the `RabbitMqClient` to publish a `PlayerJoinedEvent`.
    - It also needs to *subscribe* to `PlayerAssignedEvent` where the `PlayerId` matches its own.
    - Upon receiving its assignment, it should start a 5-minute timer, print the room ID, and then publish a `PlayerLeftEvent` when the timer elapses.

---

### Phase 4: End-to-End Integration & Polish
**Goal:** Make the two services talk to each other and handle edge cases.

- **Connect the Services:** Now that both services work independently, run them together via `docker-compose up`. Use the Player Service's terminal to join and watch the logs to see the full flow.
- **Implement Resilience:**
    - Add message acknowledgments (ACKs) in your RabbitMQ consumers.
    - Implement reconnection logic for the RabbitMQ client.
    - Think about what happens if a service crashes mid-matchmaking.
- **Finalize the `docker-compose.yml`:** Ensure all environment variables (for connection strings, queue names, etc.) are passed correctly to your containers.

### Where to Write Your First Line of Code

Your absolute first line of C# code should be in **Phase 2, Step 2**: defining the event classes in your `Matcher.MessageBroker` class library.

```csharp
// This is where you start coding.
namespace Matcher.MessageBroker.Events
{
    public class PlayerJoinedEvent
    {
        public string Event { get; set; } = "player_joined";
        public string PlayerId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
