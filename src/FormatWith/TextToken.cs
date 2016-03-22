using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormatWith {
    public class TextToken : FormatToken {
        public TextToken(string source, int startIndex, int length) : base(source, startIndex, length) { }
    }
}
