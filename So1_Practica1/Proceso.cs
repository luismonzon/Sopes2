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
        bool estado;
        List<int> recursos;
        List<int> esperando;

       
        public Proceso(string num) {
            estado = true;
            numero = num;
            recursos = new List<int>();
            esperando = new List<int>();
        }
        public List<int> Esperando
        {
            get { return esperando; }
            set { esperando = value; }
        }

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
        public String Numero
        {
            get { return numero; }
            set { numero = value; }
        }
        
        public List<int> Recursos
        {
            get { return recursos; }
            set { recursos = value; }
        }

        public bool ExisteBloqueado(int num) {

            foreach (var item in this.esperando)
            {
                if(item.Equals(num)){
                    return true;
                }
            }

            return false;
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

        public bool LiberarRecurso(int num){
           return this.recursos.Remove(num);
        
        }

        public String getBloqueados() {
            string rec = "";
            foreach (var item in this.esperando)
	        {
                rec += "R"+item+",";
	        }
            rec.TrimEnd(',');
            return rec;          
        }
    
    }
}
