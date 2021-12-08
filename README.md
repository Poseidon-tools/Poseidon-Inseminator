# Inseminator - DI addon to Poseidon framework.

Inseminator is a **dependency injection** framework built specifically to target Unity 3D and **Poseidon framework**.

# Compatibility

Tested in Unity 3D on the following platforms:

 - Standalone PC
 - Android (Mono + .Net Standard 2.0)
 - WebGL

## Purpose

In contrast to other Unity DI-frameworks available, we chosen Inseminator to have simple duty. The main and only purpose of Inseminator is to **serve dependencies for given scope**. No other functionalities, no messaging, no pub-sub systems hidden inside.

## Why not existing framework?

The idea of making our own DI-system show up when we've spent many time on writing another and another state machines with states, where all dependencies was passed in constructors. **More complex state equals more dependencies**, and this equals **looooong constructors**, even 15+ lines straight down.
We've tried integrate Extenject framework with our workflow, but the way how Extenject works, is not meeting our pipeline requirements. The main problem that occured also in our pipeline, is described in post linked below - original post is almost 4 years old now, followed by over 450 people and still waiting for any tips or solution. Even if this issue will be resolved in future, Extenject require too much additional effort to make our state machines and states working.
So, we decided to get dependency injection in our hands and make a system, that will satisfy our requirements. It's not perfect and it's not even finished yet, but at this moment is working on our main platforms, and most important - with state machines, even nested couple of times.

Related post: https://answers.unity.com/questions/1455259/how-to-fix-the-exception-zenjectexception-unable-t.html

## Main features

So, what our DI-system is able to?
* **field injection** (and property injection in future) at scene init (based on execution order) on any gameobject on scene, in any of your components -> see **Documentation/Injection**
* **injection in nested objects**, even if the object itself is not injected (even could be uninitialized and our system will try to force initialize it, if you'll allow for this in attribute) -> see **Documentation/Surrogates**
* **custom resolving modules support** - by default Inseminator comes with reflection-based injection module, which will handle injecting and surrogates for you. But you can implement your own module for more complex tasks - like we did by implementing custom module for resolving state machines, states inside and states dependencies(including nested state machines etc). See **Documentation/ResolvingModules**
* resolving for **3 main scopes - Scene, GameObject and ScriptableObject**. You can install your dependencies for scene objects (it's a main container for whole scene), you can use GameObjectDependencyResolver to separate your game object from scene dependencies and make it self-sufficient, using other set of installers/objects. Finally, you can use ScriptableObjectDependencyResolver to inject things in scriptable objects. To be honest, we don't think it will be useful, but it's ready to go.
