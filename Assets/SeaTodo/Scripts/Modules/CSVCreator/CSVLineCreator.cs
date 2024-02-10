namespace Modules.CSVCreator
{
    // Component for create line object for csv generator
    public static class CsvLineCreator
    {
        private const int lineLength = 10;

        public static CsvLine CreateNewLine() => new CsvLine(lineLength);
    }
}
