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
using Libgame.IO;
using System.IO;
using System.Linq;
using Mono.Addins;
using System.Collections.Generic;

namespace Bokuract
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			// First args is the path input & output
			if (args.Length == 0)
				return;

			// Create system folder
			GameFolder root = GameFolderFactory.FromPath(args[0], "root");
			root.AssignTagsRecursive(new Dictionary<string, object>() { {"_Device_", "PSP"} });

			// Initialize file manager
			FileManager.Initialize(root, new FileInfoCollection());
			FileManager manager = FileManager.GetInstance();

			// Gets file and read it
			GameFile file = manager.RescueFile("/root/cdimg0.img");
			file.Format.Read();

			// Extract files
			ReadAll(root);
			ExtractFolder(args[0], root);
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

		private static void ExtractFolder(string outputDir, FileContainer folder)
		{
			string folderDir = Path.Combine(outputDir, folder.Name);
			Directory.CreateDirectory(folderDir);

			foreach (GameFile subfile in folder.Files) {
				if (subfile.Files.Count > 1 || subfile.Folders.Count > 0) {
					ExtractFolder(folderDir, subfile);
				} else if (subfile.Files.Count == 1) {
					GameFile f = subfile.Files.First() as GameFile;
					f.Stream.WriteTo(Path.Combine(folderDir, f.Name));
				} else {
					subfile.Stream.WriteTo(Path.Combine(folderDir, subfile.Name));
				}
			}

			foreach (GameFolder subfolder in folder.Folders)
				ExtractFolder(folderDir, subfolder);
		}
	}
}
