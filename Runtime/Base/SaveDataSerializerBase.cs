using System.Collections;
using UnityEngine;

namespace Tetraizor.Systems.Save.Base
{
    public abstract class SaveDataSerializerBase : MonoBehaviour
    {
        protected static int _serializerCount = 0;

        protected int _serializerID = 0;
        public int SerializerID => _serializerID;

        public ISaveData ReadResult => _readResult;
        protected ISaveData _readResult = null;

        public bool IsReading => _isReading;
        protected bool _isReading = false;

        public bool IsWriting => _isWriting;
        protected bool _isWriting = false;

        public float Progress => _progress;
        protected float _progress = 0;

        public abstract IEnumerator SerializeData<T>(ISaveData saveData, string path) where T : ISaveData;
        public abstract IEnumerator DeserializeData<T>(string path) where T : ISaveData;

        public void CleanSerializer()
        {
            _isReading = false;
            _isWriting = false;
            _readResult = null;
            _progress = 0;
        }
    }
}