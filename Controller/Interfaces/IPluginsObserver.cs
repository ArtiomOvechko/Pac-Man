using System.Collections.Generic;

namespace Controller.Interfaces
{
    public interface IPluginsObserver
    {
        void Update(List<string> itemList);
    }
}
