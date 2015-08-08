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
        List<Tiempo> Cola;
        List<Proceso> procesos;
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
            if (raiz.Root != null)
            {
                this.Analize(raiz.Root, null);
                Ejecutar();
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


        public void Ejecutar()
        {

            foreach (var inst in this.instrucciones)
            {
                textBox2.AppendText(inst.id + "---------------------------------------------\n");
                foreach (var process in inst.procesos)
                {

                    string rec = "";
                    string type = "";
                    Proceso nuevoproc = new Proceso(process.Numero);
                    if (process.Tipo.Equals("f"))
                    {

                        textBox2.AppendText("Se finalizo el proceso " + process.Numero + "\n");
                        TerminarProceso(process);
                    }
                    else
                    {
                       
                        foreach (var recursos in process.Recursos)
                        {

                            if (process.Tipo.Equals("s"))
                            {

                                if (!BuscarRecursos(recursos))
                                {

                                    rec += "R" + recursos + ", ";
                                    nuevoproc.Recursos.Add(recursos);
                                    if (type == "")
                                    {
                                        type = "Se asigno ";
                                    }

                                }
                                else
                                {
                                     textBox2.AppendText(process.Numero + " en espera hasta que se libere " + recursos + "\n");
                                }
                            }

                            else if (process.Tipo.Equals("l"))
                            {
                                rec += "R" + recursos + ", ";
                                if (type == "")
                                {
                                    type = "Se libero ";
                                }
                            }

                        }
                    }
                    if (type != "")
                    {
                        textBox2.AppendText(type + rec + " a " + process.Numero + "\n");
                        textBox2.AppendText("El proceso " + process.Numero + " se esta ejecutando\n");
                        procesos.Add(nuevoproc);
                    }
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

        public void TerminarProceso(Proceso proceso)
        {
            this.procesos.Remove(proceso);
        }

    }
}
