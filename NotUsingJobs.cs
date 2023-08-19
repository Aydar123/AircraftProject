using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AircraftProject
{
    class JobsOfAir
    {
        //public event DelEventAircraft event_jobs;
        public EventsWithAircraft EWA;      //EVENTS WITH AIRPORT!

        public Storage<Aircraft> currentStor { get; set; }  //Ссылка на хранилищe
        public bool Resolution { get; set; }                //Разрешение на действие
        public int timeInterval { get; set; }               //Макс интервал времени 
        public Aircraft currentAircraft { get; set; }
        public int KolEvents { get; set; }                  //Количество событий
        //public int varAction;                             //Переключатель(для примера)
        //int v = 0;
        
        static Random rnd = new Random();

        public JobsOfAir() {currentAircraft = null; timeInterval = 0; Resolution = true; KolEvents = 0; EWA = null; }//пустой конструктор

        public JobsOfAir(Aircraft cA, EventsWithAircraft ew, /*Storage<Aircraft> cS,*/ int timeI, int KolJ)
        {
            currentAircraft = cA;
            EWA = ew;
           /* currentStor = cS;*/
            timeInterval = timeI;
            KolEvents = KolJ;
            Resolution = true;
            rnd = new Random();
        }
        
        //public void InitEvent()
        //{
        //    if (EWA != null) event_jobs += EWA.OnEventAircraft;
        //    if (EWA != null) EWA.airport_go += OnGo;
        //}

        //public void OnGo(bool g)
        //{
        //    Resolution = g;
        //}
        

        public void DoEvents()
        {
            while (Resolution)
            {
                for (int n = 0; n < KolEvents; n++)
                {
                    lock (this)
                    {
                        int r = rnd.Next(100);
                        if (r > 0 && currentAircraft != null && timeInterval > 0)
                        {
                            Thread.Sleep(rnd.Next(timeInterval));

                            while (!Resolution)
                            {
                                Thread.Sleep(25);
                            }

                            if (currentStor is EventsWithAircraft)
                            {
                                if (currentAircraft.Active && !currentStor.IsContains(currentAircraft))
                                   ((EventsWithAircraft)currentStor).Landing(currentAircraft);
                            }
                            Thread.Sleep(rnd.Next(timeInterval));

                            if (currentStor is EventsWithAircraft)
                            {
                                if (currentAircraft.Active && currentStor.IsContains(currentAircraft))
                                   ((EventsWithAircraft)currentStor).Refueling(currentAircraft, rnd.Next(timeInterval * 2 / KolEvents));
                            }
                            Thread.Sleep(rnd.Next(timeInterval));
                            //r = rnd.Next(100);

                            if (currentStor is EventsWithAircraft)
                            {
                                if (r < 20)
                                {
                                    if (currentAircraft.Active && currentStor.IsContains(currentAircraft))
                                       ((EventsWithAircraft)currentStor).Repair(currentAircraft, rnd.Next(timeInterval * 2 / KolEvents));
                                }
                            }

                            while (!Resolution)
                            {
                                Thread.Sleep(25);
                            }

                            if (currentStor is EventsWithAircraft)
                            {
                                if (currentAircraft.Active && currentStor.IsContains(currentAircraft))
                                   ((EventsWithAircraft)currentStor).Takeoff(currentAircraft);
                            }

                        }
                    }
                    Thread.Sleep(rnd.Next(timeInterval));
                }

                Console.WriteLine("Aircraft |{0}| finished of jobs with threads.", currentAircraft);
                Resolution = false;

            }
        }

    }
}
