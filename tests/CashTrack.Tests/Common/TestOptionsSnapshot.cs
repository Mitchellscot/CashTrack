using Microsoft.Extensions.Options;

namespace CashTrack.Tests
{
    public class TestOptionsSnapshot<T> : IOptionsSnapshot<T> where T : class
    {
        private T _settings;

        public TestOptionsSnapshot(T settings)
            => (_settings) = (settings);

        public T Value => _settings;
        public T Get(string name) => throw new System.NotImplementedException();
    }
}