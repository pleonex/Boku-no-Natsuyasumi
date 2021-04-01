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
    using System.Xml.Linq;
    using Yarhl.FileFormat;
    using Yarhl.IO;
    using Yarhl.Media.Text;

    public class Script2Po : IConverter<Script, Po>
    {
        public Po Convert(Script source)
        {
            var po = new Po(new PoHeader("Boku1", "TraduSquare", "en"));

            foreach (var script in source.Scripts) {
                foreach (Dialog dialog in script.Dialogs) {
                    for (int i = 0; i < dialog.Text.Count; i++) {
                        if (dialog.Text[i].Length == 0) {
                            continue;
                        }

                        var entry = new PoEntry {
                            Context = $"{script.Name}:{dialog.Id}:{i}",
                            Original = dialog.Text[i],
                        };
                        po.Add(entry);
                    }
                }
            }

            return po;
        }
    }
}
