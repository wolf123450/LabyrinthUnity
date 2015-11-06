using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DungeonGen
{
	class DungeonGenerator
	{
		int RAND_RANGE = 100;
		//#The probability variation for creating new wall seeds.

		int RAND_DENSITY = 25;
		//# The cutoff for probability. Random numbers generated higher than this
		//# will create new wall seeds.
		int WALL_DENSITY = 3;
		// #Wall density, or how many walls a wall can be connected to.
		//		int DURATION = 5;
		//#How long to let an algorithm run, in milliseconds.
		int DEAD_END = 2;


		public static char wallChar = '#';
		public static char spaceChar = '.';

		char[,] newDungeon;
		List<char> obstacles;
		Random rand;

		public DungeonGenerator ()
		{
			obstacles = new List<char> ();
			obstacles.Add (wallChar);
			rand = new Random (42);

		}



		//			## To create a Maze without dead ends, basically add wall segments
		//			## throughout the Maze at random, but ensure that each new segment added
		//			## will not cause a dead end to be made. I make them with four steps:
		//			## (1) Start with the outer wall, (2) Loop through the Maze and add
		//			## single wall segments touching each wall vertex to ensure there are no
		//			## open rooms or small "pole" walls in the Maze, (3) Loop over all
		//			## possible wall segments in random order, adding a wall there if it
		//			## wouldn't cause a dead end, (4) Either run the isolation remover
		//			## utility at the end to make a legal Maze that has a solution, or be
		//			## smarter in step three and make sure a wall is only added if it also
		//			## wouldn't cause an isolated section.
		//			## I didn't do this at all... But the result is the same.

		public void braidMaze (int sizeX = 25, int sizeY = 50)
		{
			//			#x is rows, y is columns.
			List<vector2i> startSeedsArea = new List<vector2i> ();
			List<vector2i> wallList = new List<vector2i> (); //# list of walls we have made
			newDungeon = new char[sizeX,sizeY];
			for (int xw = 0; xw < sizeX; xw++) {
				for (int yw = 0; yw < sizeY; yw++) {
					newDungeon [xw,yw] = spaceChar;
				}
			}


			for (int xw = 0; xw < sizeX; xw++) {
				for (int yw = 0; yw < sizeY; yw++) {
					startSeedsArea.Add (new vector2i (xw, yw));
				}
			}

			int c = 0;

			foreach (vector2i i in startSeedsArea) {
				c++;
				if (rand.Next (0, RAND_RANGE) < RAND_DENSITY && i != null && countAdjWall (i) == 0) {
					newDungeon [i.x,i.y] = wallChar;
					wallList.Add (i);
				}
			}
			wallList = wallCrawl (wallList);
			wallList = wallCrawl (wallList); //Two time seems to be good for detail.

		}


		public List<vector2i> wallCrawl (List<vector2i> wallList)
		{
			List<vector2i> newWalls = new List<vector2i> ();
			FloodFill f = new FloodFill ();
			vector2i start = new vector2i (0, 0);
			foreach (vector2i i in wallList) {
				int spaces = f.reachableSpaces (newDungeon, start);
				List<vector2i> card = i.getCardinalTiles (newDungeon.GetLength(0), newDungeon.GetLength(1));
				vector2i newWall = card [rand.Next (0, card.Count)];
				if (isInBounds (newWall)) {
					if (!wallList.Contains (newWall)
						&& countAdjWall (newWall) <= WALL_DENSITY) {

						newWalls.Add (newWall);
						newDungeon [newWall.x,newWall.y] = wallChar;
						if (spaces < f.reachableSpaces(newDungeon, start)-1){
							newWalls.Remove (newWall);
							
							newDungeon [newWall.x,newWall.y] = spaceChar;
						}
					}

//					foreach (vector2i j in getAdjTiles(newWall)) {
//						if (isInBounds (j) && newDungeon [j.x,j.y] == spaceChar && spaces < f.reachableSpaces(newDungeon, start)-1) {
//							newWalls.Remove (newWall);
//
//							newDungeon [newWall.x,newWall.y] = spaceChar;
//						}
//					}
				}
			}

			clearPoles (wallList); //TODO copyerror

			wallList.AddRange (newWalls);

			//newWalls = new List<vector2i> ();

			return wallList;
		}

		public void clearPoles (List<vector2i> wallList)
		{// #Clears out lonely walls, or "Poles".
			List<vector2i> newSpaces = new List<vector2i> ();
			foreach (vector2i i in wallList) {
				if (countAdjWall (i) == 0) {
					newDungeon [i.x,i.y] = spaceChar;
					newSpaces.Add (i);
				}
			}
			foreach (vector2i i in newSpaces) {
				wallList.Remove (i);
			}

			//return wallList;
		}


		bool checkDeadEnd (vector2i toCheck)
		{
			return getCardinalTiles (toCheck).Count < DEAD_END;
		}


		int countAdjWall (vector2i check)
		{
			List<vector2i> adjWalls = getAdjTiles (check);
			int num = 0;
			foreach (vector2i i in adjWalls) {
				if (isInBounds (i) && newDungeon [i.x,i.y] == wallChar) {
					num += 1;
				}
			}
			return num;
		}


		public bool isInBounds (vector2i coord)
		{
			if (coord.x < newDungeon.GetLength(0) && coord.x >= 0) {
				if (coord.y < newDungeon.GetLength(1) && coord.y >= 0) {
					return true;
				}
				//						#print "coord", coord
			}
			return false;
		}

		List<vector2i> getAdjTiles (vector2i check)
		{
			List<vector2i> adjTiles = new List<vector2i> ();

			adjTiles.Add (new vector2i (check.x - 1, check.y + 1));
			adjTiles.Add (new vector2i (check.x - 1, check.y));
			adjTiles.Add (new vector2i (check.x - 1, check.y - 1));
			adjTiles.Add (new vector2i (check.x, check.y + 1));
			adjTiles.Add (new vector2i (check.x, check.y - 1));
			adjTiles.Add (new vector2i (check.x + 1, check.y + 1));
			adjTiles.Add (new vector2i (check.x + 1, check.y));
			adjTiles.Add (new vector2i (check.x + 1, check.y - 1));

			return adjTiles;
		}


		//									## Gets the passable Tiles in the cardinal directions. (NSEW).
		public List<vector2i> getCardinalTiles (vector2i check)
		{
			if (check == null) {
				return null;
			}
			vector2i north = new vector2i (check.x, check.y + 1);
			vector2i east = new vector2i (check.x + 1, check.y);
			vector2i south = new vector2i (check.x, check.y - 1);
			vector2i west = new vector2i (check.x - 1, check.y);
			List<vector2i> adjWalls = new List<vector2i>{ north, south, east, west };
			List<vector2i> remove = new List<vector2i> ();

			foreach (vector2i i in adjWalls) {
				//				#print i, x, y
				if (isInBounds (i) && newDungeon [i.x,i.y] == wallChar) {
					remove.Add (i);
				}
			}
			foreach (vector2i i in remove) {
				adjWalls.Remove (i);
			}

			return adjWalls;
		}

		public string printMaze ()
		{
			string st = "";
			for (int xw = 0; xw < newDungeon.GetLength(0); xw++) {
				for (int yw = 0; yw < newDungeon.GetLength(1); yw++) {
					st += newDungeon[xw,yw];
				}
				st += "\n";
			}
			return st;
		}
	}
}

