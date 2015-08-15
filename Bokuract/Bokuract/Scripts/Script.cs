//
//  Script.cs
//
//  Author:
//       Benito Palacios Sánchez (aka pleonex) <benito356@gmail.com>
//
//  Copyright (c) 2015 Benito Palacios Sánchez (c) 2015
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using Mono.Addins;
using Libgame;
using Libgame.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Bokuract.Scripts
{
    [Extension]
    public class Script : XmlExportable
    {
        private GameFile scriptFile;
        private Dialog[] dialogs;

        public override string FormatName {
            get { return "Boku.Script"; }
        } 

        public override void Read(DataStream strIn)
        {
            // First uncompress the blocks
            scriptFile = new GameFile("script", strIn);
            scriptFile.SetFormat<Bokuract.Packs.Pack>(new object[] { false });
            scriptFile.Format.Read();

            var dialogsFile = scriptFile.Files[1] as GameFile;

            // Parse each block
            DataReader reader = new DataReader(dialogsFile.Stream);
            uint numBlocks = reader.ReadUInt32();
            dialogs = new Dialog[numBlocks];
            for (int i = 0; i < numBlocks; i++)
                dialogs[i] = ParseBlock(dialogsFile.Stream, i);
        }

        private static Dialog ParseBlock(DataStream strIn, int num)
        {
            DataReader reader = new DataReader(strIn);

            // Read the FAT entry
            strIn.Seek(4 + num * 0x8, SeekMode.Origin);
            ushort id = reader.ReadUInt16();
            reader.ReadUInt16();    // Length
            uint offset = reader.ReadUInt32();

            // Read num of subelements
            strIn.Seek(offset, SeekMode.Origin);
            uint elements = reader.ReadUInt32();

            // At the moment, read only text entries, skip the first 3 entries
            Dialog dialog = new Dialog(id);
            for (int i = 3; i < elements; i += 2) {
                // Firstly it's the dialog name ID
                strIn.Seek(offset + 4 + i * 4, SeekMode.Origin);          // Go to block
                strIn.Seek(offset + reader.ReadUInt32(), SeekMode.Origin);
                string dialogId = reader.ReadString();

                // Then, the text
                strIn.Seek(offset + 4 + (i + 1) * 4, SeekMode.Origin);    // Go to block
                strIn.Seek(offset + reader.ReadUInt32(), SeekMode.Origin);

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

        public override void Write(DataStream strOut)
        {
            throw new NotImplementedException();
        }

        protected override void Import(XElement root)
        {
            throw new NotImplementedException();
        }

        protected override void Export(XElement root)
        {
            foreach (Dialog dialog in dialogs) {
                XElement xdialog = new XElement("Dialog");
                root.Add(xdialog);

                xdialog.SetAttributeValue("ID", dialog.Id);
                foreach (string id in dialog.Text.Keys)
                    xdialog.Add(
                        new XElement("Text", dialog.Text[id].ToXmlString(2, '[', ']')));
            }
        }


        protected override void Dispose(bool freeManagedResourcesAlso)
        {
            if (freeManagedResourcesAlso)
                scriptFile.Stream.Dispose();
        }

        private struct Dialog
        {
            private Dictionary<string, string> text;

            public Dialog(ushort id) : this()
            {
                text = new Dictionary<string, string>();
                Id   = id;
            }

            public ushort Id {
                get;
                private set;
            }

            public ReadOnlyDictionary<string, string> Text {
                get { return new ReadOnlyDictionary<string, string>(text); }
            }

            public void AddUnparsedText(string key, ushort[] data)
            {
                text[key] = ParseText(data);
            }

            public string ParseText(ushort[] data)
            {
                StringBuilder stringer = new StringBuilder();

                for (int i = 0; i < data.Length; i ++) {
                    if (data[i] == 0x8001)
                        stringer.AppendLine();
                    else if (data[i] == 0x8002)
                        stringer.AppendFormat("{{PAUSE: {0}}}\n", data[++i]);
                    else if (data[i] == 0x8000)
                        break;
                    else if (!Table.IsInRange(data[i]))
                        stringer.AppendFormat("{{{0:X2}}}", data[i]);
                    else
                        stringer.Append(Table.GetChar(data[i]));
                }

                stringer.Replace("\0", "");
                return stringer.ToString();
            }
        }
    }
}

