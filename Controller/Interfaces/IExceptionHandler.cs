using System;

namespace Controller.Interfaces
{
    public interface IExceptionHandler: IExceptionObservable
    {
        void HandleException(Exception ex);
    }
}
