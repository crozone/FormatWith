using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace FormatWith.Internal
{
    internal static class StringBuilderCache
    {
        private static readonly ThreadLocal<WeakReference<CachedStringBuilder>> threadLocalStringBuilder =
            new ThreadLocal<WeakReference<CachedStringBuilder>>(() => new WeakReference<CachedStringBuilder>(null, false));

        private class CachedStringBuilder
        {
            public CachedStringBuilder(StringBuilder stringBuilder, ResultAction appenderAction)
            {
                StringBuilder = stringBuilder;
                AppenderAction = appenderAction;
            }

            public StringBuilder StringBuilder { get; }
            public ResultAction AppenderAction { get; }
        }

        /// <summary>
        /// Returns a StringBuilder of at least the given capacity, as well as an action delegate for appending to the StringBuilder.
        /// Caches the result in a weakly referenced ThreadLocal static variable.
        /// </summary>
        /// <param name="capacity">The minimum capacity of the returned StringBuilder</param>
        /// <returns></returns>
        internal static (StringBuilder stringBuilder, ResultAction appenderAction) GetStringBuilder(int capacity)
        {
            var weakRef = threadLocalStringBuilder.Value;
            if (weakRef.TryGetTarget(out CachedStringBuilder cache) && cache is not null)
            {
                cache.StringBuilder.Clear();
                cache.StringBuilder.EnsureCapacity(capacity);
            }
            else
            {
                StringBuilder stringBuilder = new StringBuilder(capacity);
                void ResultAction(ReadOnlySpan<char> value) => stringBuilder.Append(value);

                cache = new CachedStringBuilder(stringBuilder, ResultAction);
                weakRef.SetTarget(cache);
            }

            return (cache.StringBuilder, cache.AppenderAction);
        }
    }
}
