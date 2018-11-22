using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates a random tile based corridor
/// </summary>
public class CorridorSectionGenerator : MonoBehaviour
{

    public enum TileState
    {
        Open, Wall, Border                      //Enum to represent the three tile types
    }

    CorridorSection currentCorridorSection;     //Store the current corridor section

    //Prefabs
    public GameObject openTilePrefab;           //Open Tile Prefab
    public GameObject borderTilePrefab;         //Border Tile Prefab

    //Corridor Generation Variables
    public int maxSectionHeight;                //Maximum tile hight a corridor section can be
    public int sectionWidth;                    //Maximum width a corridor section can be
    public int sectionCount = 0;                //Tracks the number of sections that have been created

    //Data used to position next section
    public float corridorSectionYPosition;      //Position.Y in world space of the previous corridor section
    int previousSectionHeight;                  //Tile height of the previous corridor section
    int lastCorridorExitCoordinateX;            //Position.X (of the tile coordinate) of the tile that was the "exit" of the last section

    private void Start()
    {
        corridorSectionYPosition = -5f;                                                                         //Initialize at -5f as the first corridor has a fixed height of 5

        currentCorridorSection = new CorridorSection(sectionWidth, 5, Mathf.RoundToInt(sectionWidth / 2f));     //Sets currentCorridorSection to a newly created CorridorSection
        FillCorridorSection(currentCorridorSection);                                                            //Call FillCorridorSection on the currentCorridorSection

        lastCorridorExitCoordinateX = currentCorridorSection.exitCoordinateX;                                   //Store the exit position of the corridor we just filled 

        NewCorridors(4);                                                                                        //Create four additional corridors
    }

    //Calls the CreateNewCorridor method a set number of times
    public void NewCorridors(int count)
    {
        for (int i = 0; i < count; i++)
        {
            CreateNewCorridorSection();
        }
    }


    void CreateNewCorridorSection()
    {
        currentCorridorSection = new CorridorSection(sectionWidth, Random.Range(2, maxSectionHeight), lastCorridorExitCoordinateX);     //Sets currentCorridorSection to a newly created CorridorSection
        FillCorridorSection(currentCorridorSection);
        lastCorridorExitCoordinateX = currentCorridorSection.exitCoordinateX;                                                           //Store the exit position of the corridor we just filled 

        //NOT HAPPY WITH THIS SYSYEM
        if (sectionCount >= 5)
        {
            maxSectionHeight--;
            sectionCount = 0;
        }

        if (maxSectionHeight < 2)
            maxSectionHeight = 2;

        sectionCount++;                                                                                                                 //Increment section count by one
    }

    //Takes a CorridorSection and instantiates that sections tiles based on the flags
    private void FillCorridorSection(CorridorSection current)
    {
        //First create a SectionContainer GameObject to hold all the tiles
        GameObject sectionContainer = new GameObject("Corridor Section Container");     //Create the object
        sectionContainer.tag = "SectionContainer";                                      //Set its tag        
        BoxCollider2D collider = sectionContainer.AddComponent<BoxCollider2D>();        //Give it a collider
        collider.size = new Vector2(sectionWidth, current.height);                      //Size the collider to cover the whole section
        collider.isTrigger = true;                                                      //Mark the collider as a trigger
        sectionContainer.AddComponent<CorridorSectionContainer>();                      //Add the CorridorSectionContainer script

        //Then create all the tiles
        for (int x = 0; x < current.flags.GetLength(0); x++)
        {
            for (int y = 0; y < current.flags.GetLength(1); y++)
            {
                if (TileType(current.flags[x, y]) != null)                                                                          //Check what type of tile needs to be created
                {
                    GameObject newTile = Instantiate(TileType(current.flags[x, y]), sectionContainer.transform);                    //Create the tile
                    newTile.transform.localPosition = CorridorSectionCoordinateToPosition(x, y, sectionWidth, current.height);      //Set the tiles position
                }
            }
        }

        sectionContainer.transform.position = new Vector2(0.0f, corridorSectionYPosition + (current.height / 2f + previousSectionHeight / 2f));     //Set this corridor section to be exactly on top of the previous section

        //Update values for the next section
        corridorSectionYPosition = sectionContainer.transform.position.y;
        previousSectionHeight = current.height;
    }

    //Creates a Vector3 position based on a coordinate (2D Array Index), the positions are relative to the corridor section parent
    Vector3 CorridorSectionCoordinateToPosition(int x, int y, int width, int height)
    {
        return new Vector3(-width / 2f + 0.5f + x, -height / 2f + 0.5f + y);
    }


    //Returns Tile GameObject depending on the TileState given
    GameObject TileType(TileState tileState)
    {
        if (tileState == TileState.Open)        //If the tile state is Open
            return openTilePrefab;              //Return the openTilePrefab
        else if (tileState == TileState.Wall)   //If the tile state is Wall
            return null;                        //Return null as we don't actually need a GameObject as the game background is the same color as the wall tiles, which have no collider
        else
            return borderTilePrefab;            //Return a border tile prefab, unlike wall tiles, border tiles have a collider so need to be instantiated     
    }

    //Creates and stores all the data needed for a new random corridor section
    public struct CorridorSection
    {
        public TileState[,] flags;      //2D TileState array, stores which type of tile is needed      

        public int height;              //Height of this corridor

        public int exitCoordinateX;     //Tile xPosition where this corridor ended, i.e. where the next corridor needs to start

        //Called when a new corridor section is created, it needs a width, height and position where the corridor starts
        public CorridorSection(int _width, int _height, int entranceX)
        {
            height = _height;                           //Set height to the given height
            flags = new TileState[_width, height];      //Create the TileState[,] array

            //Randomly decide the direction and distance of this corridor sections horizontal open section
            int distance = Random.Range(2, (_width / (int)2f) - 2);
            int direction = Random.Range(0, 2) * 2 - 1;

            //Check this direction wont take us off the map
            if (entranceX + (distance * direction) <= 0 || entranceX + (distance * direction) >= _width - 1)
            {
                //If it does, flip the direction
                direction *= -1;
            }

            exitCoordinateX = 0;

            //Initialize all tiles as walls
            for (int i = 0; i < flags.GetLength(0); i++)
            {
                for (int j = 0; j < flags.GetLength(1); j++)
                {
                    flags[i, j] = TileState.Wall;
                }
            }

            //Entrance tile is always open
            flags[entranceX, 0] = TileState.Open;

            //Mark horizontal open flags based on the distance and direction
            for (int i = 1; i < distance + 1; i++)
            {
                flags[entranceX + i * direction, 0] = TileState.Open;

                if (i == distance)                                  //If this is the last horizontal flag
                    exitCoordinateX = entranceX + i * direction;    //This is the x position that will be the exit for this section
            }

            //Mark each flag above the horizontal exit tile as open
            for (int i = 1; i < height; i++)
            {
                flags[exitCoordinateX, i] = TileState.Open;
            }


            //Now work out which tiles are bordering open tiles

            //First loop through all the tiles in this section
            for (int i = 0; i < flags.GetLength(0); i++)
            {
                for (int j = 0; j < flags.GetLength(1); j++)
                {
                    //Only continue if we have found an open tile
                    if (flags[i, j] == TileState.Open)
                    {
                        //Loop through all that tiles neighbours
                        for (int x = -1; x <= 1; x++)
                        {
                            for (int y = -1; y <= 1; y++)
                            {
                                //Eliminate diagonal neighbours from the search
                                if (x == 0 || y == 0)
                                {
                                    int neighbourX = i + x;
                                    int neighbourY = j + y;

                                    //Check the neighour isn't outside the section (e.g. actually exists within the array)
                                    if (neighbourX >= 0 && neighbourY >= 0 && neighbourX < flags.GetLength(0) && neighbourY < flags.GetLength(1))
                                    {
                                        //Finally, if that tile is a wall then it's a wall directly adjacent to an open tile
                                        if (flags[neighbourX, neighbourY] == TileState.Wall)
                                            //Therefore needs to be a border
                                            flags[neighbourX, neighbourY] = TileState.Border;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

    }
}
