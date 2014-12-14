//
//  CdIndexEntry.cs
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

namespace Bokuract
{
	public struct CdIndexEntry
	{
		private const int Padding = 0x800;

		public CdIndexEntry(int type, int flag, string name, long offset, long size)
			: this()
		{
			this.IsFolder = (type == 1);
			this.Name    = name;
			this.Offset  = offset * Padding;
			this.Size    = size;

			if (this.IsFolder)
				this.SubEntries = flag;
			else {
				this.IsLastFile = (flag == 0);
				this.SubEntries = -1;
			}
		}

		public bool   IsFolder   { get; private set; }
		public bool   IsLastFile { get; private set; }
		public int    SubEntries { get; private set; }
		public string Name       { get; private set; }
		public long   Offset     { get; private set; }
		public long   Size       { get; private set; }
	}
}

