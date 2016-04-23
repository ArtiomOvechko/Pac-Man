using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controller.Interfaces
{
    public interface IExceptionObservable
    {
        void RegisterExceptionObserver(IExceptionObserver observer);

        void RemoveExceptionObserver(IExceptionObserver observer);

        void NotifyExceptionObservers();
    }
}
