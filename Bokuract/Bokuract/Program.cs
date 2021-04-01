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
namespace Bokuract
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using Bokuract.Packs;
    using Bokuract.Scripts;
    using Yarhl.FileSystem;
    using Yarhl.IO;
    using Yarhl.Media.Text;

    public static class Program
    {
        public static void Main(string[] args)
        {
            // First args is the path input & output folder
            if (args.Length == 0) {
                return;
            }

            Stopwatch watch = Stopwatch.StartNew();
            string folder = args[0];
            Node root = NodeFactory.FromDirectory(folder, "*", "root");

            // Unpack root files
            Console.WriteLine("Reading root file...");
            var rootIdx = root.Children["cdimg.idx"]
                .TransformWith<Binary2CdIndex>()
                .GetFormatAs<CdIndex>();
            root.Children["cdimg0.img"]
                .TransformWith<BinaryCdData2Container, CdIndex>(rootIdx);

            Console.WriteLine("Exporting files...");
            Export(folder, root);

            watch.Stop();
            Console.WriteLine("Done! It took: {0}", watch.Elapsed);
        }

        private static void Export(string output, Node container)
        {
            foreach (Node node in Navigator.IterateNodes(container)) {
                if (node.IsContainer) {
                    continue;
                }

                if (node.Parent.Path.EndsWith("/cdimg0.img/map/gz")) {
                    node.TransformWith<BinaryPack2Container, bool>(true);
                    var script = new Script();
                    foreach (var child in node.Children) {
                        var map = child.TransformWith<GZipDecompression, bool>(false)
                            .TransformWith<BinaryPack2Container, bool>(false)
                            .Children[1]
                            .TransformWith<Binary2ScriptMap>()
                            .GetFormatAs<ScriptMap>();
                        map.Name = child.Name;
                        script.Scripts.Add(map);
                    }

                    node.ChangeFormat(script);
                    node.TransformWith<Script2Po>()
                        .TransformWith<Po2Binary>()
                        .Stream.WriteTo(Path.Combine(output, node.Path[1..] + ".po"));
                } else if (node.Path.Contains("/cdimg0.img/map/gz")) {
                    // Ignore other files of the script container.
                } else if (node.Name.EndsWith(".gz")) {
                    node.TransformWith<GZipDecompression, bool>(false);
                } else if (node.Name.EndsWith(".gzx")) {
                    node.TransformWith<GZipDecompression, bool>(true);
                } else if (PackValidation.IsPackFile(node.Path, out bool hasName)) {
                    node.TransformWith<BinaryPack2Container, bool>(hasName);
                } else if (node.Format is IBinary) {
                    node.Stream.WriteTo(Path.Combine(output, node.Path[1..]));
                }
            }
        }
    }
}
