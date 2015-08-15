//
//  Table.cs
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
using System.IO;
using System.Reflection;

namespace Bokuract.Scripts
{
    public static class Table
    {
        const string Filename = "Bokuract.Scripts.table.txt";
        private static readonly char[] Dictionary = new char[1024];

        static Table()
        {
            ReadTable();
        }

        private static void ReadTable()
        {
            var tableStream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(Filename);
            string[] lines = new StreamReader(tableStream).ReadToEnd().Split('\n');
            foreach (string l in lines) {
                if (l.Length != 8 || l[0] == '#')
                    continue;

                int code = int.Parse(l.Substring(0, 4));
                char ch  = l[7];
                Dictionary[code] = ch;
            }   
        }

        public static char GetChar(ushort code)
        {
            if (code > Dictionary.Length)
                throw new ArgumentOutOfRangeException();

            return Dictionary[code];
        }

        public static bool IsInRange(ushort code)
        {
            return code < Dictionary.Length;
        }
    }
}

