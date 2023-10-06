using System.Collections;
using Tetraizor.Bootstrap.Base;
using Tetraizor.DebugUtils;
using UnityEngine;

namespace Tetraizor.Systems.Save.Base
{
    public delegate void SerializationStatusEventHandler();

    public abstract class SaveDataManagerSubsystemBase<ManagerType, DataType> : MonoBehaviour, IPersistentSubsystem
            where ManagerType : SaveDataManagerSubsystemBase<ManagerType, DataType>
            where DataType : ISaveData
    {
        #region Static Accessor

        public static ManagerType Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<ManagerType>();
                }

                return _instance;
            }
        }
        protected static ManagerType _instance;

        #endregion

        #region Events

        public event SerializationStatusEventHandler SerializationStarted;
        public event SerializationStatusEventHandler SerializationCompleted;

        public event SerializationStatusEventHandler DeserializationStarted;
        public event SerializationStatusEventHandler DeserializationCompleted;

        #endregion

        #region Properties

        // References
        protected SaveSystem _saveSystem;
        protected SaveDataSerializerSubsystemBase _serializer;

        // Data Properties
        private string _savePath = null;

        // Progress Information
        private DataType _data;
        public DataType Data => _data;

        public bool IsReading => _isReading;
        private bool _isReading = false;

        public bool IsWriting => _isWriting;
        private bool _isWriting = false;

        public float Progress => _progress;
        private float _progress = 0;

        private float _retryInterval = 1;
        private int _retryCount = 5;

        #endregion

        #region Base Methods

        protected void SetSavePath(string path)
        {
            _savePath = path;
        }

        #endregion

        #region Save/Load Methods

        public virtual IEnumerator SaveDataAsync(ISaveData saveData)
        {
            SerializationStarted?.Invoke();

            int currentRetryCount = _retryCount;

            // Wait for other operations to complete.
            while (_serializer.IsReading || _serializer.IsWriting)
            {
                DebugBus.LogPrint($"There is an already going serialization progress. Waiting {_retryInterval} seconds to try again.");

                yield return new WaitForSeconds(_retryInterval);
                currentRetryCount -= 1;

                if (currentRetryCount <= 0)
                {
                    DebugBus.LogError("Serialization process timed out. There is probably an error reading/writing the file.");
                    yield break;
                }
            }

            _isWriting = true;

            // Start serialization.
            StartCoroutine(_serializer.SerializeData<DataType>(saveData, _savePath));

            // Serialize.
            while (_serializer.IsWriting)
            {
                _progress = _serializer.Progress;

                yield return null;
            }

            // Update current data as well.
            _data = (DataType)saveData;

            _isWriting = false;

            SerializationCompleted?.Invoke();
        }

        public virtual IEnumerator LoadDataAsync()
        {
            DeserializationStarted?.Invoke();

            int currentRetryCount = _retryCount;

            // Wait for other operations to complete.
            while (_serializer.IsReading || _serializer.IsWriting)
            {
                DebugBus.LogPrint($"There is an already going serialization progress. Waiting {_retryInterval} seconds to try again.");

                yield return new WaitForSeconds(_retryInterval);
                currentRetryCount -= 1;

                if (currentRetryCount <= 0)
                {
                    DebugBus.LogError("Serialization process timed out. There is probably an error reading/writing the file.");
                    yield break;
                }
            }

            _isReading = true;

            // Start deserialization.
            StartCoroutine(_serializer.DeserializeData<DataType>(_savePath));

            // Deserialize.
            while (_serializer.IsReading)
            {
                _progress = _serializer.Progress;
                yield return null;
            }

            yield return null;

            _data = (DataType)_serializer.ReadResult;

            _isReading = false;

            DeserializationCompleted?.Invoke();
        }

        #endregion

        #region IPersistentSubsystem Methods

        public string GetSystemName()
        {
            return "Save System";
        }

        public virtual IEnumerator LoadSubsystem(IPersistentSystem system)
        {
            _saveSystem = (SaveSystem)system;

            _serializer = _saveSystem.Serializer;
            if (_serializer == null)
            {
                DebugBus.LogError("Could not find any Serializer loaded, might be the subsystem loading order is incorrect, or no serializer subsystems were specified!");
            }

            yield return LoadDataAsync();

            if (Data == null)
            {
                yield return CreateInitialSaveData();
                yield return LoadDataAsync();

                if (Data == null)
                {
                    DebugBus.LogError("Could not get data even with the creation of an initial save data.");
                }
            }
        }

        protected abstract IEnumerator CreateInitialSaveData();

        #endregion
    }
}