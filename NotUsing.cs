using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AircraftProject
{
    delegate void EventAircraft_notUsing(Operation evAir);

    class NotUsing : StorageAirport
    {
        public bool Resolution { get; set; }             //Разрешение на посадку

        public event EventAircraft_notUsing aircraft_event;       //Событие
        public List<Operation> loperation;               //Лист операций\журнал
        public NotUsing() { loperation = new List<Operation>(); aircraft_event += OnEventAircraft; } //Конструктор + регистрация обработчика событий
        
        //Обработчик событий
        public virtual void OnEventAircraft(Operation handlerEv)
        {
            lock (this)     //Блокировка потоков на момент регистрации события
            {
                if (handlerEv == null) { Console.WriteLine("Event handler is null"); return; }
                if (handlerEv.Sender == null) { Console.WriteLine("Sender is null"); return; }
                try
                {
                    loperation.Add(handlerEv);
                }
                catch (Exception Ex)
                {
                    Console.WriteLine(Ex.Message);
                    Console.WriteLine(" Event ={0}  Aircraft={1}", handlerEv.TEA, handlerEv.Sender);
                }
            }
        }

        //Ремонт самолета
        public void Repair(Aircraft currentPlane, int intervalRepair)
        {
            if (currentPlane == null) { return; }

            //Формирование информации о событии
            Operation oper_info = new Operation();
            oper_info.TEA = TypeEventAircraft.StartRepeir;
            oper_info.Sender = currentPlane;

            // генерация события
            //if (aircraft_event != null) aircraft_event(oper_info);
            
            Thread.Sleep(intervalRepair);  //Приостанавливает текущий поток на заданное время(в данном примере - на миллисекунды)

            oper_info = new Operation();
            oper_info.TEA = TypeEventAircraft.FinishRepair;
            oper_info.Sender = currentPlane;

            // генерация события
            if (aircraft_event != null) aircraft_event(oper_info);      //Почему генерация 2 раза?

            ////Проверка
            //if()
            // { 
            //     aircraft_event(oper_info);
            // }
            // else
            // {
            //     Console.WriteLine("Самолет не требуется в ремонте");
            // }
        }

        //Заправка самолета
        public void Refueling(Aircraft currentPlane, int intervalRefueling)
        {
            if (currentPlane == null) { return; }

            //Формирование информации о событии

            Operation oper_info = new Operation();
            oper_info.TEA = TypeEventAircraft.StartRefueling;
            oper_info.Sender = currentPlane;
            //currentPlane.Active = false;  

            // генерация события
            if (aircraft_event != null) aircraft_event(oper_info);

            Thread.Sleep(intervalRefueling);  //Приостанавливает текущий поток на заданное время(в данном примере - на миллисекунды)

            oper_info = new Operation();
            oper_info.TEA = TypeEventAircraft.FinishRefueling;
            oper_info.Sender = currentPlane;

            // генерация события
            if (aircraft_event != null) aircraft_event(oper_info);      //Почему генерация 2 раза?
            //currentPlane.Active = true;
        }

        //Посадка самолета
        public void Landing(Aircraft currentPlane)
        {
            if (currentPlane == null) { return; }
            Operation oper_info = new Operation();
            oper_info.TEA = TypeEventAircraft.Landing;
            oper_info.Sender = currentPlane;


            // Генерация события
            if (aircraft_event != null) aircraft_event(oper_info);
            this.Landing(currentPlane);
        }

        //Взлет самолета
        public void Takeoff(Aircraft currentPlane)
        {
            if (currentPlane == null) { return; }
            Operation oper_info = new Operation();
            oper_info.TEA = TypeEventAircraft.Takeoff;
            oper_info.Sender = currentPlane;

            // Генерация события
            if (aircraft_event != null) aircraft_event(oper_info);
            this.Takeoff(currentPlane);
        }

        //Oтчет(report) по событиям операций за интервал времени
        public IEnumerable<Operation> GetOperationWithTimeInterval2(DateTime beginDT, DateTime endDT)
        {
            IEnumerable<Operation> report = from o_info in loperation
                                            where (o_info.DT >= beginDT && o_info.DT <= endDT)
                                            select o_info;
            return report;
        }

        //Oтчет(report) по событиям самолета
        public IEnumerable<Operation> GetOperationForAircraft2(Aircraft currentPlane)
        {
            IEnumerable<Operation> report = from o_info in loperation
                                            where (o_info.Sender == currentPlane)
                                            select o_info;
            return report;
        }


    }
}
