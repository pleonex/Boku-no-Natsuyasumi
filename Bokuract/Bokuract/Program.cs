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
			if (args.Length != 1)
				return;

			// Create system folder
			GameFolder root = GameFolderFactory.FromPath(args[0], "boku1");
			root.AssignTagsRecursive(new Dictionary<string, object>() { {"_Device_", "PSP"} });

			// Initialize file manager
			FileManager.Initialize(root, new FileInfoCollection());

			// Gets file and read it
			GameFile file = FileManager.GetInstance().RescueFile("/boku1/cdimg0.img");
			file.Format.Read();

			// Extract files
			ExtractFolder(args[0], (GameFolder)file.Folders.First());
		}

		private static void ExtractFolder(string outputDir, GameFolder folder)
		{
			string folderDir = Path.Combine(outputDir, folder.Name);
			Directory.CreateDirectory(folderDir);

			foreach (GameFile file in folder.Files)
				file.Stream.WriteTo(Path.Combine(folderDir, file.Name));

			foreach (GameFolder subfolder in folder.Folders)
				ExtractFolder(folderDir, subfolder);
		}
	}
}
