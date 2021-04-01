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
    using System.Collections.Generic;
    using System.IO;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    public class Binary2ScriptMap : IConverter<BinaryFormat, ScriptMap>
    {
        public ScriptMap Convert(BinaryFormat source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            source.Stream.Seek(0, SeekOrigin.Begin);
            DataReader reader = new DataReader(source.Stream);
            uint numBlocks = reader.ReadUInt32();

            var scriptMap = new ScriptMap();
            scriptMap.Dialogs = new Dialog[numBlocks];
            for (int i = 0; i < numBlocks; i++) {
                scriptMap.Dialogs[i] = ParseScript(reader, i);
            }

            return scriptMap;
        }

        private static Dialog ParseScript(DataReader reader, int num)
        {
            // Read the FAT entry
            reader.Stream.Seek(4 + (num * 0x8), SeekOrigin.Begin);
            ushort id = reader.ReadUInt16();
            reader.ReadUInt16(); // Length
            uint offset = reader.ReadUInt32();

            // Read num of subelements
            reader.Stream.Seek(offset, SeekOrigin.Begin);
            uint elements = reader.ReadUInt32();

            // At the moment, read only text entries, skip the first 3 entries
            Dialog dialog = new Dialog(id);
            for (int i = 3; i < elements; i += 2) {
                // Firstly it's the dialog name ID
                reader.Stream.Seek(offset + 4 + (i * 4), SeekOrigin.Begin); // Go to block
                reader.Stream.Seek(offset + reader.ReadUInt32(), SeekOrigin.Begin);
                string dialogId = reader.ReadString();

                // Then, the text
                reader.Stream.Seek(offset + 4 + ((i + 1) * 4), SeekOrigin.Begin); // Go to block
                reader.Stream.Seek(offset + reader.ReadUInt32(), SeekOrigin.Begin);

                List<ushort> dialogData = new List<ushort>();
                ushort value = reader.ReadUInt16();
                while (value != 0x8000 && value != 0xFFFF && value != 0x0000) {
                    dialogData.Add(value);
                    value = reader.ReadUInt16();
                }

                dialog.AddUnparsedText(dialogId, dialogData.ToArray());
            }

            return dialog;
        }
    }
}
