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

namespace Bokuract.Packs
{
	public struct CdIndexEntry
	{
		public static int EntrySize { get { return 0x10; } }

		public bool   IsFolder   { get; set; }
		public bool   IsLastFile { get; set; }
		public int    SubEntries { get; set; }
		public string Name       { get; set; }
		public long   Offset     { get; set; }
		public long   Size       { get; set; }
	}
}

