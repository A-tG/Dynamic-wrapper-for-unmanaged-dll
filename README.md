# Dynamic wrapper for unmanaged dll
Visual Studio's C# [**Shared Project**](https://github.com/A-tG/Voicemeeter-Remote-API-dll-dynamic-wrapper/wiki/Useful-Info#how-to-useadd-a-visual-studio-shared-project). Helps to dynamically load an unmanaged library at runtime. 
 
Abstract (base) class, requires implementation. [Example (partial class)](https://github.com/A-tG/Voicemeeter-Remote-API-dll-dynamic-wrapper/blob/main/voicemeeter%20remote%20api%20wrap/RemoteApiWrapper%20partial/GetParameters.cs)

## How to add procedures from the DLL:
```csharp
   private delegate int ProcedureNameFromDLL(IntPtr someParam1, ref int someParam2);
   private ProcedureNameFromDLL myFunc;
   public int MethodName(IntPtr someParam1, ref int someParam2)
   {
        // do something with parameters here if needed
        return myFunc(someParam1, ref someParam2);
   }

   // And initialize "myFunc" somewhere (for example in constructor):
   GetReadyDelegate(ref myFunc);
```
or
```csharp
   private delegate int myNameForDelegate(IntPtr someParam1, ref int someParam2);
   private myNameForDelegate myFunc;
   public int MethodName(IntPtr someParam1, ref int someParam2)
   {
        // do something with parameters here if needed
        return myFunc(someParam1, ref someParam2);
   }

   // And initialize "myFunc" somewhere (for example in constructor):
   GetReadyDelegate(ref myfunc, "ProcedureNameFromDLL");
or
```csharp
   private delegate int ProcedureNameFromDLL(IntPtr someParam1, ref int someParam2);
   private ProcedureNameFromDLL myFunc;
   public int MethodName(IntPtr someParam1, ref int someParam2)
   {
        // do something with parameters here if needed
        return myFunc(someParam1, ref someParam2);
   }

   // And initialize "myFunc" somewhere (for example in constructor):
   myFunc = GetReadyDelegate<ProcedureNameFromDLL>();
```
or
```csharp
   private delegate int myNameForDelegate(IntPtr someParam1, ref int someParam2);
   private myNameForDelegate myFunc;
   public int MethodName(IntPtr someParam1, ref int someParam2)
   {
        // do something with parameters here if needed
        return myFunc(someParam1, ref someParam2);
   }

   // And initialize "myFunc" somewhere (for example in constructor):
   myFunc = GetReadyDelegate<myNameForDelegate>("ProcedureNameFromDLL");
```
## To achieve compatibility with different versions of DLL:
```csharp
   private delegate int ProcedureNameFromDLL(IntPtr someParam1, ref int someParam2);
   private ProcedureNameFromDLL myFunc;
   public int MethodName(IntPtr someParam1, ref int someParam2)
   {
        if (myFunc is null) return SOME_ERROR_CODE;
        // do something with parameters here if needed
        return myFunc(someParam1, ref someParam2);
   }

   // And initialize "myFunc" somewhere (for example in constructor),
   // TryGetReadyDelegate() will not throw exception if procedure is not found:
   bool isProcReceived = TryGetReadyDelegate(ref myFunc);
```
or
```csharp
   private delegate int myNameForDelegate(IntPtr someParam1, ref int someParam2);
   private myNameForDelegate myFunc;
   public int MethodName(IntPtr someParam1, ref int someParam2)
   {
        if (myFunc is null) return SOME_ERROR_CODE;
        // do something with parameters here if needed
        return myFunc(someParam1, ref someParam2);
   }

   // And initialize "myFunc" somewhere (for example in constructor),
   // TryGetReadyDelegate() will not throw exception if procedure is not found:
   bool isProcReceived = TryGetReadyDelegate(ref myFunc, "ProcedureNameFromDLL");
```
