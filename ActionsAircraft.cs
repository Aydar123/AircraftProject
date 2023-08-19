using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AircraftProject
{
    class ActionsAircraft
    {
        public event DelEventAircraft event_actions;
        public AirportWithEvents AWE;
        public Aircraft ac;
        public int timing;
        bool Resolution;   //Разрешение на действие
        static Random rnd = new Random();
       
        public ActionsAircraft() { AWE = null; ac = null; timing = 0; Resolution = true; }
        public ActionsAircraft(AirportWithEvents ew, Aircraft a, int t)
        {
            AWE = ew; ac = a; timing = t; Resolution = true;
        }

        public void InitEvent()
        {
            if (AWE != null) event_actions += AWE.OnEventAircraft;
            if (AWE != null) AWE.airport_go_new += OnGo;
        }

        public void OnGo(bool resol)   //обработчик 
        {
            Resolution = resol;
            //Console.WriteLine("Change Resolution {0}", Resolution);
        }

        //Ремонт самолета
        public void Repair(Aircraft currentPlane, int intervalRepair)
        {
            if (currentPlane == null) { return; }

            //Формирование информации о событии
            Operation oper_info = new Operation();
            oper_info.TEA = TypeEventAircraft.StartRepeir;
            oper_info.Sender = currentPlane;
            oper_info.Message = "Начало ремонта";

            // генерация события
            if (event_actions != null) event_actions(oper_info);

            Thread.Sleep(intervalRepair);  //Приостанавливает текущий поток на заданное время(в данном примере - на миллисекунды)

            oper_info = new Operation();
            oper_info.TEA = TypeEventAircraft.FinishRepair;
            oper_info.Sender = currentPlane;
            oper_info.Message = "Конец ремонта";

            // генерация события
            if (event_actions != null) event_actions(oper_info);  

        }

        //Заправка самолета
        public void Refueling(Aircraft currentPlane, int intervalRefueling)
        {
            if (currentPlane == null) { return; }

            //Формирование информации о событии

            Operation oper_info = new Operation();
            oper_info.TEA = TypeEventAircraft.StartRefueling;
            oper_info.Sender = currentPlane;
            oper_info.Message = "Начало заправки";

            // генерация события
            if (event_actions != null) event_actions(oper_info);

            Thread.Sleep(intervalRefueling);  //Приостанавливает текущий поток на заданное время(в данном примере - на миллисекунды)

            oper_info = new Operation();
            oper_info.TEA = TypeEventAircraft.FinishRefueling;
            oper_info.Sender = currentPlane;
            oper_info.Message = "Конец заправки";

            // генерация события
            if (event_actions != null) event_actions(oper_info);     
            
        }

        //Взлет самолета
        public void Landing(Aircraft currentPlane)
        {
            if (currentPlane == null) { return; }
            Operation oper_info = new Operation();
            oper_info.TEA = TypeEventAircraft.Landing;
            oper_info.Sender = currentPlane;
            oper_info.Message = "Взлет самолета";

            // Генерация события
            if (event_actions != null) event_actions(oper_info);

        }

        //Посадка самолета
        public void Takeoff(Aircraft currentPlane)
        {
            if (currentPlane == null) { return; }
            Operation oper_info = new Operation();
            oper_info.TEA = TypeEventAircraft.Takeoff;
            oper_info.Sender = currentPlane;
            oper_info.Message = "Посадка самолета";

            // Генерация события
            if (event_actions != null) event_actions(oper_info);

        }

        int r = 0;
        public void DoActionAirCraft()   //генерируются действия
        {

            if (AWE != null && ac != null && timing > 0)
            {
                Thread.Sleep(rnd.Next(timing));

                while (!Resolution)
                {
                    Thread.Sleep(25);
                }

                Landing(ac);
                Thread.Sleep(rnd.Next(timing));
                r = rnd.Next(100);
                              
                if (r < 25) { Repair(ac, timing * 10); }
                Thread.Sleep(rnd.Next(timing));

                Refueling(ac, timing);
                Thread.Sleep(rnd.Next(timing));

                while (!Resolution)
                {
                    Thread.Sleep(25);
                }

                Takeoff(ac);
            }

        }
    }
}
