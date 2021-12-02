using System;
using System.Linq;

namespace Advent2021
{
    using SubmarineCourse = List<SubmarineMovement>;

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Day 02");

            string courseFilePath = "./input.csv";

            var sub = new Submarine();
            sub.navigateSubmarineFromCourseFile(courseFilePath);

            Console.WriteLine(sub.Coordinate.xPos);
            Console.WriteLine(sub.Coordinate.depth);
            Console.WriteLine(sub.Coordinate.xPos * sub.Coordinate.depth);
        }
    }

    class Submarine
    {
        public Submarine() {
            this.Coordinate = new SubmarineCoordinate();
        }

        public SubmarineCoordinate Coordinate { get; set; }

        private List<SubmarineMovement> loadCourseFile(string courseFilePath)
        {
            return File.ReadLines(courseFilePath)
                .Select(line => {
                    var stringArray = line.Split(' ');
                    var direction =  (Direction) Enum.Parse(typeof(Direction), stringArray[0], true);
                    var magnitude = int.Parse(stringArray[1]);
                    var move = new SubmarineMovement(direction, magnitude);
                    return move;
                })
                .ToList();
        }

        public void navigateSubmarineFromCourseFile(string courseFilePath)
        {
            var moves = this.loadCourseFile(courseFilePath);
            this.navigateSubmarine(moves);
        }

        public void navigateSubmarine(SubmarineCourse course)
        {
            foreach (SubmarineMovement move in course)
            {
                this.MoveSubmarine(move);
            }
        }

        private void MoveSubmarine(SubmarineMovement movement)
        {
            var oldX = this.Coordinate.xPos;
            var oldDepth = this.Coordinate.depth;
            switch (movement.direction)
            {
                case Direction.forward:
                    var newX = this.Coordinate.xPos + movement.magnitude;
                    this.Coordinate = new SubmarineCoordinate(oldDepth, newX);
                    break;
                case Direction.backward:
                    var backX = this.Coordinate.xPos - movement.magnitude;
                    this.Coordinate = new SubmarineCoordinate(oldDepth, backX);
                    break;
                case Direction.up:
                    var upDepth = this.Coordinate.depth - movement.magnitude;
                    this.Coordinate = new SubmarineCoordinate(upDepth, oldX);
                    break;
                case Direction.down:
                    var downDepth = this.Coordinate.depth + movement.magnitude;
                    this.Coordinate = new SubmarineCoordinate(downDepth, oldX);
                    break;
                default:
                    throw new Exception("Unknown direction from ship command");
            }
        }
    }

    public struct SubmarineMovement
    {
        public Direction direction { get; set; }
        public int magnitude { get; set; }

        public SubmarineMovement(Direction direction, int magnitude)
        {
            this.direction = direction;
            this.magnitude = magnitude;
        }
    }

    public struct SubmarineCoordinate
    {
        public int depth { get; set; }
        public int xPos { get; set; }

        public SubmarineCoordinate(int depth, int xPos)
        {
            this.depth = depth;
            this.xPos = xPos;
        }
    }

    public enum Direction {
        forward,
        backward,
        up,
        down,
    }
}
