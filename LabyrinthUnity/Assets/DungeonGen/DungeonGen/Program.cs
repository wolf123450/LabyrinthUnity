//		'''
//		<Theseus, a RLG set in Daedalus's Labyrinth> Copyright (C) <2010>
//		<Clinton Day>
//
//		This program is free software: you can redistribute it and/or modify
//		it under the terms of the GNU General Public License as published by
//		the Free Software Foundation, either version 3 of the License, or
//		(at your option) any later version.
//
//		This program is distributed in the hope that it will be useful,
//		but WITHOUT ANY WARRANTY; without even the implied warranty of
//		MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//		GNU General Public License for more details.
//
//		You should have received a copy of the GNU General Public License
//		along with this program.  If not, see <http://www.gnu.org/licenses/>.
//		'''

using System;

namespace DungeonGen
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello World!");

			DungeonGenerator dgen = new DungeonGenerator ();
			dgen.braidMaze (30,70);
			Console.WriteLine (dgen.printMaze ());
		}

	}
		
}