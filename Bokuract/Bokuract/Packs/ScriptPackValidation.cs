//
//  ScriptPackValidation.cs
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
using System.Collections.Generic;
using System.IO;
using Libgame;
using Mono.Addins;

namespace Bokuract.Packs
{
    [Extension]
    public class ScriptPackValidation : FormatValidation
    {
        public override Type FormatType {
            get { return typeof(ScriptPack); }
        }

        protected override string[] GuessDependencies(GameFile file)
        {
            return null;
        }

        protected override object[] GuessParameters(GameFile file)
        {
            return null;
        }

        protected override ValidationResult TestByData(Libgame.IO.DataStream stream)
        {
            return ValidationResult.Invalid;
        }

        protected override ValidationResult TestByRegexp(string filepath, string filename)
        {
            string dirname = Path.GetDirectoryName(filepath);
            string extension = Path.GetExtension(filename);

            if (dirname.Contains("cdimg0.img/map/gz") &&
                !dirname.EndsWith("cdimg0.img/map/gz") &&
                extension == ".bin" && filename.StartsWith("M_"))
                return ValidationResult.Sure;

            if (filename == "font.bin")
                return ValidationResult.Sure;
            
            return ValidationResult.No;
        }

        protected override ValidationResult TestByTags(IDictionary<string, object> tags)
        {
            return ((string)tags["_Device_"] == "PSP") ?
                ValidationResult.CouldBe : ValidationResult.No;
        }
    }
}

