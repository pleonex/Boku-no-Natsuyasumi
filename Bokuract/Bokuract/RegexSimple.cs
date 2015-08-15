//
//  RegexSimple.cs
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

namespace Bokuract
{
    public class RegexSimple
    {
        public string Expression {
            get;
            private set;
        }

        public RegexSimpleMode Mode {
            get;
            private set;
        }

        public RegexSimple(string expression, RegexSimpleMode mode)
        {
            this.Expression = expression;
            this.Mode = mode;
        }

        public RegexSimple(string regex)
        {
            Build(regex);
        }

        public static bool Match(string expression, RegexSimpleMode mode, string text)
        {
            return new RegexSimple(expression, mode).Match(text);
        }

        public bool Match(string text)
        {
            int idx;
            switch (Mode) {
            case RegexSimpleMode.Is:
                return text == Expression;

            case RegexSimpleMode.StartsWith:
                return text.StartsWith(Expression);

            case RegexSimpleMode.EndsWith:
                return text.EndsWith(Expression);

            case RegexSimpleMode.Contains:
                return text.Contains(Expression);

            case RegexSimpleMode.ContainsButNoStartsWith:
                idx = text.IndexOf(Expression);
                return idx != -1 && idx != 0;

            case RegexSimpleMode.ContainsButNoEndsWith:
                idx = text.IndexOf(Expression);
                return idx != -1 && idx != (text.Length - Expression.Length);
            }

            return false;
        }

        private void Build(string regex)
        {
            int firstWildcard  = regex.IndexOf('*');
            int secondWildcard = regex.IndexOf('*', firstWildcard + 1);

            if (firstWildcard != -1 && secondWildcard != -1) {
                int wildcardDistance = secondWildcard - (firstWildcard + 1);
                Expression = regex.Substring(firstWildcard + 1, wildcardDistance);
                Mode = RegexSimpleMode.Contains;
            } else if (firstWildcard != -1) {
                Expression = regex.Substring(firstWildcard + 1);
                Mode = RegexSimpleMode.EndsWith;
            } else if (secondWildcard != -1) {
                Expression = regex.Substring(0, secondWildcard);
                Mode = RegexSimpleMode.StartsWith;
            } else {
                Expression = regex;
                Mode = RegexSimpleMode.Is;
            }
        }
    }
}

