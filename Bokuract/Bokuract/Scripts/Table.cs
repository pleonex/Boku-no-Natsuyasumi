// Copyright (c) 2021 Benito Palacios Sánchez
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
namespace Bokuract.Scripts
{
    using System;
    using System.IO;
    using System.Reflection;

    public static class Table
    {
        private const string Filename = "Bokuract.Scripts.table.txt";
        private static readonly char[] Dictionary = ReadTable();

        public static char GetChar(ushort code)
        {
            if (code > Dictionary.Length)
                throw new ArgumentOutOfRangeException(nameof(code));

            return Dictionary[code];
        }

        public static bool IsInRange(ushort code)
        {
            return code < Dictionary.Length;
        }

        private static char[] ReadTable()
        {
            var tableStream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(Filename);
            string[] lines = new StreamReader(tableStream).ReadToEnd().Split('\n');

            char[] dict = new char[1024];
            foreach (string l in lines) {
                if (l.Length != 8 || l[0] == '#')
                    continue;

                int code = int.Parse(l.Substring(0, 4));
                char ch = l[7];
                dict[code] = ch;
            }

            return dict;
        }
    }
}
