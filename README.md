# FormatWith

[![NuGet](https://img.shields.io/badge/nuget-1.2.0-green.svg)](https://www.nuget.org/packages/FormatWith/)

A C# library for performing named parameterized string formatting.

## Quick Info

This library provides named string formatting via the string extension .FormatWith(). It is written as a DNX project, publishes to NuGet package, and is fully compatible with .NET Core. It is currently build against 1.0.0-rc1-update1.

An example of what it can do:

`"Your name is {name}, and this is {{escaped}}, this {{{works}}}, and this is {{{{doubleEscaped}}}}".FormatWith({name = "John", works = "is good");`

Produces:

> "Your name is John, and this is {escaped}, this {is good}, and this is {{doubleEscaped}}"

It can also be fed parameters via an `IDictionary<string, object>` directly, rather than a type.

## How it works

A state machine parser quickly runs through the input format string, tokenizing the input into tokens of either "normal" or "parameter" text. These tokens are simply an index and length which reference into the original format string - SubString is avoided to prevent unnecessary object allocations. These tokens are then fed sequentially into a StringBuilder, which produces the final output string.

## Extension methods:

Two extension methods for `string` are defined in `FormatWith.FormatStringExtensions`, `FormatWith()` and `GetFormatParameters`.

### FormatWith(1/2)

The first and second overload of FormatWith() takes a format string containing named parameters, along with an object (1) or dictionary (2) of replacement parameters. Optionally, missing key behaviour, a fallback value, and custom brace characters can be specified. Two adjacent opening or closing brace characters in the format string are treated as escaped, and will be reduced to a single brace in the output string.

Missing key behaviour is specified by the MissingKeyBehaviour enum, which can be either `ThrowException`, `ReplaceWithFallback`, or `Ignore`.

`ThrowException` throws a `KeyNotFoundException` if a parameter in the format string did not have a corresponding key in the lookup dictionary.

`ReplaceWithFallback` inserts the value specified by `fallbackReplacementValue` in place of any parameters that did not have a corresponding key in the lookup dictionary.

`Ignore` ignores any parameters that did not have a corresponding key in the lookup dictionary, leaving them unmodified in the output string.

**Examples:**

`string output = "abc {Replacement1} {DoesntExist}".FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 });

output: Throws a `KeyNotFoundException` with the message "The parameter \"DoesntExist\" was not present in the lookup dictionary".

`string output = "abc {Replacement1} {DoesntExist}".FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 }, MissingKeyBehaviour.ReplaceWithFallback, "FallbackValue");`

output: "abc Replacement1 FallbackValue"

`string replacement = "abc {Replacement1} {DoesntExist}".FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 }, MissingKeyBehaviour.Ignore);

**Using custom brace characters:**

output: "abc Replacement1 {DoesntExist}"

`string replacement = "abc <Replacement1> <DoesntExist>".FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 }, MissingKeyBehaviour.Ignore, null,'<','>');`

output: "abc Replacement1 <DoesntExist>"

### FormatWith(3)

The second overload of `FormatWith()` takes a format string containing named parameters, along with an `Action<FormatToken, StringBuilder>`. The action is a handler that is called sequentially with each `FormatToken` parsed from the format string, and uses the information given by this `FormatToken` to mutate the `StringBuilder`. This allows full extensibility in controlling how parameters are handled, and also allows control over how they affect surrounding standard text.

`FormatWith(1/2)` uses this method internally by passing a simple dictionary replacement handler as the action.

### FormatParameters

`FormatParameters()` can be used to get a list of parameter names out of a format string, which can be used for inspection before performing other actions on it.

**Example:**

`IEnumerable<string> parameters = "{parameter1} {parameter2} {{not a parameter}}".FormatParameters();`

output: parameters will return "parameter1","parameter2" during iteration.

## Tests:

A testing project is included that covers basic functionality.

## Performance:

On a low end 1.3Ghz mobile i7, the SpeedTest test function completes in around 1 second. The SpeedTest test performs 1,000,000 string formats, with a format string containing 1 parameter.

The SpeedTestBigger test performs a more complex replacement on a longer string containing 2 parameters and several escaped brackets, again 1,000,000 times. On the same hardware, this test completed in around 2 seconds.

The SpeedTestBiggerAnonymous test is the same as SpeedTestBigger, but uses the anonymous function overload of FormatWith. It completes in around 2 seconds, with no noticeable difference in speed to the non-anonymous overload. 

So as a rough performance guide, FormatWith can perform about 1,000,000 parameter replacements per second on low end hardware.