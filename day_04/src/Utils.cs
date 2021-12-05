namespace Advent2021
{
    class Utils
    {
        public static List<List<bool>> InitializeBooleanMatrix(int size)
        {
            var matrix = new List<List<bool>>();

            var matrixRow = new List<bool>();
            for (int i = 0; i < size; i++)
            {
                matrixRow.Add(false);
            }

            for (int i = 0; i < size; i++)
            {
                matrix.Add(new List<bool>(matrixRow));
            }
            return matrix;
        }
    }
}