using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace So1_Practica1
{
    public class Tiempo
    {
        public List<Proceso> procesos;
        public String id;
        public Tiempo(string name){
            procesos = new List<Proceso>();
            id = name;
        }

    }
}
