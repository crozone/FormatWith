# FormatWith

A C# library for performing named parameterized string formatting.

## Quick Info

This library provides named string formatting via the string extension .FormatWith(). It is written as a DNX project, publishes to NuGet package, and is fully compatible with .NET Core.

An example of what it can do:

"Your name is {name}, and this is {{escaped}}, this {{{works}}}, and this is {{{{doubleEscaped}}}}".FormatWith({name = "John", works = "is good");

Produces:

"Your name is John, and this is {escaped}, this {is good}, and this is {{doubleEscaped}}"

It can also be fed parameters via an `IDictionary<string, object>` directly, rather than a type.

## How it works

A state machine parser quickly runs through the input format string, tokenizing the input into tokens of either "normal" or "parameter" text. These tokens are simply an index and length which reference into the original format string - SubString is avoided to prevent unnecessary object allocations. These tokens are then fed sequentially into a StringBuilder, which produces the final output string.
