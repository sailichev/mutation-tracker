# Mutation Tacker

This lightwieght library provides an easy way to check an object for fields modifications at any time.

It does add a single method to any object;
```csharp
public static Func<bool> TrackFields(this object @this);
```

You can check then if there are any field values changed by calling the returned function anytime.

### See [sample code](https://github.com/sailichev/mutation-tracker/blob/master/MutationTracker.Test/Sample.cs).

---

Limitations:

 * A field must be
    * value type implementing ```IEquitable<>```
    * immutable reference type (that is, changing a member field value spawns a new instance)
    * string
 * Parent object private fields are not tracked