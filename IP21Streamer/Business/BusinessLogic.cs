using IP21Streamer.Source;
using IP21Streamer.Source.IP21;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IP21Streamer.Business
{
    class BusinessLogic
    {
        ISource<IP21Tag> Ingester;

        public BusinessLogic(ISource<IP21Tag> ingester)
        {

        }
    }
}
