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
namespace Bokuract.Packs
{
    using System;
    using System.Collections.Generic;
    using Yarhl.FileFormat;
    using Yarhl.FileSystem;
    using Yarhl.IO;

    public class BinaryCdData2Container :
        IInitializer<CdIndex>, IConverter<BinaryFormat, NodeContainerFormat>
    {
        private CdIndex index;

        public void Initialize(CdIndex parameters)
        {
            index = parameters;
        }

        public NodeContainerFormat Convert(BinaryFormat source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (index == null)
                throw new InvalidOperationException("Missing index initialization");

            var container = new NodeContainerFormat();
            var entries = new Queue<CdIndexEntry>(index.Entries);
            while (entries.Count > 0) {
                CreateFileSystem(entries, source.Stream, container.Root);
            }

            return container;
        }

        private void CreateFileSystem(Queue<CdIndexEntry> entries, DataStream stream, Node folder)
        {
            CdIndexEntry entry = entries.Dequeue();
            if (!entry.IsFolder) {
                folder.Add(NodeFactory.FromSubstream(entry.Name, stream, entry.Offset, entry.Size));
                return;
            }

            // Create the folder and add its files
            var currFolder = NodeFactory.CreateContainer(entry.Name);
            folder.Add(currFolder);
            for (int i = 0; i < entry.SubEntries - 1; i++) {
                CreateFileSystem(entries, stream, currFolder);
            }
        }
    }
}
