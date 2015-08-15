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
using System.Linq;
using Mono.Addins;
using Libgame;
using Libgame.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading;


using System.Xml.Linq;
using System.IO;
using System.Collections.ObjectModel;

namespace Bokuract.Scripts
{
    [Extension]
    public class Script : XmlExportable
    {
        private GameFile mapFile;
        private List<ScriptMap> scripts;

        public override string FormatName {
            get { return "Boku.Script"; }
        } 

        public override void Read(DataStream strIn)
        {
            // First uncompress the script files from the map file
            mapFile = new GameFile("Map_" + this.File.Name, strIn);
            mapFile.SetFormat<Bokuract.Packs.Pack>(new object[] { true });
            mapFile.Format.Read();

            // For each script, uncompress with GZip, divide in blocks and parse
            scripts = new List<ScriptMap>();
            foreach (GameFile scriptFile in mapFile.Files.Cast<GameFile>())
                scripts.Add(ReadScript(scriptFile));
        }

        private static ScriptMap ReadScript(GameFile script)
        {
            string scriptName = Path.GetFileNameWithoutExtension(script.Name);

            // Unzip
            script.SetFormat<Bokuract.Packs.GZip>();
            script.Stream.Seek(0, SeekMode.Origin);
            script.Format.Read();
            var unscript = script.Files[0] as GameFile;

            // and uncompress the blocks
            unscript.SetFormat<Bokuract.Packs.Pack>(new object[] { false });
            unscript.Stream.Seek(0, SeekMode.Origin);
            unscript.Format.Read();
            var dialogsFile = unscript.Files[1] as GameFile;

            // Parse each block
            dialogsFile.Stream.Seek(0, SeekMode.Origin);
            DataReader reader = new DataReader(dialogsFile.Stream);
            uint numBlocks = reader.ReadUInt32();

            ScriptMap scriptMap = new ScriptMap(scriptName);
            scriptMap.Dialogs = new Dialog[numBlocks];
            for (int i = 0; i < numBlocks; i++)
                scriptMap.Dialogs[i] = ParseScript(dialogsFile.Stream, i);

            return scriptMap;
        }

        private static Dialog ParseScript(DataStream strIn, int num)
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
            foreach (ScriptMap script in scripts) {
                XElement xscript = new XElement("Script");
                xscript.SetAttributeValue("Name", script.Name);
                root.Add(xscript);

                foreach (Dialog dialog in script.Dialogs) {
                    XElement xdialog = new XElement("Dialog");
                    xdialog.SetAttributeValue("DialogID", dialog.Id);
                    xscript.Add(xdialog);

                    foreach (string txt in dialog.Text) {
                        XElement xtext = new XElement("Text");
                        xtext.Value = txt.ToXmlString(4, '[', ']');
                        xdialog.Add(xtext);
                    }
                }
            }
        }

        protected override void Dispose(bool freeManagedResourcesAlso)
        {
        }

        private struct ScriptMap
        {
            public ScriptMap(string name) : this()
            {
                Name = name;
            }

            public string Name { get; private set; }

            public Dialog[] Dialogs { get; set; }
        }

        private struct Dialog
        {
            private List<string> keys;
            private List<string> text;

            public Dialog(ushort id) : this()
            {
                text = new List<string>();
                keys = new List<string>();
                Id   = id;
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

                for (int i = 0; i < data.Length; i ++) {
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
}

