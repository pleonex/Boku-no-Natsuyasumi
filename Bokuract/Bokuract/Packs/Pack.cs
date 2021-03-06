﻿//
//  Pack.cs
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
    public class Pack : Format
    {
        public override string FormatName {
            get { return "Boku1.Pack"; }
        }

        public override void Read(DataStream strIn)
        {
            DataReader reader = new DataReader(strIn);

            bool withNames = (bool)this.parameters[0];
            int entrySize = withNames ? 0x0C : 0x08;

            int numFiles = reader.ReadInt32();
            for (int i = 0; i < numFiles; i++) {
                strIn.Seek(4 + i * entrySize, SeekMode.Origin);

                uint offset = reader.ReadUInt32();
                uint size   = reader.ReadUInt32();
                if (offset == 0 || size == 0)
                    continue;

                string filename;
                if (withNames) {
                    uint nameOffset = reader.ReadUInt32();
                    strIn.Seek(nameOffset, SeekMode.Origin);
                    filename = reader.ReadString();
                } else {
                    filename = this.File.Name + "_" + i + ".dat";
                }

                DataStream fileStream = new DataStream(strIn, offset, size);
                this.File.AddFile(new GameFile(filename, fileStream));
            }
        }

        public override void Write(DataStream strOut)
        {
            throw new NotImplementedException();
        }

        public override void Import(params DataStream[] strIn)
        {
            throw new NotImplementedException();
        }

        public override void Export(params DataStream[] strOut)
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool freeManagedResourcesAlso)
        {
        }
    }
}

