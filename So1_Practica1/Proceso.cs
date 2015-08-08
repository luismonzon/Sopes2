using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace So1_Practica1
{
    public class Proceso
    {

        String numero;
        String tipo;
        bool estado = true;

        public bool Estado
        {
            get { return estado; }
            set { estado = value; }
        }
        public String Tipo
        {
            get { return tipo; }
            set { tipo = value; }
        }

        public Proceso(string num) {

            numero = num;
            recursos = new List<int>();
        }

        public String Numero
        {
            get { return numero; }
            set { numero = value; }
        }
        List<int> recursos;

        public List<int> Recursos
        {
            get { return recursos; }
            set { recursos = value; }
        }


        public bool ExisteRecurso(int num) {

            foreach (var item in Recursos)
            {
                if(item.Equals(num)){
                    return true;
                }
            }
            return false;
        }
    }
}
