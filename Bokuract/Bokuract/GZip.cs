//
//  GZip.cs
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
using System.IO;
using Ionic.Zlib;
using Mono.Addins;
using Libgame;
using Libgame.IO;

namespace Bokuract
{
	[Extension]
	public class GZip : Format
	{
		public override string FormatName {
			get { return "Boku1.GZip"; }
		}

		public override void Read(DataStream strIn)
		{
			uint decodedSize = new DataReader(strIn).ReadUInt32();
            uint writtenBytes = 0;
            
			DataStream strOut = new DataStream(new MemoryStream(), 0, 0);
            GZipStream gzip   = new GZipStream(strIn.BaseStream, CompressionMode.Decompress, true);

            int count = 0;
            byte[] buffer = new byte[1024*10];
            do {
				count = gzip.Read(buffer, 0, buffer.Length);
                if (count != 0) {
                    strOut.Write(buffer, 0, count);
					writtenBytes += (uint)count;
                }
            } while (count > 0);
            
            if (writtenBytes != decodedSize)
				throw new FormatException("Decoded size doesn't match.");

			this.File.AddFile(new GameFile(gzip.FileName, strOut));
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
