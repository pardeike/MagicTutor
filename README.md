# MagicTutor
A framework for RimWorld mods that show hints for new features in a mod

Make your mods features discoverable with this framework! A lot of mod users have 
never seen your workshop page or have read your documentation. They install a modlist 
and suddenly have access to a new feature that they don't understand.

To solve this, this framework makes it easy for you to present hints to the user 
whenever some observable effect is produced by your code. All you have to do is:

## Basic usage

#### Create a context for a feature
1) Define a context name that groups all occurances of a feature. For example 
   `"turbo-button"` for your new widget button or `"ultra weapon"` for your new weapon.

2) Register this context at the start of your mod inside the class that extends `Mod`:
   ```cs
   using BrrainzTools;
   
   public class MyMod : Mod
   {
      public static MagicTutor tutor;
      
      public MyMod(ModContentPack content) : base(content)
      {
         // ...

         tutor = new MagicTutor(this);
         tutor.RegisterContext("turbo-button", new StandardHint() { message = "...hint text here..." });
         tutor.RegisterContext("ultra weapon", new StandardHint() { message = "...hint text here..." });
         // more registrations
         tutor.Start();
      }
   ```

3) Once you defined the contexts, you can wrap your existing code with a tutor call. 
   So if you render something, you can simply surround that code with
   ```cs
   // someRect is the area on screen you want the hint displayed for
   MyMod.tutor.DoHintableAction("turbo-button", someRect, (hintUsed) =>
   {
      // original code
      
      // optionally mark hint used so it disappears
      if (selected)
         hintUsed();
   });
   ```

Just by registering a context and wrapping some of your code with that context, you get 
all the features of MagicTutor for free:
- only one hint is ever presented to the user regardless of how many times your or some 
  other mod calls `DoHintableAction`
- hints are displayed close to the area they are explaining
- hints are positioned so they are not clipped by the screen bounds
- dismissed hints will never be shown again even if you reinstall all mods

## Advanced uses

For even more control you can create your own hints by either customizing the `StandardHint`
class or even create your own hint by implementing the abstract class `Hint`.

Override StandardHint:

```cs
// if you don't want the hint to cover content left of your area of interest
// override ShouldAvoidScreenPosition and return false for the positions that
// you don't like

public class NotFromRightHint : StandardHint
{
   public override bool ShouldAvoidScreenPosition(ScreenPosition position)
   {
      // if SmartTutor thinks the area of interest is on the right side of
      // the screen (thus displaying the hint left of the area of interest)
      // make it skip it
      return position == ScreenPosition.right;
   }
}
```

Create your own hint by implementing the `Hint` class of MagicTutor:

```cs
// for an example of an implementation, check out how MagicTutor implements
// StandardHint: https://github.com/pardeike/MagicTutor/blob/master/StandardHint.cs
public abstract class Hint
{
   public bool acknowledged;

   public abstract Vector2 WindowSize(Rect areaOfInterest);
   public abstract Vector2 WindowOffset(Rect areaOfInterest);
   public abstract void DoWindowContents(Rect canvas);
}
```
