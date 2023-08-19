using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AircraftProject
{
 
    class StorageAirport : Storage<Aircraft>
    {

        public StorageAirport() : base() { }
        public void InAirport(Aircraft a)    //Добавляет в хранилище самолет
        { this.AddBasic(a); }

        public void OutAirport(Aircraft a)
        { if (a.Active) this.RemoveBasic(a); }

        public IEnumerable<Passenger> ViewTypePassenger(TypePassenger selectTypeP)
        {
            foreach (Aircraft current_pass in _objs)
            {
                if (current_pass is Passenger)
                if (((Passenger)current_pass).TP == selectTypeP) yield return (Passenger)current_pass;
                //if (curplain.TP == selectTypeP) yield return curplain;
            }
        }

        public IEnumerable<Military> ViewMilitary(TypeMilitary selectTypeM, WeaponSystem selectWS)
        {
            foreach (Aircraft current_mil in _objs)
            {
                if (current_mil is Military)
                if (((Military)current_mil).TM == selectTypeM) yield return (Military)current_mil;
                //if (((Military)current_mil).WS == selectWS) yield return (Military)current_mil;
            }
        }

        public IEnumerable<Cargo> ViewCargo(int carry1)
        {
            foreach (Aircraft current_cargo in _objs)
            {
                if (current_cargo is Cargo)
                if (((Cargo)current_cargo).Сarrying >= carry1) yield return (Cargo)current_cargo;
            }
        }


    }
}
