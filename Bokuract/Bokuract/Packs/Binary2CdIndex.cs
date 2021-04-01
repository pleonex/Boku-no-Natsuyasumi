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
    using System.IO;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    public class Binary2CdIndex : IConverter<BinaryFormat, CdIndex>
    {
        public CdIndex Convert(BinaryFormat source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            DataReader reader = new DataReader(source.Stream);

            // Read header
            string type = reader.ReadString();
            if (type != CdIndex.Type) {
                throw new FormatException($"Invalid header: {type}");
            }

            var index = new CdIndex();
            index.Unknown = reader.ReadUInt32();
            source.Stream.Seek(8, SeekOrigin.Current);

            // Use the name offset of the main entry to get number of entries and skip it
            source.Stream.Seek(4, SeekOrigin.Current);
            int numEntries = reader.ReadInt32() / CdIndexEntry.EntrySize;
            source.Stream.Seek(8, SeekOrigin.Current);

            // Read entries
            index.Entries = new CdIndexEntry[numEntries - 1];
            for (int i = 0; i < index.Entries.Length; i++) {
                index.Entries[i] = ReadEntry(reader);
            }

            return index;
        }

        private CdIndexEntry ReadEntry(DataReader reader)
        {
            long entryOffset = reader.Stream.Position;
            CdIndexEntry entry = new CdIndexEntry();

            entry.IsFolder = reader.ReadUInt16() == 1;
            entry.SubEntries = reader.ReadUInt16();
            entry.IsLastFile = entry.SubEntries == 0;
            long nameOffset = entryOffset + reader.ReadUInt32();
            entry.Offset = reader.ReadUInt32() * CdIndex.Padding;
            entry.Size = reader.ReadUInt32();

            reader.Stream.Seek(nameOffset, SeekOrigin.Begin);
            entry.Name = reader.ReadString();
            reader.Stream.Seek(entryOffset + CdIndexEntry.EntrySize, SeekOrigin.Begin);

            return entry;
        }
    }
}
