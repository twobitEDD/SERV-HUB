namespace Modules.CSVCreator
{
    // Line for csv file with data
    public class CsvLine
    {
        // Array of cells
        private readonly string[] cells;
        public CsvLine(int length) => cells = new string[length];

        // Setup sell
        public void SetCell(int id, string item)
        {
            if (id < 0)
                id = 0;

            if (id >= cells.Length)
                id = cells.Length - 1;

            cells[id] = $"\u0022{item}\u0022";
        }

        // Form csv line
        public string FormLine() => string.Join(",", cells);
    }
}
