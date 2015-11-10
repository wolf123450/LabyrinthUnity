using System;
using System.Collections.Generic;

namespace DungeonGen
{
	public class FloodFill
	{
		bool[,] visited;
		char[,] map;
		//List<vector2i> workingSet;
		HashSet<vector2i> workingSet;
		int count = 0;

		public FloodFill ()
		{
			//workingSet = new List<vector2i> ();
			workingSet = new HashSet<vector2i> ();
		}

		public int reachableSpaces(char[,] dungeonMap, vector2i freeSpace){
			map = dungeonMap;
			visited = new bool[map.GetLength(0), map.GetLength(1)]; //Should be initialized to false.
			count = 0;
			workingSet.Add (freeSpace);

			do {
				HashSet<vector2i>.Enumerator en = workingSet.GetEnumerator();
				vector2i tile = en.Current;
				en.Dispose();
				workingSet.Remove (tile);
				visited [tile.x, tile.y] = true;
				count++;
				List<vector2i> neighbors = tile.getCardinalTiles (map.GetLength (0), map.GetLength (1));
				neighbors.RemoveAll (x => map [x.x, x.y] == DungeonGenerator.wallChar);
				neighbors.RemoveAll (x => visited [x.x, x.y] == true);
//				workingSet.AddRange(neighbors); //Check duplicates.
				workingSet.UnionWith(neighbors);
			} while(workingSet.Count > 0);

			return count;
		}

		void recurseFill(vector2i tile){



		}

	}
}

