using Code.App.Services.Interfaces;

namespace Code.App.Services
{
    public class IdGeneratorService : IIdGeneratorService
    {
        private int _nextId = 1;

        public int GenerateId()
        {
            return _nextId++;
        }
    }
}