# Card Games Prototype

This repo is a prototype/warmup/prelude to a more robust card game solution. It exists to quickly familiarize myself
coding card games.

## Notes: The Component Design Pattern

This section contains info, quotes, and code from Bob
Nystrom's [Game Programming Patterns](https://gameprogrammingpatterns.com).

"Allow a single entity to span multiple domains without coupling the domains to each
other." 

Nystrom details this pattern as a way of decomposing game objects (like an object Bjorn that responds to player input,
has physics, and has graphics) by using interfaces and DI.

```c++
class GameObject
{
public:
  int velocity;
  int x, y;

  GameObject(InputComponent* input,
             PhysicsComponent* physics,
             GraphicsComponent* graphics)
  : input_(input),
    physics_(physics),
    graphics_(graphics)
  {}

  void update(World& world, Graphics& graphics)
  {
    input_->update(*this);
    physics_->update(*this, world);
    graphics_->update(*this, graphics);
  }

private:
  InputComponent* input_;
  PhysicsComponent* physics_;
  GraphicsComponent* graphics_;
};
```

```c++
GameObject* createBjorn()
{
  return new GameObject(new PlayerInputComponent(),
                        new BjornPhysicsComponent(),
                        new BjornGraphicsComponent());
}
```

Nystrom does highlight that communication may be necessary between components, and that some possible implementations
could be:

- container state
- injected components into each other (but probably only when they're closely related, like animation and rendering)
- utilize messaging, like have a messaging container injected into each component so they can send messages to all
  sibling components

## Notes: The Entity-Component-System (ECS) Pattern

This section contains info and quotes from Austin
Morlan's [A Simple Entity Component System (ECS) [C++]](https://austinmorlan.com/posts/entity_component_system/).

ECS builds off the Component pattern by having three major parts:

1. An entity, which is just an ID
    - Morlan details this as a runtime determined value, but I've also heard that FromSoftware has IDs for distinct game
      object classes, like an entity type ID
2. A component, which is a Plain Old Data (POD) struct containing data needed for a component's computation
    - This may also contain a component type ID
    - This has performance perks. It is common every object's physics are determined, and then to be drawn to the screen,
      by having a packed array of physics PODs, the goal is to minimize cache misses.
3. A system, which "is any functionality that iterates upon a list of entities with [certain] components," like a system
   that applies animation and rendering components or a system that applies physics and collision components.

There are some other parts to the implementation discussed by Morlan:

1. A signature, which tracks which compoment types an entity has
2. An entity manager, "is in charge of distributing entity IDs and keeping record of which IDs are in use and which are
   not"
3. A component array, which handles compacting the components as their entities are added and removed, as well as having
   methods to map to/from the array index and the entity ID
4. A component manager, which "is in charge of talking to all of the different ComponentArrays when a component needs to
   be added or removed"
5. A system manager, which "is in charge of maintaining a record of registered systems and their signatures"
6. The coordinator/welcome, which wires up all the managers and mediates this mess

### Applied Example + Value Demo

Here is interesting quote from r/SeniorePlatypus:

    The idea is to decouple data and processing. Allowing multiple systems to operate on the same components.

    For example. I can have a transform component (storing position, rotation, etc.). A velocity component (storing angular
    and linear velocity).

    A physics system can now update the transform based on velocity. Nice and easy.

    But now, if I want to have wind push stuff around I can just get all velocity components and apply a static value to
    them. Effectively pushing things around. Without a new component. Without breaking any old code. Without even touching
    any old code.

    That is what's nice about ECS. Components, data itself acts as strongly defined interfaces between systems. You are able
    to seamlessly add or remove systems at runtime. Anytime you have a bug you know it's gonna end up in the system
    responsible for this feature. Anytime you need something extra, a new feature you can just write it. No touching or
    integrating into other parts of the game.

This details that an adv of ECS over just using Components is that it allows for this runtime registration of systems
to operate on the same component.

### System Interactions

Here is a quote from r/ajvrgames demonstrating how systems can interact:

    Your collision system would usually mark up colliding entities, and then separate systems would handle those collisions.

    For instance, your collision system would mark an entity as having collided with another entity. A second system would
    look for all entities marked as having health with collisions and test to see if the collider is damaging; a third
    system might look for players with collisions and test to see if the collider is a monster, etc.

    Alternatively, you might want to store collisions as entities themselves, so that each time you detect a collision you
    create an entity with a collision component that references the two colliding entities. You can then handle these
    entities like any other in collision-type-specific systems again.

## Notes: Unity Game Objects

Unity's `GameObject` is a Component implementation that reportedly has ECS support as well.

https://docs.unity3d.com/Manual/GameObjects.html

https://unity.com/ecs
