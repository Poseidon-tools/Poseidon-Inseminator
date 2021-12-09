# Inseminator - DI addon to Poseidon framework.

Inseminator is a **dependency injection** framework built specifically to target Unity 3D and **Poseidon framework**.

## Status
#### Addon status: in development
Must-have todo list:
* resolvers performance optimalization (reduce GC collection and CPU time usage -> optimize attributes searching)
* property injection
* support injection to GameObject resolvers from scene resolver if needed

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

# Documentation

How to use Inseminator system?
Table of content:
* Simple Injection
	* Dependency resolvers
	* Installers & Binding
	* Resolving Modules
* Surrogates
* Injecting into instantiated object
	* Factories
* Advanced stuff
	* Self-sufficient instantiated objects
	* Resolving Poseidon's State Machines


## Simple Injection
### Default way

By default, we can inject value into object in a few ways. We can do it by constructor (if we're working on non-mono classes), in MonoBehaviours we can do it with some sort of injection method and pass dependencies as method params, or make a ugly and low-performance calls to one of Unity's Find methods, like FindObjectOfType\<T>.
```C#
// constructor injection example
public MyClass(MyObject myObject)
{
    this.myObject = myObject;
}

// method injection example in Mono class
public void Initialize(MyObject myObject)  
{  
  this.myObject = myObject;  
}

// using Unity's default, low-performance methods in Mono class
private void Start()
{
    myObject = FindObjectOfType<MyObject>();
}
```
### Using Inseminator
Injecting values into MonoBehaviours is pretty simple, and require you to use special attribute, that comes with Inseminator.
```C#
public class MonoInjectable : MonoBehaviour  
{  
	#region Private Variables  
	[InseminatorAttributes.Inseminate] private ViewManager viewManager;  
	[InseminatorAttributes.Inseminate] private InseminatorMonoFactory monoFactory;  
	#endregion
	#region Unity Methods
	private void Start()  
	 {
		 Debug.Log($"Hey, {name} here! I'm using properly injected ViewManager: {viewManager.name}");  
	 }
	 #endregion
 }
```
As a result you'll see message logged in console, and suprisingly no NullReferenceException error, although at first glance it seems that viewManager can't have value, because it's nor exposed in inspector or initialized via any method.

Using `[Insemine]` attribute is only a top of an iceberg, but it's required to tell Inseminator system that you have field in your component, waiting for value to be injected. Next step is dependency resolvers setup.
