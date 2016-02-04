using GodsWill_ASCIIRPG.Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model.PathFinding
{
    public class AStar
    {
        public bool Success { get; private set; }

        List<Coord> path;

        public AStar(Map map, Coord start, Coord goal)
        {
            this.path = new List<Coord>();
            Success = false;//failure

            var closedset = new List<Coord>();
            var openset = new List<Coord>();

            var g_score = new BidimensionalArray<int>(map.Height, map.Width);
            var h_score = new BidimensionalArray<int>(map.Height, map.Width);
            var f_score = new BidimensionalArray<int>(map.Height, map.Width);
            var came_from = new BidimensionalArray<Coord?>(map.Height, map.Width);

            openset.Add(start);// The set of tentative nodes to be evaluated, initially containing the start node
            came_from[start] = null;

            g_score[start] = 0;    // Cost from start along best known path.
            h_score[start] = heuristic_cost_estimate(start, goal);
            f_score[start] = g_score[start] + h_score[start];    // Estimated total cost from start to goal through y.

            while (openset.Count > 0)
            {
                var x = findMin(openset, f_score);
                if (openset[x] == goal)
                {
                    reconstruct_path(came_from, (Coord)goal);
                    Success = true;//success
                    break;
                }

                //add x to closedset
                closedset.Add(openset[x]);
                //remove x from openset
                openset.RemoveAt(x);

                var neighbours = new List<Coord>();
                findNeighbours(map, closedset[closedset.Count - 1], neighbours);

                bool tentative_is_better = false;
                //foreach y in neighbor_nodes(x)
                int current = 0;
                foreach (var coord in neighbours)
                {
                    if (closedset.Contains(neighbours[current]))
                    {
                        current++;
                        continue;
                    }
                    int tentative_g_score = g_score[closedset[closedset.Count - 1]] + 1;

                    if (!openset.Contains(neighbours[current])) //y not in openset
                    {
                        //add y to openset
                        openset.Add(neighbours[current]);
                        h_score[neighbours[current]] = heuristic_cost_estimate(neighbours[current], goal);
                        tentative_is_better = true;
                    }
                    else
                    {
                        if (tentative_g_score < g_score[neighbours[current]])
                        {
                            tentative_is_better = true;
                        }
                        else
                        {
                            tentative_is_better = false;
                        }
                    }

                    if (tentative_is_better == true)
                    {
                        came_from[neighbours[current]] = closedset[closedset.Count - 1];
                        g_score[neighbours[current]] = tentative_g_score;
                        f_score[neighbours[current]] = g_score[neighbours[current]] + h_score[neighbours[current]];
                    }

                    current++;
                }
            }
        }

        private void reconstruct_path(BidimensionalArray<Coord?> came_from, Coord current_node)
        {
            if (came_from[current_node] != null)
            {
                path.Add((Coord)current_node);
                reconstruct_path(came_from, (Coord)came_from[current_node]);
            }
        }

        private int heuristic_cost_estimate(Coord start, Coord goal)
        {
            return start.ManhattanDistance(goal);
        }

        private void findNeighbours(Map map, Coord loc, List<Coord> neighbours)
        {
            //var explorableDirs = new Direction[]
            //{
            //    Direction.North,
            //    Direction.East,
            //    Direction.South,
            //    Direction.West,
            //};

            var explorableDirs = (Direction[])Enum.GetValues(typeof(Direction));

            foreach (var dir in explorableDirs)
            {
                var to = loc.CoordFromDirection(dir);
                if (to.X >= 0 && to.X < map.Width
                    && to.Y >= 0 && to.Y < map.Height
                    && (map[to].Walkable || map[to].IsAttackable()))
                {
                    neighbours.Add(to);
                }
            }
        }

        private int findMin(List<Coord> openset, BidimensionalArray<int> f_score)
        {
            var min = f_score[openset[0]];
            var pos = 0;

            for (int i = 1; i < openset.Count; i++)
            {
                var val = f_score[openset[i]];
                if (val < min)
                {
                    pos = i;
                    min = val;
                }
            }

            return pos;
        }

        public List<Direction> GetDirections(Coord currPos, int numSteps = -1)
        {
            var n = numSteps == -1 ? path.Count : Math.Min(path.Count, numSteps);
            var directions = new List<Direction>(n);

            if (n > 0)
            {
                directions.Add(currPos.DirectionFromOffset(path[0]));
                for (int i = n - 1; i > 0; i--)
                {
                    directions.Add(path[i].DirectionFromOffset(path[i - 1]));
                }
            }

            return directions;
        }
    }
}