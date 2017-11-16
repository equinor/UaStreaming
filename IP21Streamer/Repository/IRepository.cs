using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IP21Streamer.Repository
{
    interface IRepository
    {
        void Initialize();
        void Dispose();
        void UpdateMetaDataWith(List<TagItem> foundTagItems);
        List<TagItem> GetSubscriptionList();
    }
}
