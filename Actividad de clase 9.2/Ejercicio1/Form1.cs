using Ejercicio1.Models;
using Ejercicio1.Models.Exportadores;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ejercicio1
{
    public partial class Form1 : Form
    {
        //List<Multa> multas;
        List<IExportable>multas  = new List<IExportable>();

        public Form1()
        {
            InitializeComponent();
            //multas = new List<Multa>();
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            IExportable nuevo = null;
            string patente;
            DateTime vencimiento;
            double importe;

            try
            {
                patente = tbPatente.Text;
                //vencimiento = Convert.ToDateTime( dtpVencimiento.Text);
                //vencimiento = dtpVencimiento.Value;
                vencimiento = new DateTime(dtpVencimiento.Value.Year,dtpVencimiento.Value.Month,dtpVencimiento.Value.Day);

                importe = Convert.ToDouble(tbImporte.Text);

                //Multa unaMulta = new Multa(patente, vencimiento, importe);
                //if (unaMulta != null ) { multas.Add(unaMulta); }
                
                nuevo = new Multa(patente, vencimiento.Date, importe);

                try
                {
                    //ver si es actualziar o es para agregar uno nuevo
                    multas.Sort();

                    int idx = multas.BinarySearch(nuevo);
                    if (idx >= 0)// Actualizo los datos
                    {
                        Multa multa = multas[idx] as Multa;
                        multa.Importe = importe;
                        if (multa.Vencimiento < ((Multa)nuevo).Vencimiento) { multa.Vencimiento = ((Multa)nuevo).Vencimiento; }
                    }
                    else // agrego uno nuevo
                    {
                        multas.Add(nuevo);
                    }

                }catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    MessageBox.Show("Error dentro de comparar ");
                }

                btnActualizar.PerformClick();

                tbPatente.Clear();
                tbImporte.Clear();
                dtpVencimiento.Value = DateTime.Now;




            }
            catch (ValidaPatenteException pex) { MessageBox.Show(pex.Message, "Formato", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            catch(Exception ex) { MessageBox.Show(ex.Message, "UPs!", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            lsbVer.Items.Clear();
            foreach(IExportable multa in multas)
            {
                lsbVer.Items.Add(multa);
            }
        }

        private void lsbVer_SelectedIndexChanged(object sender, EventArgs e)
        {
            Multa multaSelect = lsbVer.SelectedItem as Multa;
            try
            {
                if (multaSelect != null)
                {
                    dtpVencimiento.Value = multaSelect.Vencimiento;
                    tbImporte.Text = multaSelect.Importe.ToString();
                    tbPatente.Text = multaSelect.Patente;

                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Ups!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show("Error en Listbox Select Cahnge");
            }
        }

        private void btnImportar_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "(csv)|*.csv|(json)|*.json|(txt)|*.txt|(xml)|*.xml";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string path = openFileDialog1.FileName;
                int tipo = openFileDialog1.FilterIndex;

                IExportador exportador = (new ExportadorFactory()).GetInstance(tipo);

                FileStream fs = null;
                StreamReader sr = null;
                try
                {
                    fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                    sr = new StreamReader(fs);

                    //descarto la cabecera
                    sr.ReadLine();

                    while (!sr.EndOfStream)
                    {
                        string linea = sr.ReadLine();
                        IExportable nuevo = new Multa();

                        if (nuevo.Importar(linea, exportador) == true)
                        {
                            int idx = multas.BinarySearch(nuevo);
                            if (idx >= 0)
                            {
                                Multa multa = multas[idx] as Multa;
                                multa.Importe += ((Multa)nuevo).Importe;
                                if (multa.Vencimiento < ((Multa)nuevo).Vencimiento) ;
                                multa.Vencimiento = ((Multa)nuevo).Vencimiento;
                            }
                            else
                                multas.Add(nuevo);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    if (sr != null)
                        sr.Close();
                    if (fs != null)
                        fs.Close();
                }
            }
            btnActualizar.PerformClick();
        }
    }
}
