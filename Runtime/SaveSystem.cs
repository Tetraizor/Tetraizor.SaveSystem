using System.Collections;
using Tetraizor.Bootstrap.Base;
using Tetraizor.MonoSingleton;

namespace Tetraizor.Systems.Save
{
    public class SaveSystem : MonoSingleton<SaveSystem>, IPersistentSystem
    {
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