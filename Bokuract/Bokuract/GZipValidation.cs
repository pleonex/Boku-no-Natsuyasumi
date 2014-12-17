//
//  GZipValidation.cs
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
using Mono.Addins;
using Libgame;
using System.IO;
using System.Collections.Generic;
using Libgame.IO;

namespace Bokuract
{
	[Extension]
	public class GZipValidation : FormatValidation
	{
		public override Type FormatType {
			get { return typeof(GZip); }
		}

		protected override string[] GuessDependencies(GameFile file)
		{
			return null;
		}

		protected override object[] GuessParameters(GameFile file)
		{
			return null;
		}

		protected override ValidationResult TestByData(DataStream stream)
		{
			stream.Seek(4, SeekMode.Origin);
			return (new DataReader(stream).ReadUInt16() == 0x8b1f) ?
				ValidationResult.CouldBe : ValidationResult.No;
		}

		protected override ValidationResult TestByRegexp(string filepath, string filename)
		{
			return (filename.EndsWith(".gzx") || filename.EndsWith(".gz")) ?
				ValidationResult.Sure : ValidationResult.No;
		}

		protected override ValidationResult TestByTags(IDictionary<string, object> tags)
		{
			return ((string)tags["_Device_"] == "PSP") ? ValidationResult.CouldBe : ValidationResult.No;
		}
	}
}

