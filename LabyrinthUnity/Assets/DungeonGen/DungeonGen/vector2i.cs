using System;
using System.Collections.Generic;

namespace DungeonGen
{
	public class vector2i : IComparable //Make comparable TODO
	{

		public int CompareTo(object obj){
			if (obj == null) return 1;
			
			vector2i otherTemperature = obj as vector2i;
			if (otherTemperature != null) 
				return this.x.CompareTo(otherTemperature.x) == 0? this.y.CompareTo(otherTemperature.y) : this.x.CompareTo(otherTemperature.x);
			else
				throw new ArgumentException("Object is not a vector2i");
		}

		public int x { get; set; }

		public int y { get; set; }

		public vector2i (int _x, int _y)
		{
			this.x = _x;
			this.y = _y;
		}

		public List<vector2i> getCardinalTiles (int boundsX = 0, int boundsY = 0)
		{
			vector2i north = new vector2i (x,y + 1);
			vector2i east = new vector2i (x + 1, y);
			vector2i south = new vector2i (x, y - 1);
			vector2i west = new vector2i (x - 1, y);
			List<vector2i> adjWalls = new List<vector2i>{ north, south, east, west };

			adjWalls.RemoveAll(i => !i.isInBounds(boundsX, boundsY));

			return adjWalls;
		}

		public bool isInBounds (int boundsX = 0, int boundsY = 0)
		{
			if (x < boundsX && x >= 0) {
				if (y < boundsY && y >= 0) {
					return true;
				}
				//						#print "coord", coord
			}
			return false;
		}

	}
}

