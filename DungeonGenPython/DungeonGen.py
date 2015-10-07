'''
 <Theseus, a RLG set in Daedalus's Labyrinth> Copyright (C) <2010>
 <Clinton Day>

 This program is free software: you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation, either version 3 of the License, or
 (at your option) any later version.

 This program is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public License
 along with this program.  If not, see <http://www.gnu.org/licenses/>.
 '''

import random, time, sys

newDungeon = [] #The dungeon to work with while creating. (List of Lists)
RAND_RANGE = 100 #The probability variation for creating new wall seeds.

RAND_DENSITY = 25# The cutoff for probability. Random numbers generated higher than this
# will create new wall seeds.
WALL_DENSITY = 2 #Wall density, or how many walls a wall can be connected to.
DURATION = 100.0 #How long to let an algorithm run, in milliseconds.
DEAD_END = 2 #How many spaces must be open to qualify as a passable space.
staircaseSpacing = 30 # The distance between staircases.
spaceList = set([]) #The Set of spaces in the map.

#Syntactic sugar
x = 0
y = 1

wallChar = "#"
spaceChar = "."
obstacles = [wallChar]

justWalls = True  #Whether to start growing walls from just the edges of the map.
    
## To create a Maze without dead ends, basically add wall segments
## throughout the Maze at random, but ensure that each new segment added
## will not cause a dead end to be made. I make them with four steps:
## (1) Start with the outer wall, (2) Loop through the Maze and add
## single wall segments touching each wall vertex to ensure there are no
## open rooms or small "pole" walls in the Maze, (3) Loop over all
## possible wall segments in random order, adding a wall there if it
## wouldn't cause a dead end, (4) Either run the isolation remover
## utility at the end to make a legal Maze that has a solution, or be
## smarter in step three and make sure a wall is only added if it also
## wouldn't cause an isolated section.
## I didn't do this at all... But the result is the same.

def braidMaze(sizeX = 25, sizeY = 50):
    #x is rows, y is columns.
    global wallList, spaceList, newDungeon, x, y
    newDungeon = []
    startSeedsArea = []
    wallList = [] # list of walls we have made
    #spaceList = [] # list of spaces
    for xw in range(sizeX):
        newDungeon.append([])
        for yw in range(sizeY):
            newDungeon[xw].append(spaceChar)

    if justWalls:
        for xw in range(sizeX):
            for yw in (0, sizeY-1):
                startSeedsArea.append((xw, yw))
        for yw in range(1, sizeY-1):
            for xw in (0, sizeX-1):
                startSeedsArea.append((xw, yw))
    else:
        for xw in range(sizeX):
            for yw in range(sizeY):
                startSeedsArea.append((xw, yw))

    for i in startSeedsArea:
        if (random.randint(0, RAND_RANGE) < RAND_DENSITY and i
                and countAdjWall(i, newDungeon) == 0):
            newDungeon[i[x]][i[y]] = wallChar
            wallList.append(i)

    wallList = wallCrawl(wallList)
    
##    for xw in range(sizeX):
##        for yw in range(sizeY):
##            spaceList.append(newDungeon.getTile(xw, yw))

    #spaceList.retainAll(wallList);


def wallCrawl(wallList = []):
    global newDungeon
    newWalls = []
    t = time.time()*1000
    while (time.time()*1000 - t < DURATION):
        for i in wallList:
            newWall = random.choice(getCardinalTiles(i, newDungeon))
            if (isInBounds(newWall, newDungeon)):
                if (not newWall in wallList
                    and countAdjWall(newWall, newDungeon) <= WALL_DENSITY):

                    newWalls.append(newWall)
                    newDungeon[newWall[x]][newWall[y]] = wallChar
                        
                for j in getAdjTiles(newWall, newDungeon):
                    if (isInBounds(j, newDungeon)
                        and newDungeon[j[x]][j[y]] == spaceChar
                        and checkDeadEnd(j, newDungeon)):
                        newWalls.remove(newWall)
                        newDungeon[newWall[x]][newWall[y]] = spaceChar
        wallList = clearPoles(wallList)

        wallList.extend(newWalls)
        newWalls = []
    return wallList

def clearPoles(wallList): #Clears out lonely walls, or "Poles".
    global newDungeon
    newSpaces = []
    for i in wallList:
        if (countAdjWall(i, newDungeon) == 0):
            newDungeon[i[x]][i[y]] = spaceChar
            newSpaces.append(i)
    for i in newSpaces:
        wallList.remove(i)
    return wallList


def checkDeadEnd(toCheck, inMap):
    return len(getCardinalTiles(toCheck, inMap)) < DEAD_END

    
def countAdjWall(check, checkMap):
    global x,y
    adjWalls = getAdjTiles(check, map)
    num = 0
    for i in adjWalls:
        if (isInBounds(i, checkMap) and checkMap[i[x]][i[y]] == wallChar):
            num += 1;
    return num
    

def isInBounds(coord, cMap):
    global x,y
    
    if coord[x] < len(cMap) and coord[x] >= 0:
        if coord[y] < len(cMap[0]) and coord[y] >= 0:
            return True
    #print "coord", coord
    return False

def getAdjTiles(check, checkMap):
    adjTiles = []
    adjTiles.append((check[x] - 1, check[y] + 1))
    adjTiles.append((check[x] - 1, check[y]))
    adjTiles.append((check[x] - 1, check[y] - 1))
    adjTiles.append((check[x], check[y] + 1))
    adjTiles.append((check[x], check[y] - 1))
    adjTiles.append((check[x] + 1, check[y] + 1))
    adjTiles.append((check[x] + 1, check[y]))
    adjTiles.append((check[x] + 1, check[y] - 1))

    return adjTiles


## Gets the passable Tiles in the cardinal directions. (NSEW).
def getCardinalTiles(check, checkMap):
    global x, y
    #x, y = (0,1)
    if not check:
        return
    north = (check[x], check[y] + 1)
    east = (check[x] + 1 , check[y])
    south = (check[x], check[y] - 1)
    west = (check[x]-1, check[y])
    adjWalls = [north,south,east,west]
    remove = []

    for i in adjWalls:
        #print i, x, y
        if (isInBounds(i,checkMap) and checkMap[i[x]][i[y]] == wallChar):
            remove.append(i)
    for i in remove:
        adjWalls.remove(i);

    return adjWalls

def generateMaze(seed = 42, height = 25, width = 50):
    random.seed(seed)
    braidMaze(height, width)
    return newDungeon

def printMaze(dun):
    st = ""
    for xw in dun:
        for yw in xw:
            st += yw
        st += "\n"

    return st

def main():
    if (len (sys.argv) > 1):
        print (printMaze(generateMaze((int)(sys.argv[1]))))
    else:
        print (printMaze(generateMaze()))


if __name__ == "__main__":
    main()
    
