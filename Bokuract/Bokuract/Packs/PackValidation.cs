//
//  PackValidation.cs
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
using System.IO;
using Libgame;
using Libgame.IO;
using Mono.Addins;

namespace Bokuract.Packs
{
    [Extension]
    public class PackValidation : FormatValidation
    {
        private static readonly RegexSimple[,] SupportedFilesWithSubName = {
            { BuildRe("*/cdimg0.img/map/gz"),   null,                 null },
            { null,                             BuildRe("startup"),   BuildRe(".bin") },
            { null,                             BuildRe("full_pack"), BuildRe(".bin") },
            { null,                             BuildRe("all_tex"),   BuildRe(".bin") },
            { null,                             BuildRe("jumbo"),     BuildRe(".bin") },
            { null,                             null,                 BuildRe(".mrg") },
            { null,                             BuildRe("saveload"),  BuildRe(".bin") },
            { null,                             BuildRe("day"),       BuildRe(".bin") },
        };

        private static readonly RegexSimple[,] SupportedFilesNoSubName = {
            { BuildRe("*/cdimg0.img/map/gz/*"), BuildRe("M_*"),       BuildRe(".bin") },
            { null,                             BuildRe("font"),      BuildRe(".bin") }
        };

        private static readonly Dictionary<string, bool> CachedFiles = 
            new Dictionary<string, bool>();

        public override Type FormatType {
            get { return typeof(Pack); }
        }

        protected override string[] GuessDependencies(GameFile file)
        {
            return null;
        }

        protected override object[] GuessParameters(GameFile file)
        {
            return new object[] { CachedFiles[file.Path] };
        }

        protected override ValidationResult TestByData(DataStream stream)
        {
            return ValidationResult.Invalid;
        }

        protected override ValidationResult TestByRegexp(string filepath, string filename)
        {
            string dirname   = Path.GetDirectoryName(filepath);
            string extension = Path.GetExtension(filename);
            filename = Path.GetFileNameWithoutExtension(filename);

            ValidationResult result = TestRegex(SupportedFilesWithSubName,
                dirname, filename, extension);
            
            if (result == ValidationResult.Sure) {
                CachedFiles[filepath] = true;
            } else {
                result = TestRegex(SupportedFilesNoSubName,
                    dirname, filename, extension);

                if (result == ValidationResult.Sure)
                    CachedFiles[filepath] = false;
            }
           
            return result;
        }

        private static ValidationResult TestRegex(RegexSimple[,] regex, string dirname,
            string filename, string extension)
        {
            ValidationResult result = ValidationResult.No;
            for (int i = 0; i < regex.GetLength(0) && result == ValidationResult.No; i++){
                result = ValidationResult.Sure;

                if (regex[i, 0] != null)
                    result = regex[i, 0].Match(dirname) ?
                        ValidationResult.Sure : ValidationResult.No;

                if (regex[i, 1] != null && result != ValidationResult.No)
                    result = regex[i, 1].Match(filename) ?
                        ValidationResult.Sure : ValidationResult.No;

                if (regex[i, 2] != null && result != ValidationResult.No)
                    result = regex[i, 2].Match(extension) ?
                        ValidationResult.Sure : ValidationResult.No;
            }

            return result;
        }

        protected override ValidationResult TestByTags(IDictionary<string, object> tags)
        {
            return ((string)tags["_Device_"] == "PSP") ?
                ValidationResult.CouldBe : ValidationResult.No;
        }

        private static RegexSimple BuildRe(string regex)
        {
            return new RegexSimple(regex);   
        }
    }
}

