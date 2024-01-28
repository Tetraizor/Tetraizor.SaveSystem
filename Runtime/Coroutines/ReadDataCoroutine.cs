using Tetraizor.Systems.Save.Base;

namespace Tetraizor.Systems.Save.Coroutines
{
    public class ReadDataCoroutine
    {
        public ISaveData Result => _result;
        private ISaveData _result = null;

        public bool IsDone => _isDone;
        private bool _isDone = false;

        public float Progress => _progress;
        private float _progress = 0;

        private SaveDataSerializerBase _serializer;
        private string _path;

        public ReadDataCoroutine(SaveDataSerializerBase serializer, string path)
        {
            _serializer = serializer;
            _path = path;
        }
    }
}
