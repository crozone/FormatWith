# FormatWith

[![NuGet](https://img.shields.io/badge/nuget-1.4.1-green.svg)](https://www.nuget.org/packages/FormatWith/)
[![license](https://img.shields.io/github/license/mashape/apistatus.svg?maxAge=2592000)]()

A C# library for performing {named} {{parameterized}} string formatting.

## Quick Info

This library provides named string formatting via the string extension .FormatWith(). It formats strings with named parameters based upon an input lookup dictionary or type.

It is written as a PCL class library, publishes to NuGet package, and is fully compatible with .NET Core. It is currently built against .NETStandard 1.3 and .NET Full Framework 4.5, which makes it compatible with .NET Core 1.0, .NET Full 4.5+, UWP/uap 10, and most mono/xamarin platforms.

An example of what it can do:

`"Your name is {name}, and this is {{escaped}}, this {{{works}}}, and this is {{{{doubleEscaped}}}}".FormatWith({name = "John", works = "is good");`

Produces:

> "Your name is John, and this is {escaped}, this {is good}, and this is {{doubleEscaped}}"

It can also be fed parameters via an `IDictionary<string, string>` or an `IDictionary<string, object>`, rather than a type.

The value of each replacement parameter is given by whatever the object's .ToString() method produces. This value is not cached, so you can get creative with the implementation (the object is fed directly into a StringBuilder).

## How it works

A state machine parser quickly runs through the input format string, tokenizing the input into tokens of either "normal" or "parameter" text. These tokens are simply an index and length which reference the original format string - SubString is avoided to prevent unnecessary object allocations. These tokens are provided by the tokenizer's enumerator which is given to a token processor, which in turn feeds each token into a StringBuilder. Since StringBuilder is only `.Append()`ed relatively large segments of string, it produces the final output string quickly and efficiently.

## Extension methods:

Two extension methods for `string` are defined in `FormatWith.FormatStringExtensions`, `FormatWith()` and `GetFormatParameters()`.

### FormatWith(1,2,3)

The first, second, and third overload of FormatWith() take a format string containing named parameters, along with an object (1) or dictionary (2) of replacement parameters. Optionally, missing key behaviour, a fallback value, and custom brace characters can be specified. Two adjacent opening or closing brace characters in the format string are treated as escaped, and will be reduced to a single brace in the output string.

Missing key behaviour is specified by the MissingKeyBehaviour enum, which can be either `ThrowException`, `ReplaceWithFallback`, or `Ignore`.

`ThrowException` throws a `KeyNotFoundException` if a parameter in the format string did not have a corresponding key in the lookup dictionary.

`ReplaceWithFallback` inserts the value specified by `fallbackReplacementValue` in place of any parameters that did not have a corresponding key in the lookup dictionary.

`Ignore` ignores any parameters that did not have a corresponding key in the lookup dictionary, leaving the unmodified braced parameter in the output string.

**Examples:**

`string output = "abc {Replacement1} {DoesntExist}".FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 });

output: Throws a `KeyNotFoundException` with the message "The parameter \"DoesntExist\" was not present in the lookup dictionary".

`string output = "abc {Replacement1} {DoesntExist}".FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 }, MissingKeyBehaviour.ReplaceWithFallback, "FallbackValue");`

output: "abc Replacement1 FallbackValue"

`string replacement = "abc {Replacement1} {DoesntExist}".FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 }, MissingKeyBehaviour.Ignore);

output: "abc Replacement1 {DoesntExist}"

**Using custom brace characters:**

Custom brace characters can be specified for both opening and closing parameters, if required.

`string replacement = "abc <Replacement1> <DoesntExist>".FormatWith(new { Replacement1 = Replacement1, Replacement2 = Replacement2 }, MissingKeyBehaviour.Ignore, null,'<','>');`

output: "abc Replacement1 <DoesntExist>"

### GetFormatParameters

`GetFormatParameters()` can be used to get a list of parameter names out of a format string, which can be used for inspecting a format string before performing other actions on it.

**Example:**

`IEnumerable<string> parameters = "{parameter1} {parameter2} {{not a parameter}}".GetFormatParameters();`

output: parameters will return "parameter1","parameter2" during iteration.

## Tests:

A testing project is included that covers basic functionality.

## Performance:

The SpeedTest test function performs 1,000,000 string formats, with a format string containing 1 parameter. On a low end 1.3Ghz mobile i7, this completes in around 700ms, giving ~1.4 million replacements per second.

The SpeedTestBigger test performs a more complex replacement on a longer string containing 2 parameters and several escaped brackets, again 1,000,000 times. On the same hardware, this test completed in around 1 seconds.

The SpeedTestBiggerAnonymous test is the same as SpeedTestBigger, but uses the anonymous function overload of FormatWith. It completes in just under 2 seconds. Using the anonymous overload of FormatWith is slightly slower due to reflection overhead, although this is minimised by caching.

So as a rough performance guide, FormatWith will usually manage about 1 million parameter replacements per second on low end hardware.
