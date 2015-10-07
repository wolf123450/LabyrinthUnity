import bpy, DungeonGen

map1 ="""
001101
010001
01101101
10101
"""

def main():
    map1 = DungeonGen.generateMaze()
    print(map1)
    sToLocs(map1)
    #print(sys.path)
    

def sToLocs(m = map1):
    rowList = m.split('\n')
    wide = max([len(x)-1 for x in rowList])
    tall = len(rowList)-1

    centerx = wide/2.0
    centery = tall/2.0

    for index, item in enumerate(rowList):
        for ind, it in enumerate(item):
            if it == "#":
                placeCube(((ind-centerx)*2.0, (-index+centery)*2.0, 0))
    


def placeCube(loc = (0,0,0)):
    bpy.ops.mesh.primitive_cube_add(view_align=False, enter_editmode=False, 
        location=loc,rotation=(0,0,0), layers =(True,False,False,False,False,False,False,False,
        False,False,False,False,False,False,False,False,False,False,False,False))

if __name__ == "__main__":
    main()
