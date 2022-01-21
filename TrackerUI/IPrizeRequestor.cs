using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.models;

namespace TrackerUI
{
    public interface IPrizeRequestor
    {
        void PrizeComplete(PrizeModel model);
    }
}
