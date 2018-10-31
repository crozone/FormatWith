using System;

namespace FormatWithTests.FormatProvider
{
    internal class UpperCaseFormatProvider : IFormatProvider
    {
        private readonly UpperCaseFormatter _formatter;

        public UpperCaseFormatProvider()
        {
            _formatter = new UpperCaseFormatter();
        }

        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
                return _formatter;

            return null;
        }
 
        class UpperCaseFormatter : ICustomFormatter
        {
            public string Format(string format, object arg, IFormatProvider formatProvider)
            {
                if(arg == null) return string.Empty;

                if (format == "upper" && arg is string str)
                {
                    return str.ToUpper();
                }

                return arg.ToString();
            }
        }
    }
}
