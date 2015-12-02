using UnityEngine;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections;


public class DungeonGenerator :MonoBehaviour 
{
	public GameObject playerController;
	public GameObject MinotaurPrefab;
	public GameObject GhostPrefab;
	public GameObject RobotPrefab;
	public GameObject StairsPrefab;
	public GameObject WallPrefab;


	public DungeonGen dgen;
	public int[,] map;
	public int[,] borderedMap;
	MeshGenerator meshGen;

	void Start(){
//		
//		DungeonGen dgen = new DungeonGen ();
//		dgen.braidMaze (30,70);
//		UnityEngine.Debug.Log (dgen.printMaze ());
		dgen = new DungeonGen ();
		if (DungeonGen.width == 0){
			DungeonGen.width = 5;
			DungeonGen.height = 10;
		}

		dgen.Start(); // Don't touch any data in the job class after you called Start until IsDone is true.
	}

	void Update()
	{
		if (dgen != null)
		{
			if (dgen.Update())
			{
				// Alternative to the OnFinished callback
				map = dgen.newDungeon;
				int borderSize = 2;
				borderedMap = new int[DungeonGen.width + borderSize * 2,DungeonGen.height + borderSize * 2];

				for (int x = 0; x < borderedMap.GetLength(0); x ++) {
					for (int y = 0; y < borderedMap.GetLength(1); y ++) {
						if (x >= borderSize && x < DungeonGen.width + borderSize && y >= borderSize && y < DungeonGen.height + borderSize) {
							borderedMap[x,y] = map[x-borderSize,y-borderSize];
						}
						else {
							borderedMap[x,y] =1;
						}
					}
				}


				meshGen = GetComponent<MeshGenerator>();

				Vector3 start = new Vector3(0,-10,0);
				Vector3 end = new Vector3(0,-10,1);
				bool startPlaced = false;
				bool endPlaced = false;
				for (int x = 0; x < borderedMap.GetLength(0); x++){
					for (int y = 0; y < borderedMap.GetLength(1); y++){
						if ( borderedMap[x,y] == DungeonGen.space && dgen.rand.Next(0,100) > 50 && !startPlaced){

							start = new Vector3((x-borderedMap.GetLength(0)/2)*meshGen.squareWidth-meshGen.squareWidth,-10,(y-borderedMap.GetLength(1)/2)*meshGen.squareWidth-meshGen.squareWidth);
							UnityEngine.Debug.Log((x-(borderedMap.GetLength(0)/2))*meshGen.squareWidth);
							startPlaced = true;
						} else if ( borderedMap[x,y] == DungeonGen.space && dgen.rand.Next(0,100) > 50 && startPlaced && !endPlaced){
							
							end = new Vector3((x-borderedMap.GetLength(0)/2)*meshGen.squareWidth-meshGen.squareWidth,-11,(y-borderedMap.GetLength(1)/2)*meshGen.squareWidth-meshGen.squareWidth);

							endPlaced = true;
//							break;
						} else if (borderedMap[x,y] == DungeonGen.wall){
							Instantiate(WallPrefab, new Vector3((x-borderedMap.GetLength(0)/2)*meshGen.squareWidth-meshGen.squareWidth,-8,(y-borderedMap.GetLength(1)/2)*meshGen.squareWidth-meshGen.squareWidth), transform.rotation);
						}
					}
//					if (endPlaced)
//						break;
				}
				UnityEngine.Debug.Log(start);
//				meshGen.GenerateMesh(borderedMap);

				Destroy(Camera.main.gameObject);

				Instantiate(playerController, start, transform.rotation);

				Instantiate(StairsPrefab, end, StairsPrefab.transform.rotation);
				dgen = null;
			}
		}

	}

	void OnDrawGizmos(){
		if (map != null){
//			for (int x = 0; x < map.GetLength(0); x++){
//				for (int y = 0; y < map.GetLength(1); y++){
//					if (map[x,y] == DungeonGen.wall){
//						Gizmos.DrawCube(new Vector3(-map.GetLength(0)/2 + x,0,-map.GetLength(1)/2 + y), new Vector3(.5f,.5f,.5f));
//					}
//				}
//			}
			for (int x = 0; x < borderedMap.GetLength(0); x++){
				for (int y = 0; y < borderedMap.GetLength(1); y++){
					if ( borderedMap[x,y] == DungeonGen.space ){
						Gizmos.DrawCube(new Vector3((x-borderedMap.GetLength(0)/2)*meshGen.squareWidth-meshGen.squareWidth,-10,(y-borderedMap.GetLength(1)/2)*meshGen.squareWidth-meshGen.squareWidth), new Vector3(.5f,.5f,.5f));
					}
				}
			}

		}
	}
	
	public class ThreadedJob //No more blocking!
	{
		private bool m_IsDone = false;
		private object m_Handle = new object();
		private System.Threading.Thread m_Thread = null;
		public bool IsDone
		{
			get
			{
				bool tmp;
				lock (m_Handle)
				{
					tmp = m_IsDone;
				}
				return tmp;
			}
			set
			{
				lock (m_Handle)
				{
					m_IsDone = value;
				}
			}
		}
		
		public virtual void Start()
		{
			m_Thread = new System.Threading.Thread(Run);
			m_Thread.Start();
		}
		public virtual void Abort()
		{
			m_Thread.Abort();
		}
		
		protected virtual void ThreadFunction() { }
		
		protected virtual void OnFinished() { }
		
		public virtual bool Update()
		{
			if (IsDone)
			{
				OnFinished();
				return true;
			}
			return false;
		}
		IEnumerator WaitFor()
		{
			while(!Update())
			{
				yield return null;
			}
		}
		private void Run()
		{
			ThreadFunction();
			IsDone = true;
		}
	}
	
	public class DungeonGen : ThreadedJob{
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

		public static int width;
		public static int height;


		public static int wall = 1;
		public static int space = 0;

		public int[,] newDungeon;
		public System.Random rand;

		public DungeonGen ()
		{
			rand = new System.Random ();//(42);
		}

		protected override void ThreadFunction(){
			braidMaze (width,height);
		}

		protected override void OnFinished()
		{
			// This is executed by the Unity main thread when the job is finished
			UnityEngine.Debug.Log (printMaze ());
			
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

		public void braidMaze (int sizeX, int sizeY)
		{
			//			#x is rows, y is columns.
			List<vector2i> startSeedsArea = new List<vector2i> ();
			List<vector2i> wallList = new List<vector2i> (); //# list of walls we have made
			newDungeon = new int[sizeX,sizeY];
			for (int xw = 0; xw < sizeX; xw++) {
				for (int yw = 0; yw < sizeY; yw++) {
					newDungeon [xw,yw] = space;
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
					newDungeon [i.x,i.y] = wall;
					wallList.Add (i);
				}
			}
			UnityEngine.Debug.Log(printMaze());
			UnityEngine.Debug.Log (wallList.Count);
			wallList = wallCrawl (wallList);
			UnityEngine.Debug.Log (wallList.Count);
			wallList = wallCrawl (wallList); //Two time seems to be good for detail.
			UnityEngine.Debug.Log (wallList.Count);
			clearPoles (wallList); //TODO copyerror

		}


		public List<vector2i> wallCrawl (List<vector2i> wallList)
		{
			List<vector2i> newWalls = new List<vector2i> ();
			FloodFill f = new FloodFill ();
			vector2i start = null;
			for (int x = 0; x < newDungeon.GetLength(0); x++){
				for (int y = 0; y < newDungeon.GetLength(1); y++){
					if ( newDungeon[x,y] == space){
						start = new vector2i (x, y);
						break;
					}
				}
				if (start != null){
					break;
				}
			}
			UnityEngine.Debug.Log ("(" + start.x + " " + start.y + ")");

			foreach (vector2i i in wallList) {
				int spaces = f.reachableSpaces (newDungeon, start);
				List<vector2i> card = i.getCardinalTiles (newDungeon.GetLength(0), newDungeon.GetLength(1));
				vector2i newWall = card [rand.Next (0, card.Count)];

				if (newDungeon[newWall.x, newWall.y] == space){
					newWalls.Add (newWall);
					newDungeon [newWall.x,newWall.y] = wall;
					int newSpaces = f.reachableSpaces(newDungeon, start);

					if (spaces - newSpaces > 3){
						newWalls.Remove (newWall);
						
						newDungeon [newWall.x,newWall.y] = space;
					} else {
						spaces = newSpaces;
					}

				}

			}

			wallList.AddRange (newWalls);

			//newWalls = new List<vector2i> ();

			return wallList;
		}

		public void clearPoles (List<vector2i> wallList)
		{// #Clears out lonely walls, or "Poles".

			FloodFill f = new FloodFill();
			for (int x = 0; x < newDungeon.GetLength(0); x++){
				for (int y = 0; y < newDungeon.GetLength(1); y++){
					if (newDungeon[x,y] == wall && countAdjWall (new vector2i(x,y)) == 0){
						newDungeon [x,y] = space;
					} else if(f.reachableSpaces(newDungeon, new vector2i(x,y)) < 10) {
						newDungeon [x,y] = wall;
					}
				}
			}

//			List<vector2i> newSpaces = new List<vector2i> ();
//			foreach (vector2i i in wallList) {
//				if (countAdjWall (i) == 0) {
//					newDungeon [i.x,i.y] = space;
//					newSpaces.Add (i);
//				}
//			}
//			foreach (vector2i i in newSpaces) {
//				wallList.Remove (i);
//			}

			//return wallList;
		}

		int countAdjWall (vector2i check)
		{
			List<vector2i> adjWalls = getAdjTiles (check);
			int num = 0;
			foreach (vector2i i in adjWalls) {
				if (isInBounds (i) && newDungeon [i.x,i.y] == wall) {
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




		public string printMaze ()
		{
			string st = "";
			for (int xw = 0; xw < newDungeon.GetLength(0); xw++) {
				for (int yw = 0; yw < newDungeon.GetLength(1); yw++) {
					st += newDungeon[xw,yw] == 1 ? "#" : ".";
				}
				st += "\n";
			}
			return st;
		}
	}

	public class FloodFill
	{
		bool[,] visited;
		int[,] map;
		int count;

		Queue<vector2i> workingSet;
		
		public FloodFill ()
		{
			workingSet = new Queue<vector2i>();
		}
		
		public int reachableSpaces(int[,] dungeonMap, vector2i freeSpace){

			map = dungeonMap;
			visited = new bool[map.GetLength(0), map.GetLength(1)]; //Should be initialized to false.
			count = 0;
			recurseFill(freeSpace);
			
			return count;
		}

		void recurseFill(vector2i parent){ //DFS
			if (map[parent.x, parent.y] == DungeonGen.wall ||visited[parent.x, parent.y]){
				return;
			}
			visited [parent.x, parent.y] = true;
			count++;

			List<vector2i> neighbors = parent.getCardinalTiles (map.GetLength (0), map.GetLength (1));

			foreach(vector2i i in neighbors){
				recurseFill(i); 
			}


		}
		
	}

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
