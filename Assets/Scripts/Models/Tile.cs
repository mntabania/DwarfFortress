using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathFind;
using UnityEngine;

namespace Model {
    public class Tile : SpacialObject, IHasNeighbours<Tile> {
		public Tile(int x, int y, HexTile tile)
            : base(x, y) {
			hexTile = tile;
        }			
		public HexTile hexTile { get; set; }
       
		public IEnumerable<Tile> Neighbours { get;  set;  }

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
					if (gameBoard [neighbourCoordinateX, neighbourCoordinateY].hexTile.canPass) {
						neighbours.Add (gameBoard [neighbourCoordinateX, neighbourCoordinateY]);
					}
				}
				
			}
			Neighbours = neighbours;
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
}