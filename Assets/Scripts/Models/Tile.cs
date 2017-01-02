using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathFind;
using UnityEngine;

public class Tile : SpacialObject, IHasNeighbours<Tile> {
	public Tile(int x, int y, HexTile tile)
        : base(x, y) {
		hexTile = tile;
		canPass = true;
    }

	public HexTile hexTile { get; set; }
	public bool canPass { get; set; }

	public IEnumerable<Tile> AllNeighbours { get;  set;  }
	public IEnumerable<Tile> ValidTiles { get { return AllNeighbours.Where (o => o.canPass); } } //Returns all neighbour tiles that aren't cities
	public IEnumerable<Tile> RoadTiles { get { return AllNeighbours.Where (o => o.hexTile.isRoad); } } // Returns all neighbouring road tiles

    public void FindNeighbours(Tile[,] gameBoard) {
        var neighbours = new List<Tile>();

		List<Point> possibleExits;
		if ((X % 2) == 0) {
			possibleExits = EvenNeighbours;
		} else {
			possibleExits = OddNeighbours;
		}

		for (int i = 0; i < possibleExits.Count; i++) {
			int neighbourCoordinateX = X + possibleExits [i].X;
			int neighbourCoordinateY = Y + possibleExits [i].Y;
			if (neighbourCoordinateX >= 0 && neighbourCoordinateX < gameBoard.GetLength(0) && neighbourCoordinateY >= 0 && neighbourCoordinateY < gameBoard.GetLength(1)){
				neighbours.Add (gameBoard [neighbourCoordinateX, neighbourCoordinateY]);
			}
			
		}
		AllNeighbours = neighbours;
    }

    public static List<Point> EvenNeighbours {
        get {
            return new List<Point> {
				new Point(0, 1),
				new Point(1, 0),
				new Point(1, -1),
				new Point(0, -1),
				new Point(-1, -1),
				new Point(-1, 0),

            };
        }
    }

    public static List<Point> OddNeighbours {
        get {
            return new List<Point> {
				new Point(0, 1),
				new Point(1, 1),
				new Point(1, 0),
				new Point(0, -1),
				new Point(-1, 0),
				new Point(-1, 1),
            };
        }
    }
}
