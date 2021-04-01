// Copyright (c) 2021 Benito Palacios Sánchez
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
namespace Bokuract.Packs
{
    using System.Linq;
    using System.Text.RegularExpressions;

    public static class PackValidation
    {
        private static readonly Regex[] SupportedFilesWithSubName = {
            Re(@".*/cdimg0\.img/map/models/day.*/day.*\.bin$"),
            Re(@".*/startup\.bin$"),
            Re(@".*/full_pack\.bin$"),
            Re(@".*/all_tex\.bin$"),
            Re(@".*/jumbo\.bin$"),
            Re(@".*\.mrg$"),
            Re(@".*/saveload\.bin$"),
            Re(@".*/day\.bin$"),
            Re(@".*\.pack$"),
            Re(@".*\.pack_$"),
            Re(@".*/model_pack\.bin$"),
            Re(@".*\.mpk$"),
            Re(@".*/cdimg0\.img/map/models/se/smap\.bin$"),
            Re(@".*/map/models/test\.pack/test\.mdl$"),
            Re(@".*/map/models/system/saveload_\..*\.bin$"),
        };

        private static readonly Regex[] SupportedFilesNoSubName = {
            Re(@".*/font\.bin$"),
            Re(@".*/cdimg0\.img/map/models/animlist\.bin$"),
            Re(@".*/cdimg0\.img/map/models/title/title\.bin$"),
            Re(@".*/cdimg0\.img/map/models/title/memory\.bin$"),
            Re(@".*/models/title/config\.bin\.gzx/config\.bin$"),
        };

        public static bool IsPackFile(string path, out bool hasName)
        {
            hasName = false;
            bool result = SupportedFilesWithSubName.Any(r => r.Match(path).Success);
            if (result) {
                hasName = true;
                return true;
            }

            result = SupportedFilesNoSubName.Any(r => r.Match(path).Success);
            if (result) {
                hasName = false;
                return true;
            }

            return false;
        }

        private static Regex Re(string regex) => new Regex(regex);
    }
}
