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
using Mono.Addins;
using Libgame.IO;
using System.Collections.Generic;

namespace Bokuract
{
	[Extension]
	public class CdIndex : Format
	{
		public static int Padding { get { return 0x800; } }
		public override string FormatName { get { return "Boku1.DIF"; } }

		public string Type     { get { return "DFI"; } }
		public uint   Unknown  { get; set; }
		public GameFolder Root { get; private set; }
		private GameFile FileData { get; set; }

		public override void Initialize(GameFile file, params object[] parameters)
		{
			base.Initialize(file, parameters);
			this.FileData = file.Dependencies["cdimg0.img"];
		}

		public override void Read(DataStream strIn)
		{
			DataReader reader = new DataReader(strIn);

			// Read header
			string type = reader.ReadString(4);
			if (type != this.Type) throw new FormatException();
			this.Unknown = reader.ReadUInt32();
			strIn.Seek(8, SeekMode.Current);

			// Use the name offset of the main entry to get number of entries and skip it
			strIn.Seek(4, SeekMode.Current);
			int numEntries = reader.ReadInt32() / CdIndexEntry.EntrySize;
			strIn.Seek(8, SeekMode.Current);

			// Read entries
			Queue<CdIndexEntry> entries = new Queue<CdIndexEntry>();
			for (int i = 0; i < numEntries - 1; i++)
				entries.Enqueue(ReadEntry(reader));

			// Generate file system
			GameFolder root = new GameFolder("cdimg");
			while (entries.Count > 0)
				GiveFormat(entries, root);
			this.File.AddFolder(root);
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

		private void GiveFormat(Queue<CdIndexEntry> entries, GameFolder folder)
		{
			CdIndexEntry entry = entries.Dequeue();
			if (!entry.IsFolder) {
				folder.AddFile(new GameFile(
					entry.Name,
					new DataStream(this.FileData.Stream, entry.Offset, entry.Size)
				));
				return;
			}

			// Create the folder
			GameFolder currFolder = new GameFolder(entry.Name, folder);

			// Add files and folders
			for (int i = 0; i < entry.SubEntries - 1; i++)
				GiveFormat(entries, currFolder);
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

