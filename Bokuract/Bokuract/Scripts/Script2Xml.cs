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

    public class Script2Xml : IConverter<Script, BinaryFormat>
    {
        public BinaryFormat Convert(Script source)
        {
            var doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
            var root = new XElement("Script");
            doc.Add(root);

            foreach (var script in source.Scripts) {
                XElement xscript = new XElement("Script");
                xscript.SetAttributeValue("Name", script.Name);
                root.Add(xscript);

                foreach (Dialog dialog in script.Dialogs) {
                    XElement xdialog = new XElement("Dialog");
                    xdialog.SetAttributeValue("DialogID", dialog.Id);
                    xscript.Add(xdialog);

                    foreach (string txt in dialog.Text) {
                        XElement xtext = new XElement("Text");
                        xtext.SetIndentedValue(txt, 4);
                        xdialog.Add(xtext);
                    }
                }
            }

            var binary = new BinaryFormat();
            doc.Save(binary.Stream);
            return binary;
        }
    }
}
