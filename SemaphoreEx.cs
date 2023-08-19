using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AircraftProject
{
    class SemaphoreEx 
    {
        //Семафоры
        Semaphore sem = new Semaphore(5, 20);
        Thread myThread;
        int count = 3;// счетчик чтения
        
        
        public SemaphoreEx()
        {
            myThread = new Thread(ThA);
            //myThread.Name = "Самолет_" + i;
            myThread.Start();
            
        }

        public void ThA()
        {
            while (count > 0)
            {
                sem.WaitOne();  //ожидания получения семафора

                Console.WriteLine($"{Thread.CurrentThread} самолет приземляется - посадка");

                Console.WriteLine($"{Thread.CurrentThread} ремонт или заправка");
                Thread.Sleep(1000);

                Console.WriteLine($"{Thread.CurrentThread} самолет улетает - взлет");

                sem.Release();  //высвобождаем семафор

                count--;
                Thread.Sleep(1000);
            }
        }


    }
}
