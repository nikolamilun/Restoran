using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.ServiceModel;
using Common;

namespace Klijent
{
    public partial class GlavnaForma : Form
    {
        static IRestoran proxy;
        DataSet dsPodaci;
        public GlavnaForma()
        {
            InitializeComponent();
        }

        void osveziKontroleNaMeniju()
        {
            // Ubacivanje opcija u comboBox-ove za tabelu "Na meniju"
            foreach (DataRow dr in dsPodaci.Tables["meni"].Rows)
                cbIDMenijaNaMeniju.Items.Add(dr["id_menija"]);
            foreach (DataRow dr in dsPodaci.Tables["jelo"].Rows)
                cbIDJelaNaMeniju.Items.Add(dr["id_jela"]);
        }

        void osveziKontroleSeSastoji()
        {
            // Ubacivanje opcija u comboBox-ove za tabelu "Se sastoji"
            foreach (DataRow dr in dsPodaci.Tables["sastojci"].Rows)
                cbIDSastojkaSeSastoji.Items.Add(dr["id_sastojka"]);
            foreach (DataRow dr in dsPodaci.Tables["jelo"].Rows)
                cbIDJelaSeSastoji.Items.Add(dr["id_jela"]);
        }

        void osveziKontroleSpecPretraga()
        {
            // Ubacivanje opcija u comboBox-ove za specijalne pretrage
            foreach (DataRow dr in dsPodaci.Tables["sastojci"].Rows)
                cbSastojakJelaPrema.Items.Add(dr["naziv_sastojka"]);
            foreach (DataRow dr in dsPodaci.Tables["meni"].Rows)
                cbSastojakJelaPrema.Items.Add(dr["naziv_sastojka"]);
        }

        private void GlavnaForma_Load(object sender, EventArgs e)
        {
            // Otvaranje proksija za serverske radnje
            ChannelFactory<IRestoran> chf  = new ChannelFactory<IRestoran>(new BasicHttpBinding(), new EndpointAddress("http://localhost:8000"));
            proxy = chf.CreateChannel();
            // Preuzimanje dataSeta sa podacima iz baze i ubacivanje podataka u formu
            dsPodaci = proxy.PreuzmiDataSet();
            dgwMeni.DataSource = dsPodaci.Tables["meni"];
            dgwJelo.DataSource = dsPodaci.Tables["jelo"];
            dgwSastojak.DataSource = dsPodaci.Tables["sastojci"];
            dgwNaMeniju.DataSource = dsPodaci.Tables["naMeniju"];
            dgwSeSastoji.DataSource = dsPodaci.Tables["seSastoji"];
            osveziKontroleNaMeniju();
            osveziKontroleSeSastoji();
            osveziKontroleSpecPretraga();
            // Upisivanje imena tabela u comboBox za biranje tabele
            foreach (DataTable dt in dsPodaci.Tables)
                cbIzaberiteTabelu.Items.Add(dt.TableName);
        }
    }
}
