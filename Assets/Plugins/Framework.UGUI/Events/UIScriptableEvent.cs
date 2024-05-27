namespace Kurisu.Framework.UGUI
{
    public abstract class UIScriptableEvent : ManagedObject
    {
        public abstract void Trigger();
        /// <summary>
        /// Reset event manually if cross scene
        /// </summary>
        public virtual void ResetEvent() { }
    }
}