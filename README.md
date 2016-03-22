# FormatWith

A C# library for performing named parameterized string formatting.

## Quick Info

This library provides named string formatting via the string extension .FormatWith(). It is written as a DNX project, publishes to NuGet package, and is fully compatible with .NET Core.

An example of what it can do:

`"Your name is {name}, and this is {{escaped}}, this {{{works}}}, and this is {{{{doubleEscaped}}}}".FormatWith({name = "John", works = "is good");`

Produces:

> "Your name is John, and this is {escaped}, this {is good}, and this is {{doubleEscaped}}"

It can also be fed parameters via an `IDictionary<string, object>` directly, rather than a type.

## How it works

A state machine parser quickly runs through the input format string, tokenizing the input into tokens of either "normal" or "parameter" text. These tokens are simply an index and length which reference into the original format string - SubString is avoided to prevent unnecessary object allocations. These tokens are then fed sequentially into a StringBuilder, which produces the final output string.

## Extension methods:

Two extention methods for `string` are defined in `FormatWith.FormatStringExtensions`, `FormatWith()` and `GetFormatParameters`.

### FormatWith

FormatWith() takes a format string containing named parameters, along with a dictionary (or type) of replacement parameters. Optionally, missing key behaviour, a fallback value, and custom brace characters can be specified. Two adjacent opening or closing brace characters in the format string are treated as escaped, and will be reduced to a single brace in the output string.

Missing key behaviour is specified by the MissingKeyBehaviour enum, which can be either `ThrowException`, `ReplaceWithFallback`, or `Ignore`.

`ThrowException` throws a `KeyNotFoundException` if a parameter in the format string did not have a corresponding key in the lookup dictionary.

`ReplaceWithFallback` inserts the value specified by `fallbackReplacementValue` in place of any parameters that did not have a corresponding key in the lookup dictionary.

`Ignore` ignores any parameters that did not have a corresponding key in the lookup dictionary, leaving them unmodified in the output string.

Examples:

`string output = "abc {Replacement1} {DoesntExist}".FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 });

output: Throws a `KeyNotFoundException` with the message "The parameter \"DoesntExist\" was not present in the lookup dictionary".

`string output = "abc {Replacement1} {DoesntExist}".FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 }, MissingKeyBehaviour.ReplaceWithFallback, "FallbackValue");`

output: "abc Replacement1 FallbackValue"

`string replacement = "abc {Replacement1} {DoesntExist}".FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 }, MissingKeyBehaviour.Ignore);

Using custom brace characters:

output: "abc Replacement1 {DoesntExist}"

`string replacement = "abc <Replacement1> <DoesntExist>".FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 }, MissingKeyBehaviour.Ignore, null,'<','>');`

output: "abc Replacement1 <DoesntExist>"


In addition to performing the string formatting, `GetFormatParameters()` can be used to get a list of parameter names out of a format string.

Example:

`List<string> parameters = "{parameter1} {parameter2} {{not a parameter}}".GetFormatParameters();`

output: parameters now contains "parameter1","parameter2".
