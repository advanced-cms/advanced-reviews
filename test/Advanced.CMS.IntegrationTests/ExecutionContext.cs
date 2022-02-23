using System;

namespace Advanced.CMS.IntegrationTests
{
    public enum ContextState
    {
        Same,
        New,
        Changed
    }

    public static class ExecutionContext
    {
        private static Type _currentEnv;
        private static Action _cleanUp;

        public static ContextState EstablishContext(Type t, Action cleanupContext)
        {
            if (t == _currentEnv)
            {
                return ContextState.Same;
            }

            var context = ContextState.New;
            if (_currentEnv != null)
            {
                CleanContext();
                context = ContextState.Changed;
            }
            _currentEnv = t;
            _cleanUp = cleanupContext;

            return context;
        }

        public static void RegisterCleanup(Action cleanup)
        {
            var currentClenup = _cleanUp;
            _cleanUp = () =>
            {
                currentClenup?.Invoke();
                cleanup?.Invoke();
            };
        }

        public static void CleanContext()
        {
            _currentEnv = null;
            _cleanUp?.Invoke();
            _cleanUp = null;
        }
    }
}
