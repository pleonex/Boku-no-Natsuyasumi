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
using System.Collections.Generic;
using Libgame;
using Libgame.IO;
using Mono.Addins;

namespace Bokuract.Packs
{
    [Extension]
    public class CdData : Format
    {
        public override string FormatName {
            get { return "Boku1.DIF_DATA"; }
        }

        public GameFolder Root {
            get;
            private set;
        }

        public override void Read(DataStream strIn)
        {
            CdIndex index = (CdIndex)File.Dependencies["cdimg.idx"].Format;

            // Generate file system
            var entries = new Queue<CdIndexEntry>(index.Entries);
            while (entries.Count > 0)
                GiveFormat(entries, this.File);
        }

        private void GiveFormat(Queue<CdIndexEntry> entries,FileContainer folder)
        {
            CdIndexEntry entry = entries.Dequeue();
            if (!entry.IsFolder) {
                folder.AddFile(new GameFile(
                    entry.Name,
                    new DataStream(this.File.Stream, entry.Offset, entry.Size)
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

