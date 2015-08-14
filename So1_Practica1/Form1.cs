using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Irony.Parsing;
using Irony.Ast;
namespace So1_Practica1
{
    public partial class Form1 : Form
    {

        List<Tiempo> instrucciones;
        List<Proceso> procesos;
        Stack<Proceso> dependencias = new Stack<Proceso>();
        public Form1()
        {
            InitializeComponent();
            instrucciones = new List<Tiempo>();
            procesos = new List<Proceso>();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            Gramatica gramatica = new Gramatica();
            Parser parser = new Parser(gramatica);
            ParseTree raiz = parser.Parse(this.textBox1.Text);
            this.textBox2.Clear();
            if (raiz.Root != null)
            {
                this.Analize(raiz.Root, null);
                if (this.checkBox1.Checked)
                {
                    Ordenar();
                }
                Ejecutar();
                Console.Write("");
                this.instrucciones.Clear();
                this.procesos.Clear();

            }


        }
        private string Analize(ParseTreeNode node, Tiempo instruccion)
        {
            if (node.Term.Name.Equals("inicio"))
            {
                Analize(node.ChildNodes[0], instruccion);

            }
            else if (node.Term.Name.Equals("instruccion"))
            {

                string t = Analize(node.ChildNodes[0], instruccion);
                Tiempo nuevo = new Tiempo(t);
                string process = Analize(node.ChildNodes[2], nuevo);

                this.instrucciones.Add(nuevo);

            }
            else if (node.Term.Name.Equals("instrucciones"))
            {
                foreach (var item in node.ChildNodes)
                {
                    Analize(item, instruccion);
                }
            }
            else if (node.Term.Name.Equals("proceso"))
            {
                if (node.ChildNodes.Count.Equals(6))
                {


                    string nombre = Analize(node.ChildNodes[0], instruccion);
                    Proceso nuevo = new Proceso(nombre);
                    nuevo.Tipo = Analize(node.ChildNodes[4], instruccion);
                    instruccion.procesos.Add(nuevo);
                    Analize(node.ChildNodes[2], instruccion);


                }
                else
                {

                    string nombre = Analize(node.ChildNodes[0], instruccion);
                    Proceso nuevo = new Proceso(nombre);
                    nuevo.Tipo = "f";
                    instruccion.procesos.Add(nuevo);
                }
            }
            else if (node.Term.Name.Equals("procesos"))
            {
                foreach (var item in node.ChildNodes)
                {

                    Analize(item, instruccion);
                }
            }

            else if (node.Term.Name.Equals("recursos"))
            {
                foreach (var item in node.ChildNodes)
                {
                    instruccion.procesos[instruccion.procesos.Count - 1].Recursos.Add(Int32.Parse(item.Token.Value.ToString().Remove(0, 1).ToString()));
                    instruccion.procesos[instruccion.procesos.Count - 1].Recursos.Sort();
                }
            }
            else if (node.Term.Name.Equals("tipo"))
            {
                return node.ChildNodes[0].Token.Value.ToString();
            }
            else if (node.Term.Name.Equals("proc"))
            {
                return node.Token.Value.ToString();
            }
            else if (node.Term.Name.Equals("tiempo"))
            {
                return node.Token.Value.ToString();
            }
            else if (node.Term.Name.Equals("rec"))
            {
                return node.Token.Value.ToString();
            }

            return "";
        }



        public void Ordenar() {


            for (int i = 0; i < instrucciones.Count; i++)
            {

                for (int j = 0; j < instrucciones[i].procesos.Count;j++ )
                {

                    if (instrucciones[i].procesos[j].Tipo.Equals("s"))
                    {
                        foreach (var rec in instrucciones[i].procesos[j].Recursos)
                        {
                            Proceso nuevo = this.Buscar_Recurso(rec, instrucciones[i].procesos[j]);

                            if (nuevo != null)
                            {
                                instrucciones[i].procesos[j] = nuevo;

                            }


                        }
                    }

                }


            }
            Console.Write("");

        }



        public void Ejecutar()
        {

            foreach (var inst in this.instrucciones)
            {
                textBox2.AppendText(inst.id + "-------------------------------------------------------------------------\n");
                ImprimirEstados();
                foreach (var process in inst.procesos)
                {

                    string rec = "";
                    string type = "";
                    string bloqueados = "";
                    Proceso nuevoproc = getProceso(process.Numero);
                    if (nuevoproc == null)
                    {
                        nuevoproc = new Proceso(process.Numero);
                        this.procesos.Add(nuevoproc);
                    }

                    if (process.Tipo.Equals("f"))
                    {

                        textBox2.AppendText("Se finalizo el proceso " + process.Numero + "\n");
                        TerminarProceso(process.Numero);
                    }
                    else
                    {

                        foreach (int recursos in process.Recursos)
                        {

                            if (process.Tipo.Equals("s"))
                            {

                                if (!BuscarRecursos(recursos))
                                {
                                    rec += "R" + recursos + ", ";
                                    nuevoproc.Recursos.Add(recursos);
                                    nuevoproc.Recursos.Sort();
                                    if (type == "")
                                    {
                                        type = "Se asigno ";
                                    }

                                }
                                else
                                {

                                    nuevoproc.Estado = false;
                                    nuevoproc.Esperando.Add(recursos);
                                    nuevoproc.Esperando.Sort();
                                    bloqueados += "R" + recursos + ",";
                                }
                            }

                            else if (process.Tipo.Equals("l"))
                            {
                                rec += "R" + recursos + ", ";
                                if (type == "")
                                {
                                    type = "Se libero ";
                                    this.LiberarRecurso(process.Numero, recursos);
                                    nuevoproc = null;
                                }
                            }
                        }
                    }
                    if (type != "")
                    {
                        textBox2.AppendText(type + rec + " a " + process.Numero + "\n");
                        if (nuevoproc != null)
                        {
                            if (nuevoproc.Estado)
                            {
                                textBox2.AppendText("El proceso " + process.Numero + " se esta ejecutando\n");
                            }
                            else
                            {
                                textBox2.AppendText(process.Numero + " en espera hasta que se libere " + bloqueados + "\n");
                            }
                        }
                    }
                    else
                    {
                        textBox2.AppendText(process.Numero + " en espera hasta que se libere " + bloqueados + "\n");
                    }

                }
                if (this.checkBox2.Checked)
                {
                    Interbloqueo();
                }
            }
        }

        public void Interbloqueo()
        {
            foreach (var item in this.procesos)
            {
                if (!item.Estado)
                {

                    recorre_inter(item);
                    break;
                }
            }

            if (dependencias.Count > 0)
            {
                dependencias.Clear();
            }
        }
        public void recorre_inter(Proceso proceso)
        {
            if (proceso != null && !proceso.Estado)
            {

                if (!Ingresar_nodo(proceso))
                {

                    foreach (var item in proceso.Recursos)
                    {
                        recorre_inter(this.Proceso_Recurso_Bloqueado(item, proceso));

                    }

                }


            }

        }
        public bool Ingresar_nodo(Proceso nodo)
        {
            bool band = false;
            if (dependencias.Count == 0)
            {
                dependencias.Push(nodo);
            }
            else
            {

                foreach (var item in dependencias)
                {
                    if (item.Numero.Equals(nodo.Numero))
                    {
                        band = true;
                        break;
                    }

                }
                if (!band)
                {
                    dependencias.Push(nodo);
                }
            }
            return band;
        }

        public Proceso Proceso_Recurso_Bloqueado(int num, Proceso proceso)
        {


            foreach (var item in this.procesos)
            {
                if (item.ExisteBloqueado(num) && !item.Estado && item.Numero != proceso.Numero)
                {

                    bool terminar = false;
                    foreach (var cola in this.dependencias)
                    {
                        if (cola.Numero.Equals(item.Numero))
                        {
                            terminar = true;

                            break;
                        }
                    }

                    if (terminar)
                    {
                        textBox2.AppendText("***Hay Interbloqueo***\n");
                        return null;
                    }
                    return item;
                }

            }


            return null;
        }



        public void ImprimirEstados()
        {

            foreach (var item in this.procesos)
            {
                if (item.Estado)
                {
                    textBox2.AppendText("El proceso " + item.Numero + " se esta ejecutando\n");
                }
                else
                {
                    textBox2.AppendText(item.Numero + " en espera hasta que se libere " + item.getBloqueados() + "\n");

                }
            }

        }
        public void LiberarRecurso(String proc, int num)
        {

            foreach (var item in this.procesos)
            {
                if (item.Numero.Equals(proc))
                {
                    item.LiberarRecurso(num);
                    break;
                }
            }

        }

        public Boolean BuscarRecursos(int num)
        {

            foreach (var item in this.procesos)
            {
                if (item.ExisteRecurso(num))
                {
                    return true;
                }
            }
            return false;
        }

        public Proceso Buscar_Recurso(int num, Proceso proc)
        {
            for (int i = 0; i < instrucciones.Count;i++ )
            {
                for (int j = 0; j < instrucciones[i].procesos.Count;j++ )
                {

                    if (instrucciones[i].procesos[j].Numero == proc.Numero && instrucciones[i].procesos[j].Existe_Recurso_Menor(num))
                    {
                        Proceso aux = instrucciones[i].procesos[j];
                        instrucciones[i].procesos[j] = proc;
                        return aux ;
                    }

                }

            }


            return null;
        }

        public void TerminarProceso(string nombre)
        {
            foreach (var item in procesos)
            {
                if (item.Numero.Equals(nombre))
                {
                    procesos.Remove(item);
                    break;
                }

            }
        }

        public Proceso getProceso(String number)
        {

            foreach (var item in this.procesos)
            {
                if (item.Numero.Equals(number))
                {
                    return item;
                }

            }
            return null;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
