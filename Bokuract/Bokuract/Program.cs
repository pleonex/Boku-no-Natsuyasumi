//
//  Program.cs
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
using Libgame;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace Bokuract
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            // First args is the path input & output folder
            if (args.Length == 0)
                return;

            Stopwatch watch = Stopwatch.StartNew();

            string folder = args[0];

            // Create system folder
            GameFolder root = GameFolderFactory.FromPath(folder, "root");
            root.AssignTagsRecursive(
                new Dictionary<string, object> { {"_Device_", "PSP"} });

            // Initialize file manager
            FileManager.Initialize(root, new FileInfoCollection());
            FileManager manager = FileManager.GetInstance();

            // Gets file and read it
            GameFile file = manager.RescueFile("/root/cdimg0.img");
            file.Format.Read();

            // Extract files
            Console.WriteLine("Reading files...");
            ReadAll(root);

            Console.WriteLine("Writing files...");
            ExtractFolder(folder, root);

            watch.Stop();
            Console.WriteLine("Done! It took: {0}", watch.Elapsed);
        }

        private static void ReadAll(FileContainer container)
        {
            foreach (GameFile subfile in container.Files) {
                if (subfile.Format == null)
                    FileManager.AssignBestFormat(subfile);

                if (subfile.Format != null)
                    subfile.Format.Read();

                if (subfile.Files.Count > 0 || subfile.Folders.Count > 0)
                    ReadAll(subfile);
            }

            foreach (GameFolder subfolder in container.Folders)
                ReadAll(subfolder);
        }

        private static void ExtractFolder(string output, FileContainer folder)
        {
            string folderDir = Path.Combine(output, folder.Name);
            Directory.CreateDirectory(folderDir);

            foreach (GameFile subfile in folder.Files) {
                if (subfile.Files.Count > 0 || subfile.Folders.Count > 0) {
                    ExtractFolder(folderDir, subfile);
                } else if (subfile.Length > 0) {
                    string filepath = Path.Combine(folderDir, subfile.Name);
                    subfile.Stream.WriteTo(filepath);
                }
            }

            foreach (GameFolder subfolder in folder.Folders)
                ExtractFolder(folderDir, subfolder);
        }
    }
}
