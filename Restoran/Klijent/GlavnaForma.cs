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
    enum PoljaMenija { id_menija, naziv_menija, aktivan}
    enum PoljaJela { id_jela, naziv_jela, opis_jela}
    enum PoljaSastojka { id_sastojka, naziv_sastojka, opis_sastojka}
    enum PoljaNaMeniju { id_menija, id_jela, cena}
    enum PoljaSeSastoji { id_jela, id_sastojka, kolicina }
    public partial class GlavnaForma : Form
    {
        static IRestoran proxy;
        DataSet dsPodaci;
        public GlavnaForma()
        {
            InitializeComponent();
        }

        #region Pomocne funkcije
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

        private int obradiComboBoxItem(object item)
        {
            // Iz combo box-a se izdvaja ID izabranog elementa, trimuje se i pretvara u int
            return int.Parse(item.ToString().Split('-')[0].Trim());
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
                cbIDSastojkaSeSastoji.Items.Add(dr["id_sastojka"].ToString() + " - " + dr["naziv_sastojka"]);
            foreach (DataRow dr in dsPodaci.Tables["jelo"].Rows)
                cbIDJelaSeSastoji.Items.Add(dr["id_jela"].ToString() + " - " +  dr["naziv_jela"]);
        }

        void osveziKontroleSpecPretraga()
        {
            // Ubacivanje opcija u comboBox-ove za specijalne pretrage
            foreach (DataRow dr in dsPodaci.Tables["sastojci"].Rows)
                cbSastojakJelaPrema.Items.Add(dr["id_sastojka"].ToString() + " - " + dr["naziv_sastojka"] );
            foreach (DataRow dr in dsPodaci.Tables["meni"].Rows)
                cbMeniJelaPrema.Items.Add(dr["id_menija"].ToString() + " - " + dr["naziv_menija"]);
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

        private void GlavnaForma_FormClosing(object sender, FormClosingEventArgs e)
        {
            proxy.vratiDataSet(dsPodaci);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            dgwSpecijalnaPretraga.DataSource = null;
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
        }

        private void chbMZUJelo_CheckedChanged(object sender, EventArgs e)
        {
            btnDodajRedJelo.Enabled = !btnDodajRedJelo.Enabled;
            btnObrisiRedJelo.Enabled = !btnObrisiRedJelo.Enabled;
            btnIzmeniRedJelo.Enabled = !btnIzmeniRedJelo.Enabled;
        }

        private void chbMZUSastojak_CheckedChanged(object sender, EventArgs e)
        {
            btnDodajRedSastojak.Enabled = !btnDodajRedSastojak.Enabled;
            btnObrisiRedSastojak.Enabled = !btnObrisiRedSastojak.Enabled;
            btnIzmeniRedSastojak.Enabled = !btnIzmeniRedSastojak.Enabled;
        }

        private void chbMZUNaMeniju_CheckedChanged(object sender, EventArgs e)
        {
            btnDodajRedNaMeniju.Enabled = !btnDodajRedNaMeniju.Enabled;
            btnObrisiRedNaMeniju.Enabled = !btnObrisiRedNaMeniju.Enabled;
            btnIzmeniRedNaMeniju.Enabled = !btnIzmeniRedNaMeniju.Enabled;
        }

        private void chbMZUSeSastoji_CheckedChanged(object sender, EventArgs e)
        {
            btnDodajRedSeSastoji.Enabled = !btnDodajRedSeSastoji.Enabled;
            btnObrisiRedSeSastoji.Enabled = !btnObrisiRedSeSastoji.Enabled;
            btnIzmeniRedSeSastoji.Enabled = !btnIzmeniRedSeSastoji.Enabled;
        }

        private void chbMZUSeSastoji_CheckedChanged_1(object sender, EventArgs e)
        {
            btnDodajRedSeSastoji.Enabled = !btnDodajRedSeSastoji.Enabled;
            btnObrisiRedSeSastoji.Enabled = !btnObrisiRedSeSastoji.Enabled;
            btnIzmeniRedSeSastoji.Enabled = !btnIzmeniRedSeSastoji.Enabled;
        }
        #endregion

        #region Dodavanje, Izmena, Brisanje
        private void btnObrisiRedMeni_Click(object sender, EventArgs e)
        {
            // Potvrda brisanja
            if (MsgBoxQuestion("Da li stvarno zelite da izbrisete taj red?") == DialogResult.Yes)
            {
                dgwMeni.Rows.Remove(dgwMeni.SelectedRows[0]);
                dgwMeni.DataSource = dsPodaci.Tables["meni"];
                MsgBoxInfo("Red uspesno obrisan!");
                osveziKontroleNaMeniju();
                osveziKontroleSpecPretraga();
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
                MsgBoxInfo("Red je uspesno unet!");
                osveziKontroleNaMeniju();
                osveziKontroleSpecPretraga();
            }
            catch (Exception exc)
            {
                MsgBoxError("Tekst greske: " + exc.Message);
            }
        }

        private void btnIzmeniRedMeni_Click(object sender, EventArgs e)
        {
            try
            {
                // Potvrda izmene
                if (MsgBoxQuestion("Da li zelite da izmenite ovaj red?") == DialogResult.Yes)
                {
                    string naziv = txtNazivMenijaMeni.Text;
                    bool aktivan = chbMeniAktivanMeni.Checked;
                    foreach (DataRow dr in dsPodaci.Tables["meni"].Rows)
                    {
                        if (dr["id_menija"].ToString() == dgwMeni.SelectedRows[0].Cells[0].Value.ToString())
                        {
                            dr["naziv_menija"] = naziv;
                            dr["aktivan"] = aktivan;
                        }
                    }
                    dgwMeni.DataSource = dsPodaci.Tables["meni"];
                    MsgBoxInfo("Red je izmenjen!");
                }
                else
                    MsgBoxInfo("Red se ne menja.");
            }
            catch (Exception exc)
            {
                MsgBoxError("Greska:" + exc.Message);
            }
        }

        private void btnObrisiRedJelo_Click(object sender, EventArgs e)
        {
            // Potvrda brisanja
            if (MsgBoxQuestion("Da li stvarno zelite da izbrisete taj red?") == DialogResult.Yes)
            {
                dgwJelo.Rows.Remove(dgwJelo.SelectedRows[0]);
                dgwJelo.DataSource = dsPodaci.Tables["jelo"];
                osveziKontroleSeSastoji();
                osveziKontroleNaMeniju();
                MsgBoxInfo("Red uspesno obrisan!");
            }
            else
            {
                MsgBoxInfo("Red nije izbrisan");
            }
        }

        private void btnDodajRedJelo_Click(object sender, EventArgs e)
        {
            try
            {
                string naziv = txtNazivJelaJelo.Text;
                string opis = txtOpisJelaJelo.Text;
                int id = proxy.InsertIntoJelo(naziv, opis);
                DataRow dr = dsPodaci.Tables["jelo"].NewRow();
                dr["id_jela"] = id;
                dr["naziv_jela"] = naziv;
                dr["opis_jela"] = opis;
                dsPodaci.Tables["jelo"].Rows.Add(dr);
                dgwJelo.DataSource = dsPodaci.Tables["jelo"];
                osveziKontroleNaMeniju();
                osveziKontroleSeSastoji();
                MsgBoxInfo("Red je uspesno unet!");
            }
            catch (Exception exc)
            {
                MsgBoxError("Tekst greske: " + exc.Message);
            }
        }

        private void btnIzmeniRedJelo_Click(object sender, EventArgs e)
        {
            try
            {
                // Potvrda izmene
                if (MsgBoxQuestion("Da li zelite da izmenite ovaj red?") == DialogResult.Yes)
                {
                    string naziv = txtNazivJelaJelo.Text;
                    string opis = txtOpisJelaJelo.Text;
                    foreach (DataRow dr in dsPodaci.Tables["jelo"].Rows)
                    {
                        if (dr["id_jela"].ToString() == dgwJelo.SelectedRows[0].Cells[0].Value.ToString())
                        {
                            dr["naziv_jela"] = naziv;
                            dr["opis_jela"] = opis;
                        }
                    }
                    dgwJelo.DataSource = dsPodaci.Tables["jelo"];
                    MsgBoxInfo("Red je izmenjen!");
                }
                else
                    MsgBoxInfo("Red se ne menja.");
            }
            catch (Exception exc)
            {
                MsgBoxError("Greska:" + exc.Message);
            }
        }

        private void btnObrisiRedSastojak_Click(object sender, EventArgs e)
        {
            // Potvrda brisanja
            if (MsgBoxQuestion("Da li stvarno zelite da izbrisete taj red?") == DialogResult.Yes)
            {
                dgwSastojak.Rows.Remove(dgwSastojak.SelectedRows[0]);
                dgwSastojak.DataSource = dsPodaci.Tables["sastojci"];
                osveziKontroleSpecPretraga();
                osveziKontroleSeSastoji();
                MsgBoxInfo("Red uspesno obrisan!");
            }
            else
            {
                MsgBoxInfo("Red nije izbrisan");
            }
        }

        private void btnDodajRedSastojak_Click(object sender, EventArgs e)
        {
            try
            {
                string naziv = txtNazivSastojkaSastojak.Text;
                string opis = txtOpisSastojkaSastojak.Text;
                int id = proxy.InsertIntoSastojak(naziv, opis);
                DataRow dr = dsPodaci.Tables["sastojci"].NewRow();
                dr["id_sastojak"] = id;
                dr["naziv_sastojka"] = naziv;
                dr["opis_sastojka"] = opis;
                dsPodaci.Tables["sastojak"].Rows.Add(dr);
                dgwSastojak.DataSource = dsPodaci.Tables["sastojci"];
                osveziKontroleSpecPretraga();
                osveziKontroleSeSastoji();
                MsgBoxInfo("Red je uspesno unet!");
            }
            catch (Exception exc)
            {
                MsgBoxError("Tekst greske: " + exc.Message);
            }
        }

        private void btnIzmeniRedSastojak_Click(object sender, EventArgs e)
        {
            try
            {
                // Potvrda izmene
                if (MsgBoxQuestion("Da li zelite da izmenite ovaj red?") == DialogResult.Yes)
                {
                    string naziv = txtNazivSastojkaSastojak.Text;
                    string opis = txtOpisSastojkaSastojak.Text;
                    foreach (DataRow dr in dsPodaci.Tables["sastojci"].Rows)
                    {
                        if (dr["id_sastojka"].ToString() == dgwSastojak.SelectedRows[0].Cells[0].Value.ToString())
                        {
                            dr["naziv_sastojka"] = naziv;
                            dr["opis_sastojka"] = opis;
                        }
                    }
                    dgwSastojak.DataSource = dsPodaci.Tables["sastojci"];
                    MsgBoxInfo("Red je izmenjen!");
                }
                else
                    MsgBoxInfo("Red se ne menja.");
            }
            catch (Exception exc)
            {
                MsgBoxError("Greska:" + exc.Message);
            }
        }

        private void btnObrisiRedNaMeniju_Click(object sender, EventArgs e)
        {
            // Potvrda brisanja
            if (MsgBoxQuestion("Da li stvarno zelite da izbrisete taj red?") == DialogResult.Yes)
            {
                dgwNaMeniju.Rows.Remove(dgwNaMeniju.SelectedRows[0]);
                dgwNaMeniju.DataSource = dsPodaci.Tables["naMeniju"];
                MsgBoxInfo("Red uspesno obrisan!");
            }
            else
            {
                MsgBoxInfo("Red nije izbrisan");
            }
        }

        private void btnDodajRedNaMeniju_Click(object sender, EventArgs e)
        {
            try
            {
                int idjela = obradiComboBoxItem(cbIDJelaNaMeniju.SelectedItem);
                int idmenija = obradiComboBoxItem(cbIDMenijaNaMeniju.SelectedItem);
                // Odredjuje da li je korisnik izabrao dva ID-ja koja vec postoje u nekom redu
                bool validno = true;
                foreach (DataRow dr in dsPodaci.Tables["naMeniju"].Rows)
                    if ((int)dr["id_jela"] == idjela && (int)dr["id_menija"] == idmenija)
                        validno = false;
                // Ako nije, red se unosi
                if (validno)
                {
                    int cena = int.Parse(txtCenaNaMeniju.Text);
                    DataRow dr = dsPodaci.Tables["naMeniju"].NewRow();
                    dr["id_menija"] = idmenija;
                    dr["id_jela"] = idjela;
                    dr["cena"] = cena;
                    dsPodaci.Tables["naMeniju"].Rows.Add(dr);
                    dgwNaMeniju.DataSource = dsPodaci.Tables["naMeniju"];
                }
                else
                    MsgBoxError("Vec postoji objekat sa tim identitetom! Unesite drugacije ID-jeve!");
            }
            catch (Exception exc)
            {
                MsgBoxError("Tekst greske: " + exc.Message);
            }
        }

        private void btnIzmeniRedNaMeniju_Click(object sender, EventArgs e)
        {
            try
            {
                // Potvrda izmene
                if (MsgBoxQuestion("Da li zelite da izmenite ovaj red?") == DialogResult.Yes)
                {
                    int idjela = obradiComboBoxItem(cbIDJelaNaMeniju.SelectedItem);
                    int idmenija = obradiComboBoxItem(cbIDMenijaNaMeniju.SelectedItem);
                    int cena = int.Parse(txtCenaNaMeniju.Text);
                    foreach (DataRow dr in dsPodaci.Tables["naMeniju"].Rows)
                    {
                        if (dr["id_menija"].ToString() == dgwNaMeniju.SelectedRows[0].Cells[0].Value.ToString() && dr["id_jela"].ToString() == dgwNaMeniju.SelectedRows[0].Cells[1].Value.ToString())
                        {
                            dr["id_menija"] = idmenija;
                            dr["id_jela"] = idjela;
                            dr["cena"] = cena;
                        }
                    }
                    dgwNaMeniju.DataSource = dsPodaci.Tables["naMeniju"];
                    MsgBoxInfo("Red je izmenjen!");
                }
                else
                    MsgBoxInfo("Red se ne menja.");
            }
            catch (Exception exc)
            {
                MsgBoxError("Greska:" + exc.Message);
            }
        }

        private void btnObrisiRedSeSastoji_Click(object sender, EventArgs e)
        {
            // Potvrda brisanja
            if (MsgBoxQuestion("Da li stvarno zelite da izbrisete taj red?") == DialogResult.Yes)
            {
                dgwSeSastoji.Rows.Remove(dgwSeSastoji.SelectedRows[0]);
                dgwSeSastoji.DataSource = dsPodaci.Tables["seSastoji"];
                MsgBoxInfo("Red uspesno obrisan!");
            }
            else
            {
                MsgBoxInfo("Red nije izbrisan");
            }
        }

        private void btnDodajRedSeSastoji_Click(object sender, EventArgs e)
        {
            try
            {
                int idsastojka = obradiComboBoxItem(cbIDSastojkaSeSastoji.SelectedItem);
                int idjela = obradiComboBoxItem(cbIDJelaSeSastoji.SelectedItem);
                int kolicina = int.Parse(txtKolicinaSeSastoji.Text);
                // Odredjuje da li je korisnik izabrao dva ID-ja koja vec postoje u nekom redu
                bool validno = true;
                foreach (DataRow dr in dsPodaci.Tables["seSastoji"].Rows)
                    if ((int)dr["id_jela"] == idjela && (int)dr["id_sastojka"] == idsastojka)
                        validno = false;
                // Ako nije, red se menja
                if (validno)
                {
                    DataRow dr = dsPodaci.Tables["seSastoji"].NewRow();
                    dr["id_sastojka"] = idsastojka;
                    dr["id_jela"] = idjela;
                    dr["kolicina"] = kolicina;
                    dsPodaci.Tables["seSastoji"].Rows.Add(dr);
                    dgwSeSastoji.DataSource = dsPodaci.Tables["seSastoji"];
                }
                else
                    MsgBoxError("Vec postoji objekat sa tim identitetom! Unesite drugacije ID-jeve!");
            }
            catch (Exception exc)
            {
                MsgBoxError("Tekst greske: " + exc.Message);
            }
        }

        private void btnIzmeniRedSeSastoji_Click(object sender, EventArgs e)
        {
            try
            {
                // Potvrda izmene
                if (MsgBoxQuestion("Da li zelite da izmenite ovaj red?") == DialogResult.Yes)
                {
                    int idsastojka = obradiComboBoxItem(cbIDSastojkaSeSastoji.SelectedItem);
                    int idjela = obradiComboBoxItem(cbIDJelaSeSastoji.SelectedItem);
                    int kolicina = int.Parse(txtKolicinaSeSastoji.Text);
                    foreach (DataRow dr in dsPodaci.Tables["seSastoji"].Rows)
                    {
                        if (dr["id_sastojka"].ToString() == dgwNaMeniju.SelectedRows[0].Cells[0].Value.ToString() && dr["id_jela"].ToString() == dgwNaMeniju.SelectedRows[0].Cells[1].Value.ToString())
                        {
                            dr["id_sastojka"] = idsastojka;
                            dr["id_jela"] = idjela;
                            dr["cena"] = kolicina;
                        }
                    }
                    dgwSeSastoji.DataSource = dsPodaci.Tables["seSastoji"];
                    MsgBoxInfo("Red je izmenjen!");
                }
                else
                    MsgBoxInfo("Red se ne menja.");
            }
            catch (Exception exc)
            {
                MsgBoxError("Greska:" + exc.Message);
            }
        }

        #endregion

        #region Specijalne pretrage
        private void btnPretraziJelaMeniCena_Click(object sender, EventArgs e)
        {
            try
            {
                List<Jelo> pretraga = proxy.jelaNaMenijimaKojiSuAktivniSaCenom(int.Parse(txtCenaSpecPretraga.Text));
                dgwSpecijalnaPretraga.DataSource = pretraga;
            }
            catch (Exception exc)
            {
                MsgBoxError("Greska: " + exc.Message);
            }
        }

        private void btnPretraziJelaBrSastojci_Click(object sender, EventArgs e)
        {
            try
            {
                List<Jelo> pretraga = proxy.jelaPremaBrojuSastojaka(int.Parse(txtBrojSastojakaJela.Text));
                dgwSpecijalnaPretraga.DataSource = pretraga;
            }
            catch (Exception exc)
            {
                MsgBoxError("Greska: " + exc.Message);
            }
        }

        private void btnPretraziJelaMeni_Click(object sender, EventArgs e)
        {
            try
            {
                List<Jelo> pretraga = proxy.jelaPremaMeniju(obradiComboBoxItem(cbMeniJelaPrema.SelectedItem));
                dgwSpecijalnaPretraga.DataSource = pretraga;
            }
            catch (Exception exc)
            {
                MsgBoxError("Greska: " + exc.Message);
            }
        }

        private void btnPretraziJelaSastojak_Click(object sender, EventArgs e)
        {
            try
            {
                List<Jelo> pretraga = proxy.jelaPremaSastojku(obradiComboBoxItem(cbSastojakJelaPrema.SelectedItem));
                dgwSpecijalnaPretraga.DataSource = pretraga;
            }
            catch (Exception exc)
            {
                MsgBoxError("Greska: " + exc.Message);
            }
        }

        private void cbIzaberiteTabelu_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbIzaberiteTabelu.SelectedIndex) 
            {
                case 0:
                    {
                        cbIzaberiteParametar.DataSource = PoljaJela.GetNames(typeof(PoljaJela));
                        break;
                    }
                case 1:
                    {
                        cbIzaberiteParametar.DataSource = PoljaMenija.GetNames(typeof(PoljaMenija));
                        break;
                    }
                case 2:
                    {
                        cbIzaberiteParametar.DataSource = PoljaSastojka.GetNames(typeof(PoljaSastojka));
                        break;
                    }
                case 3:
                    {
                        cbIzaberiteParametar.DataSource = PoljaNaMeniju.GetNames(typeof(PoljaNaMeniju));
                        break;
                    }
                case 4:
                    {
                        cbIzaberiteParametar.DataSource = PoljaSeSastoji.GetNames(typeof(PoljaSeSastoji));
                        break;
                    }
                default:
                    {
                        cbIzaberiteParametar.DataSource = null;
                        break;
                    }
            }
        }

        private void btnPretraziParametri_Click(object sender, EventArgs e)
        {
            if (cbIzaberiteParametar.SelectedItem != null)
            {
                string tabela = cbIzaberiteTabelu.SelectedItem.ToString();
                // Ova promenljiva sluzi da bi funkcija TryParse radila (tu postoji da bi proverila da li je string broj)
                int n;
                string polje = cbIzaberiteParametar.SelectedItem.ToString();
                string vrednost = txtVrednostParametra.Text;
                DataTable podaci = dsPodaci.Tables[tabela].Clone();
                podaci.Clear();
                foreach (DataRow dr in dsPodaci.Tables[tabela].Rows)
                {
                    if(!int.TryParse(dr[polje].ToString(), out n) && dr[polje].ToString().ToLower() != "false" && dr[polje].ToString().ToLower() != "true")
                        if (dr[polje].ToString().Contains(vrednost))
                            podaci.ImportRow(dr);
                }
                dgwPretragaPoParametrima.DataSource = podaci;
            }
        }
        #endregion
    }
}
