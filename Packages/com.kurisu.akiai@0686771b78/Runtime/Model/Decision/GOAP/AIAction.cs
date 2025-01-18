using Kurisu.GOAP;
namespace Kurisu.AkiAI
{
    public abstract class AIAction<TContext> : GOAPAction where TContext : IAIContext
    {
        protected IAIHost<TContext> Host { get; private set; }
        public void Setup(IAIHost<TContext> host)
        {
            Host = host;
            OnSetup();
        }
        protected virtual void OnSetup() { }
    }
}