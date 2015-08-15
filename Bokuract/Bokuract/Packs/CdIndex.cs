//
//  CdIndex.cs
//
//  Author:
//       Benito Palacios Sánchez <benito356@gmail.com>
//
//  Copyright (c) 2014 Benito Palacios Sánchez
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
using Libgame;
using Libgame.IO;
using Mono.Addins;

namespace Bokuract.Packs
{
    [Extension]
    public class CdIndex : Format
    {
        public static int Padding {
            get { return 0x800; }
        }

        public static string Type {
            get { return "DFI"; }
        }

        public override string FormatName {
            get { return "Boku1.DIF"; }
        }
            
        public uint Unknown {
            get;
            set;
        }

        public CdIndexEntry[] Entries {
            get;
            set;
        }

        public override void Read(DataStream strIn)
        {
            DataReader reader = new DataReader(strIn);

            // Read header
            string type = reader.ReadString(4);
            if (type != Type) throw new FormatException();
            this.Unknown = reader.ReadUInt32();
            strIn.Seek(8, SeekMode.Current);

            // Use the name offset of the main entry to get number of entries and skip it
            strIn.Seek(4, SeekMode.Current);
            int numEntries = reader.ReadInt32() / CdIndexEntry.EntrySize;
            strIn.Seek(8, SeekMode.Current);

            // Read entries
            this.Entries = new CdIndexEntry[numEntries - 1];
            for (int i = 0; i < this.Entries.Length; i++)
                this.Entries[i] = ReadEntry(reader);
        }

        private CdIndexEntry ReadEntry(DataReader reader)
        {
            long entryOffset = reader.Stream.Position;
            CdIndexEntry entry = new CdIndexEntry();

            entry.IsFolder   = (reader.ReadUInt16() == 1);
            entry.SubEntries = reader.ReadUInt16();
            entry.IsLastFile = (entry.SubEntries == 0);
            long nameOffset  = entryOffset + reader.ReadUInt32();
            entry.Offset     = reader.ReadUInt32() * Padding;
            entry.Size       = reader.ReadUInt32();

            reader.Stream.Seek(nameOffset, SeekMode.Origin);
            entry.Name = reader.ReadString();
            reader.Stream.Seek(entryOffset + CdIndexEntry.EntrySize, SeekMode.Origin);

            return entry;
        }

        public override void Write(DataStream strOut)
        {
            throw new NotImplementedException();
        }

        public override void Export(params DataStream[] strOut)
        {
            throw new NotImplementedException();
        }

        public override void Import(params DataStream[] strIn)
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool freeManagedResourcesAlso)
        {
        }
    }
}
