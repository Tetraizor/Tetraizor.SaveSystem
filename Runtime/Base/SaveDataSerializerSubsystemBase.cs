using System.Collections;
using Tetraizor.Bootstrap.Base;
using Tetraizor.Systems.Save.Base;
using UnityEngine;

namespace Tetraizor.Systems.Save.Base
{
    public abstract class SaveDataSerializerSubsystemBase : MonoBehaviour, IPersistentSubsystem
    {
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

        public abstract IEnumerator LoadSubsystem(IPersistentSystem system);

        public abstract string GetSystemName();

        public void CleanSerializer()
        {
            _isReading = false;
            _isWriting = false;
            _readResult = null;
            _progress = 0;
        }
    }
}