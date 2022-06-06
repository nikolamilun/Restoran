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

        #region MsgBox funkcije
        private void MsgBoxError(string errorMsg)
        {
            MessageBox.Show(errorMsg, "Greska!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private DialogResult MsgBoxQuestion(string questionMsg)
        {
            return MessageBox.Show(questionMsg, "Pitanje", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        private void MsgBoxInfo(string infoMsg)
        {
            MessageBox.Show(infoMsg, "Obavestenje", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion

        #region Osvezavanje kontrola
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
                cbSastojakJelaPrema.Items.Add(dr["naziv_menija"]);
        }
        #endregion

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

        #region Popunjavanje kontrola podacima
        // Kontrole se popunjavaju podacima iz izabranog reda
        private void dgwMeni_SelectionChanged(object sender, EventArgs e)
        {
            if (dgwMeni.SelectedRows.Count != 0)
            {
                txtIDMenijaMeni.Text = dgwMeni.SelectedRows[0].Cells[0].Value.ToString();
                txtNazivMenijaMeni.Text = dgwMeni.SelectedRows[0].Cells[1].Value.ToString();
                chbMeniAktivanMeni.Checked = (bool)dgwMeni.SelectedRows[0].Cells[2].Value;
            }
        }

        private void dgwJelo_SelectionChanged(object sender, EventArgs e)
        {
            if (dgwJelo.SelectedRows.Count != 0)
            {
                txtIDJelaJelo.Text = dgwJelo.SelectedRows[0].Cells[0].Value.ToString();
                txtNazivJelaJelo.Text = dgwJelo.SelectedRows[0].Cells[1].Value.ToString();
                txtOpisJelaJelo.Text = dgwJelo.SelectedRows[0].Cells[2].Value.ToString();
            }
        }

        private void dgwSastojak_SelectionChanged(object sender, EventArgs e)
        {
            if (dgwSastojak.SelectedRows.Count != 0)
            {
                txtIDSastojkaSastojak.Text = dgwSastojak.SelectedRows[0].Cells[0].Value.ToString();
                txtNazivSastojkaSastojak.Text = dgwSastojak.SelectedRows[0].Cells[1].Value.ToString();
                txtOpisSastojkaSastojak.Text = dgwSastojak.SelectedRows[0].Cells[2].Value.ToString();
            }
        }

        private void dgwNaMeniju_SelectionChanged(object sender, EventArgs e)
        {
            if (dgwNaMeniju.SelectedRows.Count != 0)
            {
                cbIDMenijaNaMeniju.SelectedItem = dgwNaMeniju.SelectedRows[0].Cells[0].Value;
                cbIDJelaNaMeniju.SelectedItem = dgwNaMeniju.SelectedRows[0].Cells[1].Value;
                txtCenaNaMeniju.Text = dgwNaMeniju.SelectedRows[0].Cells[2].Value.ToString();
            }
        }

        private void dgwSeSastoji_SelectionChanged(object sender, EventArgs e)
        {
            if (dgwSeSastoji.SelectedRows.Count != 0)
            {
                cbIDJelaSeSastoji.SelectedItem = dgwSeSastoji.SelectedRows[0].Cells[0].Value;
                cbIDSastojkaSeSastoji.SelectedItem = dgwSeSastoji.SelectedRows[0].Cells[1].Value;
            }
        }

        #endregion

        #region Kontrola unosa
        private void chbMZUMeni_CheckedChanged(object sender, EventArgs e)
        {
            btnDodajRedMeni.Enabled = !btnDodajRedMeni.Enabled;
            btnObrisiRedMeni.Enabled = !btnObrisiRedMeni.Enabled;
            btnIzmeniRedMeni.Enabled = !btnIzmeniRedMeni.Enabled;
            txtIDMenijaMeni.Enabled = !txtIDMenijaMeni.Enabled;
        }

        private void chbMZUJelo_CheckedChanged(object sender, EventArgs e)
        {
            btnDodajRedJelo.Enabled = !btnDodajRedJelo.Enabled;
            btnObrisiRedJelo.Enabled = !btnObrisiRedJelo.Enabled;
            btnIzmeniRedJelo.Enabled = !btnIzmeniRedJelo.Enabled;
            txtIDJelaJelo.Enabled = !txtIDJelaJelo.Enabled;
        }

        private void chbMZUSastojak_CheckedChanged(object sender, EventArgs e)
        {
            btnDodajRedSastojak.Enabled = !btnDodajRedSastojak.Enabled;
            btnObrisiRedSastojak.Enabled = !btnObrisiRedSastojak.Enabled;
            btnIzmeniRedSastojak.Enabled = !btnIzmeniRedSastojak.Enabled;
            txtIDSastojkaSastojak.Enabled = !txtIDSastojkaSastojak.Enabled;
        }

        private void chbMZUNaMeniju_CheckedChanged(object sender, EventArgs e)
        {
            btnDodajRedNaMeniju.Enabled = !btnDodajRedNaMeniju.Enabled;
            btnObrisiRedNaMeniju.Enabled = !btnObrisiRedNaMeniju.Enabled;
            btnIzmeniRedNaMeniju.Enabled = !btnIzmeniRedNaMeniju.Enabled;
        }

        private void chbMZUSeSastoji_CheckedChanged(object sender, EventArgs e)
        {

        }
        #endregion

        #region Dodavanje, Izmena, Brisanje
        private void btnObrisiRedMeni_Click(object sender, EventArgs e)
        {
            if (MsgBoxQuestion("Da li stvarno zelite da izbrisete taj red?") == DialogResult.Yes)
            {
                int id = (int)dgwMeni.SelectedRows[0].Cells[0].Value;
                dgwMeni.Rows.Remove(dgwMeni.SelectedRows[0]);
                dgwMeni.DataSource = dsPodaci.Tables["meni"];
                MsgBoxInfo("Red uspesno obrisan!");
            }
            else
            {
                MsgBoxInfo("Red nije izbrisan");
            }
        }

        private void btnDodajRedMeni_Click(object sender, EventArgs e)
        {
            try
            {
                string naziv = txtNazivMenijaMeni.Text;
                bool aktivan = chbMeniAktivanMeni.Checked;
                int id = proxy.InsertIntoMeni(naziv, aktivan);
                DataRow dr = dsPodaci.Tables["meni"].NewRow();
                dr["id_menija"] = id;
                dr["naziv_menija"] = naziv;
                dr["aktivan"] = aktivan;
                dsPodaci.Tables["meni"].Rows.Add(dr);
                dgwMeni.DataSource = dsPodaci.Tables["meni"];
            }
            catch (Exception exc)
            {
                MsgBoxError("Tekst greske: " + exc.Message);
            }
        }

        #endregion

        private void GlavnaForma_FormClosing(object sender, FormClosingEventArgs e)
        {
            proxy.vratiDataSet(dsPodaci);
        }

        private void btnIzmeniRedMeni_Click(object sender, EventArgs e)
        {
            try
            {
                if (MsgBoxQuestion("Da li zelite da izmenite ovaj red?") == DialogResult.Yes)
                {
                    int id = int.Parse(txtIDMenijaMeni.Text);
                    string naziv = txtNazivMenijaMeni.Text;
                    bool aktivan = chbMeniAktivanMeni.Checked;
                    foreach (DataRow dr in dsPodaci.Tables["meni"].Rows)
                        if (dr["id_menija"] == dgwMeni.SelectedRows[0].Cells[0].Value)
                        {
                            dr["id_menija"] = id;
                            dr["naziv_menija"] = naziv;
                            dr["aktivan"] = aktivan;
                        }
                    dgwMeni.DataSource = dsPodaci.Tables["meni"];
                    MsgBoxInfo("Red je izmenjen!");
                }
                else
                    MsgBoxInfo("Red se ne menja.");
            }
            catch (Exception exc)
            {
                MsgBoxError("Greska:" +  exc.Message);
            }
        }
    }
}
