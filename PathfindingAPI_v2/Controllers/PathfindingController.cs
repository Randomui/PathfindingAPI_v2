using Microsoft.AspNetCore.Mvc;
using PathfindingAPI_v2.Models; // Ensure this points to the correct models namespace
using System.Collections.Generic;

namespace PathfindingAPI_v2.Controllers
{
    [ApiController]
    [Route("api")]
    public class PathfindingController : ControllerBase
    {
        [HttpPost("find-path")]
        public ActionResult<PathResponse> FindPath([FromBody] PathRequest request)
        {
            if (request.Start == null || request.End == null)
            {
                return BadRequest("Start and End coordinates must be provided.");
            }

            var path = DepthFirstSearch(request.Start, request.End, request.Obstacles);
            if (path.Count == 0)
            {
                return NotFound("No path found.");
            }

            return Ok(new PathResponse { Path = path });
        }

        private List<Coordinate> DepthFirstSearch(Coordinate start, Coordinate end, List<Coordinate> obstacles)
        {
            int gridSize = 20;
            bool[,] visited = new bool[gridSize, gridSize];
            List<Coordinate> path = new List<Coordinate>();
            bool found = false;

            HashSet<(int, int)> obstacleSet = new HashSet<(int, int)>();
            foreach (var obstacle in obstacles)
            {
                obstacleSet.Add((obstacle.X, obstacle.Y));
            }

            bool Dfs(Coordinate current)
            {
                if (current.X < 0 || current.X >= gridSize || current.Y < 0 || current.Y >= gridSize ||
                    visited[current.Y, current.X] || obstacleSet.Contains((current.X, current.Y)))
                {
                    return false;
                }

                visited[current.Y, current.X] = true;
                path.Add(current);

                if (current.X == end.X && current.Y == end.Y)
                {
                    found = true;
                    return true; // Path found
                }

                // Prioritize moving towards the end first
                var directions = new List<Coordinate>
                {
                    new Coordinate { X = 1, Y = 0 }, // Right
                    new Coordinate { X = -1, Y = 0 }, // Left
                    new Coordinate { X = 0, Y = 1 }, // Down
                    new Coordinate { X = 0, Y = -1 }  // Up
                };

                foreach (var direction in directions)
                {
                    if (found) return true; // Stop if the end is found

                    if (Dfs(new Coordinate { X = current.X + direction.X, Y = current.Y + direction.Y }))
                    {
                        return true; // If a valid path to the end is found
                    }
                }

                // Backtrack if no path to end was found from this position
                path.RemoveAt(path.Count - 1);
                return false;
            }

            Dfs(start);
            return path; // Return the path leading to the end
        }
    }
}