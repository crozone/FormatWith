using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FormatWith;

namespace FormatWithConsoleTests
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IReadOnlyCollection<FormatToken> tokens = null;
            string formatString1 = "test{param1}test{{escaped1}}{{{param2}}}{{{{escaped2}}}}test";
            string formatString2 = "test";
            try {
                tokens = FormatHelpers.Tokenize(formatString2);
            }
            catch (Exception e){
                Console.WriteLine(e);
            }

            if (tokens != null) {
                foreach (var thisToken in tokens) {
                    Console.WriteLine($"{thisToken.Text}");
                }

                string formattedOutput = FormatHelpers.ProcessTokens(tokens, ObjectHelpers.GetPropertiesDictionary(new {
                    param1 = "REPLACE1",
                    param2 = "REPLACE2"
                }));

                Console.WriteLine(formattedOutput);
                }



            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
