# Priority EventBus System for Unity

****

# Overview

The **EventBus System** is a flexible and lightweight event management solution for Unity that follows the Publish-Subscribe design pattern. This system allows developers to efficiently manage communication between different parts of their game using events, promoting a decoupled and modular architecture.

The core components of this project include:

- **`EventBus.cs`**: The main class that manages event subscriptions and handles event dispatching.
- **`SubscribeAttribute.cs`**: A custom attribute used to mark methods for automatic registration for specific events.
- **`Subscriber.cs`**: Represents subscribers and stores information about event listeners.

# Features

- **Flexible Event Management**: Subscribe to and publish events without tightly coupling components.
- **Automatic Subscription with Reflection**: Methods marked with `[Subscribe]` are automatically registered to receive events, simplifying setup.
- **Priority-Based Dispatching**: Control the order in which subscribers receive notifications by assigning different priorities, ensuring that critical subscribers are notified first.

# Getting Started

To use the EventBus system in your Unity project, follow these steps:

## Installation

1. **Clone the Repository**: Clone this repository into your Unity project or copy the `EventBus.cs`, `SubscribeAttribute.cs`, and `Subscriber.cs` files into your scripts folder.
   ```bash
   git clone https://github.com/Ddemon26/EventBus.git
   ```
2. **Add the Scripts**: Place the scripts into your Unity Assets folder so they can be accessed by other game components.

## Basic Usage

### 1. Create an EventBus

```csharp
EventBus eventBus = new EventBus();
```

### 2. Subscribe to Events

To subscribe a method to an event, use the `[Subscribe]` attribute. The method must accept a parameter representing the event being dispatched.

```csharp
public class Player
{
    [Subscribe]
    public void OnPlayerScored(ScoreEvent scoreEvent)
    {
        Debug.Log($"Player scored: {scoreEvent.Points}");
    }
}
```

### 3. Publish Events

Publish events using the `EventBus` instance:

```csharp
ScoreEvent scoreEvent = new ScoreEvent(10);
eventBus.Publish(scoreEvent);
```

### 4. Register Subscribers with Priority

Register an object to receive events and specify a priority level:

```csharp
Player player = new Player();
eventBus.Register(player, priority: 1); // Higher priority subscribers are notified first
```

## Priority Example

The priority feature allows you to control the order in which subscribers receive notifications. Subscribers with higher priority values are processed first.

```csharp
public class AudioSystem
{
    [Subscribe]
    public void OnPlayerScored(ScoreEvent scoreEvent)
    {
        Debug.Log("Playing cheering sound.");
    }
}

public class AchievementSystem
{
    [Subscribe]
    public void OnPlayerScored(ScoreEvent scoreEvent)
    {
        Debug.Log("Tracking achievement progress.");
    }
}

// Register systems with different priorities
AudioSystem audioSystem = new AudioSystem();
AchievementSystem achievementSystem = new AchievementSystem();

// AudioSystem has higher priority, so it will be notified before AchievementSystem
eventBus.Register(audioSystem, priority: 2);
eventBus.Register(achievementSystem, priority: 1);

// Publish the event
ScoreEvent scoreEvent = new ScoreEvent(10);
eventBus.Publish(scoreEvent);
```

In this example, when the `ScoreEvent` is published, the `AudioSystem` is notified before the `AchievementSystem` because it has a higher priority.

# Example Scenario

Consider a scenario where multiple systems in your game need to react when a player scores points:

- **UI System**: Updates the score display.
- **Audio System**: Plays a cheering sound.
- **Achievement System**: Tracks progress towards an achievement.

With the EventBus system:

1. Each system creates a method with the `[Subscribe]` attribute to handle the score event.
2. Register these systems with the `EventBus` instance, specifying priorities as needed.
3. When a player scores, call `eventBus.Publish(new ScoreEvent(points));`, and all registered systems are notified in the correct priority order.

# Classes Summary

- **`EventBus`**: Manages registering, unregistering, and publishing events to subscribers, with support for priority-based notifications.
- **`SubscribeAttribute`**: Marks methods for automatic event subscription.
- **`Subscriber`**: Holds information about the subscribing object, the method, and the priority level.

# Contributing

Contributions are welcome! Feel free to submit pull requests to enhance the system or add new features. Please ensure that your code adheres to the existing coding style.

# License

This project is licensed under the MIT License - see the LICENSE file for more details.

# Contact

If you have questions or suggestions, join our community on [Discord](https://discord.gg/knwtcq3N2a) or open an issue on GitHub.