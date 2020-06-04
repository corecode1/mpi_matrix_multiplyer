namespace MPI_Lab3
{
    class WrongProcessorCountException : System.Exception
    {
        public WrongProcessorCountException(int neededCount) : base($"Process count must be equal {neededCount}.") { }
    }
}
