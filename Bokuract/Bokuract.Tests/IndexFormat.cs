//
//  Test.cs
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
using NUnit.Framework;
using System;
using Bokuract;

namespace Bokuract.Tests
{
	[TestFixture]
	public class IndexFormat
	{
		[Test]
		public void GatherIsFolder()
		{
			CdIndexEntry entry = new CdIndexEntry(1, 0, "", 0, 0);
			Assert.IsTrue(entry.IsFolder);
		}

		[Test]
		public void GatherIsFile()
		{
			CdIndexEntry entry = new CdIndexEntry(0, 0, "", 0, 0);
			Assert.IsFalse(entry.IsFolder);
		}

		[Test]
		public void GatherNumEntries()
		{
			CdIndexEntry entry = new CdIndexEntry(1, 3, "", 0, 0);
			Assert.AreEqual(3, entry.SubEntries);
		}

		[Test]
		public void GatherIsLastFileFalse()
		{
			CdIndexEntry entry = new CdIndexEntry(0, 1, "", 0, 0);
			Assert.AreEqual(-1, entry.SubEntries);
			Assert.IsFalse(entry.IsLastFile);
		}

		[Test]
		public void GahterIsLastFileTrue()
		{
			CdIndexEntry entry = new CdIndexEntry(0, 0, "", 0, 0);
			Assert.AreEqual(-1, entry.SubEntries);
			Assert.IsTrue(entry.IsLastFile);
		}

		[Test]
		public void GatherName()
		{
			CdIndexEntry entry = new CdIndexEntry(0, 0, "test", 0, 0);
			Assert.AreEqual("test", entry.Name);
		}

		[Test]
		public void GatherOffset()
		{
			CdIndexEntry entry = new CdIndexEntry(0, 0, "", 0x10, 0);
			Assert.AreEqual(0x10 * 0x800, entry.Offset);
		}

		[Test]
		public void GatherSize()
		{
			CdIndexEntry entry = new CdIndexEntry(0, 0, "", 0, 0xCAFE);
			Assert.AreEqual(0xCAFE, entry.Size);
		}
	}
}

