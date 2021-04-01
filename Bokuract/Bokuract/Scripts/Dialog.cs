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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Text;

    public class Dialog
    {
        private readonly List<string> keys;
        private readonly List<string> text;

        public Dialog(ushort id)
        {
            text = new List<string>();
            keys = new List<string>();
            Id = id;
        }

        public ushort Id {
            get;
            private set;
        }

        public ReadOnlyCollection<string> Text {
            get { return new ReadOnlyCollection<string>(text); }
        }

        public void AddUnparsedText(string key, ushort[] data)
        {
            text.Add(ParseText(data));
            keys.Add(key);
        }

        public string ParseText(ushort[] data)
        {
            StringBuilder stringer = new StringBuilder();

            for (int i = 0; i < data.Length; i++) {
                if (data[i] == 0x8001)
                    stringer.AppendLine();
                else if (data[i] == 0x8002)
                    stringer.AppendFormat("{{PAUSE: {0}}}\n", data[++i]);
                else if (data[i] == 0x8000)
                    break;
                else if (!Table.IsInRange(data[i]))
                    stringer.AppendFormat("{{{0}}}", data[i].ToString("X2"));
                else
                    stringer.Append(Table.GetChar(data[i]));
            }

            return stringer.ToString();
        }
    }
}
