using System;
using System.Collections.Generic;

using Controller.Interfaces;

namespace Controller.Core
{
    public class ExceptionHandler : IExceptionHandler
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection
                .MethodBase.GetCurrentMethod().DeclaringType);

        private string _message;

        private readonly List<IExceptionObserver> _exceptionObservers;

        public ExceptionHandler()
        {
            _exceptionObservers = new List<IExceptionObserver>();
        }

        public void HandleException(Exception ex)
        {
            Log.Error(ex);
            _message = ex.Message;
            NotifyExceptionObservers();
            Environment.Exit(0);
        }

        public void RegisterExceptionObserver(IExceptionObserver observer)
        {
            _exceptionObservers.Add(observer);
        }

        public void RemoveExceptionObserver(IExceptionObserver observer)
        {
            _exceptionObservers.Remove(observer);
        }

        public void NotifyExceptionObservers()
        {
            foreach (IExceptionObserver observer in _exceptionObservers)
            {
                observer.Update(_message);
            }
        }
    }
}
