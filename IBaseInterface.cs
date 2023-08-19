using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AircraftProject
{
    interface IBaseInterface
    {
        void Fly();         //Лететь 
        void Break();       //Ломаться
        void Refuel();      //Заправляться
    }
}
