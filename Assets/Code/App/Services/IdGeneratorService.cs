namespace Code.App.Services
{
    public interface IIdGeneratorService
    {
        int GenerateId();
    }

    public class IdGeneratorService : IIdGeneratorService
    {
        private int _nextId = 1;

        public int GenerateId()
        {
            return _nextId++;
        }
    }
}