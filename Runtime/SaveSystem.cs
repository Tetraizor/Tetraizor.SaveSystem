using System.Collections;
using UnityEngine;
using Tetraizor.Bootstrap.Base;
using Tetraizor.MonoSingleton;
using Tetraizor.Systems.Save.Base;

namespace Tetraizor.Systems.Save
{
    public class SaveSystem : MonoSingleton<SaveSystem>, IPersistentSystem
    {
        #region Properties

        private SaveDataSerializerSubsystemBase _serializer;
        public SaveDataSerializerSubsystemBase Serializer => _serializer;

        #endregion

        public void SetSerializer(SaveDataSerializerSubsystemBase serializer)
        {
            _serializer = serializer;
        }

        #region IPersistentSystem Methods

        public string GetName()
        {
            return "Save System";
        }

        public IEnumerator LoadSystem()
        {
            // Initialize subsystems.
            IPersistentSubsystem[] subsystems = gameObject.GetComponentsInChildren<IPersistentSubsystem>(true);

            foreach (IPersistentSubsystem subsystem in subsystems)
            {
                yield return subsystem.LoadSubsystem(this);
            }

            yield return null;
        }

        #endregion
    }
}