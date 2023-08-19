using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AircraftProject
{
    public enum TypeEventAircraft
    {
        Landing = 1,     //Посадка самолета
        Takeoff,         //Взлет самолета
        StartRefueling,  //Начало заправки
        FinishRefueling, //Конец заправки
        StartRepeir,     //Начало ремонта
        FinishRepair     //Конец ремонта

    }

    class Operation
    {
        public TypeEventAircraft TEA;
        public Aircraft Sender;        //источник событий
        public DateTime DT;
        public string Message;

        public Operation() { Sender = null; DT = DateTime.Now; Message = ""; }
        public override string ToString()
        {
            return string.Format("Operation {0} {1} {2} {3}", TEA, Sender, DT, Message);
        }


    }

}
