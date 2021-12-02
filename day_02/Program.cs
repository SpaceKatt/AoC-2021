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

        private SubmarineCourse loadCourseFile(string courseFilePath)
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
            var oldAim = this.Coordinate.aim;
            switch (movement.direction)
            {
                case Direction.forward:
                    var forwardX = oldX + movement.magnitude;
                    var forwardDepth = oldDepth + oldAim * movement.magnitude;
                    this.Coordinate = new SubmarineCoordinate(forwardDepth, forwardX, oldAim);
                    break;
                case Direction.backward:
                    var backX = oldX - movement.magnitude;
                    var backDepth = oldDepth - oldAim * movement.magnitude;
                    this.Coordinate = new SubmarineCoordinate(backDepth, backX, oldAim);
                    break;
                case Direction.up:
                    var upAim = oldAim - movement.magnitude;
                    this.Coordinate = new SubmarineCoordinate(oldDepth, oldX, upAim);
                    break;
                case Direction.down:
                    var downAim = oldAim + movement.magnitude;
                    this.Coordinate = new SubmarineCoordinate(oldDepth, oldX, downAim);
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
        public int aim { get; set; }

        public SubmarineCoordinate(int depth, int xPos, int aim)
        {
            this.depth = depth;
            this.xPos = xPos;
            this.aim = aim;
        }
    }

    public enum Direction {
        forward,
        backward,
        up,
        down,
    }
}
