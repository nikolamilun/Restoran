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
    // Ovi enum-ovi sluze da bi se olaksalo ispunjavanje comboBoxa u pretrazi poljima izabrane tabele (linija 669)
    enum PoljaMenija { id_menija, naziv_menija, aktivan}
    enum PoljaJela { id_jela, naziv_jela, opis_jela}
    enum PoljaSastojka { id_sastojka, naziv_sastojka, opis_sastojka}
    enum PoljaNaMeniju { id_menija, id_jela, cena}
    enum PoljaSeSastoji { id_jela, id_sastojka, kolicina }
    public partial class GlavnaForma : Form
    {
        // Proksi za komunikaciju sa serverom
        static IRestoran proxy;
        // DataSet kojim se manipulise podacima u celoj formi
        DataSet dsPodaci;
        public GlavnaForma()
        {
            InitializeComponent();
        }

        #region Pomocne funkcije
        // Operacije koje se cesto koriste sam smestio u funkcije
        private void MsgBoxError(string errorMsg)
        {
            // Prikazivanje errora korisniku sa unesenim tekstom
            MessageBox.Show(errorMsg, "Greska!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private DialogResult MsgBoxQuestion(string questionMsg)
        {
            // Prikazivanje pitanja korisniku sa unesenim tekstom, funkcija vraca odgovor
            return MessageBox.Show(questionMsg, "Pitanje", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        private void MsgBoxInfo(string infoMsg)
        {
            // Prikazivanje pitanja korisniku sa unesenim tekstom
            MessageBox.Show(infoMsg, "Obavestenje", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private int obradiComboBoxItem(object item)
        {
            // Iz combo box-a se izdvaja ID izabranog elementa, trimuje se i pretvara u int
            return int.Parse(item.ToString().Split('-')[0].Trim());
        }
        #endregion

        #region Osvezavanje kontrola
        // ComboBox-ovi se pune redovima iz tri osnovne tabele i nazivima svakog reda
        void osveziKontroleNaMeniju()
        {
            // Ubacivanje opcija u comboBox-ove za tabelu "Na meniju"
            cbIDMenijaNaMeniju.Items.Clear();
            foreach (DataRow dr in dsPodaci.Tables["meni"].Rows)
                cbIDMenijaNaMeniju.Items.Add(dr["id_menija"].ToString() + " - " + dr["naziv_menija"]);
            cbIDJelaNaMeniju.Items.Clear();
            foreach (DataRow dr in dsPodaci.Tables["jelo"].Rows)
                cbIDJelaNaMeniju.Items.Add(dr["id_jela"].ToString() + " - " + dr["naziv_jela"]);
        }

        void osveziKontroleSeSastoji()
        {
            // Ubacivanje opcija u comboBox-ove za tabelu "Se sastoji"
            cbIDSastojkaSeSastoji.Items.Clear();
            foreach (DataRow dr in dsPodaci.Tables["sastojci"].Rows)
                cbIDSastojkaSeSastoji.Items.Add(dr["id_sastojka"].ToString() + " - " + dr["naziv_sastojka"]);
            cbIDJelaSeSastoji.Items.Clear();
            foreach (DataRow dr in dsPodaci.Tables["jelo"].Rows)
                cbIDJelaSeSastoji.Items.Add(dr["id_jela"].ToString() + " - " + dr["naziv_jela"]);
        }

        void osveziKontroleSpecPretraga()
        {
            // Ubacivanje opcija u comboBox-ove za specijalne pretrage
            cbSastojakJelaPrema.Items.Clear();
            foreach (DataRow dr in dsPodaci.Tables["sastojci"].Rows)
                cbSastojakJelaPrema.Items.Add(dr["id_sastojka"].ToString() + " - " + dr["naziv_sastojka"]);

            cbMeniJelaPrema.Items.Clear();
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
            dsPodaci = proxy.PosaljiDataSet();
            // Loadovanje podataka iz DataSeta u dgw-ove sa podacima
            dgwMeni.DataSource = dsPodaci.Tables["meni"];
            dgwJelo.DataSource = dsPodaci.Tables["jelo"];
            dgwSastojak.DataSource = dsPodaci.Tables["sastojci"];
            dgwNaMeniju.DataSource = dsPodaci.Tables["naMeniju"];
            dgwSeSastoji.DataSource = dsPodaci.Tables["seSastoji"];
            // comboBox-ovi sa izborima redova se pune podacima
            osveziKontroleNaMeniju();
            osveziKontroleSeSastoji();
            osveziKontroleSpecPretraga();
            // Upisivanje imena tabela u comboBox za biranje tabele
            foreach (DataTable dt in dsPodaci.Tables)
                cbIzaberiteTabelu.Items.Add(dt.TableName);
        }

        private void GlavnaForma_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // Podaci se salju serveru radi upisa u bazu podataka
                proxy.vratiDataSet(dsPodaci);
            }
            catch (Exception exc)
            {
                MsgBoxError("Greska pri upisivanju u bazu: " + exc.Message);
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Kada se promeni izabrani tab u specijalnoj pretrazi, podaci iz dgw-a se brisu
            dgwSpecijalnaPretraga.DataSource = null;
        }

        #region Popunjavanje kontrola podacima
        // Kontrole se popunjavaju podacima iz izabranog reda
        private void dgwMeni_SelectionChanged(object sender, EventArgs e)
        {
            // Kontrole za uredjivanje tabele meni se pune podacima iz izabranog reda u dgw-u
            txtIDMenijaMeni.Text = dgwMeni.SelectedRows[0].Cells[0].Value.ToString();
            txtNazivMenijaMeni.Text = dgwMeni.SelectedRows[0].Cells[1].Value.ToString();
            chbMeniAktivanMeni.Checked = (bool)dgwMeni.SelectedRows[0].Cells[2].Value;
        }

        private void dgwJelo_SelectionChanged(object sender, EventArgs e)
        {
            // Kontrole za uredjivanje tabele jelo se pune podacima iz izabranog reda u dgw-u
            txtIDJelaJelo.Text = dgwJelo.SelectedRows[0].Cells[0].Value.ToString();
            txtNazivJelaJelo.Text = dgwJelo.SelectedRows[0].Cells[1].Value.ToString();
            txtOpisJelaJelo.Text = dgwJelo.SelectedRows[0].Cells[2].Value.ToString();
        }

        private void dgwSastojak_SelectionChanged(object sender, EventArgs e)
        {
            // Kontrole za uredjivanje tabele "se sastoji" se pune podacima iz izabranog reda u dgw-u
            txtIDSastojkaSastojak.Text = dgwSastojak.SelectedRows[0].Cells[0].Value.ToString();
            txtNazivSastojkaSastojak.Text = dgwSastojak.SelectedRows[0].Cells[1].Value.ToString();
            txtOpisSastojkaSastojak.Text = dgwSastojak.SelectedRows[0].Cells[2].Value.ToString();
        }

        private void dgwNaMeniju_SelectionChanged(object sender, EventArgs e)
        {
            // U kontrolama se prikazuju iazbrane vrednosti
            foreach(object sel in cbIDMenijaNaMeniju.Items)
                // Proverava se da li je ID iz reda comboBox-a isti kao i ID izabranog reda u dgw-u
                if(obradiComboBoxItem(sel) == (int)dgwNaMeniju.SelectedRows[0].Cells[0].Value)
                    cbIDMenijaNaMeniju.SelectedItem = sel;
            foreach (object sel in cbIDJelaNaMeniju.Items)
                if (obradiComboBoxItem(sel) == (int)dgwNaMeniju.SelectedRows[0].Cells[1].Value)
                    // -||- linija 161
                    cbIDJelaNaMeniju.SelectedItem = sel;
            txtCenaNaMeniju.Text = dgwNaMeniju.SelectedRows[0].Cells[2].Value.ToString();
        }

        private void dgwSeSastoji_SelectionChanged(object sender, EventArgs e)
        {
            if (dgwSeSastoji.SelectedRows.Count != 0)
            {
                foreach (object sel in cbIDJelaSeSastoji.Items)
                    if (obradiComboBoxItem(sel) == (int)dgwSeSastoji.SelectedRows[0].Cells[0].Value)
                        // -||- linija 161
                        cbIDJelaSeSastoji.SelectedItem = sel;
                foreach (object sel in cbIDSastojkaSeSastoji.Items)
                    if (obradiComboBoxItem(sel) == (int)dgwSeSastoji.SelectedRows[0].Cells[0].Value)
                        // -||- linija 161
                        cbIDSastojkaSeSastoji.SelectedItem = sel;
                txtKolicinaSeSastoji.Text = dgwSeSastoji.SelectedRows[0].Cells[2].Value.ToString();
            }
        }

        #endregion

        #region Kontrola unosa
        // Kontrolise se da li korisnik trenutno menja/brise red ili dodaje novi
        private void chbMZUMeni_CheckedChanged(object sender, EventArgs e)
        {
            // Naizmenicno se pale i gase dugmad za izmenu/brisanje i dodavanje
            btnDodajRedMeni.Enabled = !btnDodajRedMeni.Enabled;
            btnObrisiRedMeni.Enabled = !btnObrisiRedMeni.Enabled;
            btnIzmeniRedMeni.Enabled = !btnIzmeniRedMeni.Enabled;
        }

        private void chbMZUJelo_CheckedChanged(object sender, EventArgs e)
        {
            // -||- linija 193
            btnDodajRedJelo.Enabled = !btnDodajRedJelo.Enabled;
            btnObrisiRedJelo.Enabled = !btnObrisiRedJelo.Enabled;
            btnIzmeniRedJelo.Enabled = !btnIzmeniRedJelo.Enabled;
        }

        private void chbMZUSastojak_CheckedChanged(object sender, EventArgs e)
        {
            // -||- linija 193
            btnDodajRedSastojak.Enabled = !btnDodajRedSastojak.Enabled;
            btnObrisiRedSastojak.Enabled = !btnObrisiRedSastojak.Enabled;
            btnIzmeniRedSastojak.Enabled = !btnIzmeniRedSastojak.Enabled;
        }

        private void chbMZUNaMeniju_CheckedChanged(object sender, EventArgs e)
        {
            // -||- linija 193
            btnDodajRedNaMeniju.Enabled = !btnDodajRedNaMeniju.Enabled;
            btnObrisiRedNaMeniju.Enabled = !btnObrisiRedNaMeniju.Enabled;
            btnIzmeniRedNaMeniju.Enabled = !btnIzmeniRedNaMeniju.Enabled;
        }

        private void chbMZUSeSastoji_CheckedChanged(object sender, EventArgs e)
        {
            // -||- linija 193
            btnDodajRedSeSastoji.Enabled = !btnDodajRedSeSastoji.Enabled;
            btnObrisiRedSeSastoji.Enabled = !btnObrisiRedSeSastoji.Enabled;
            btnIzmeniRedSeSastoji.Enabled = !btnIzmeniRedSeSastoji.Enabled;
        }

        private void chbMZUSeSastoji_CheckedChanged_1(object sender, EventArgs e)
        {
            // -||- linija 193
            btnDodajRedSeSastoji.Enabled = !btnDodajRedSeSastoji.Enabled;
            btnObrisiRedSeSastoji.Enabled = !btnObrisiRedSeSastoji.Enabled;
            btnIzmeniRedSeSastoji.Enabled = !btnIzmeniRedSeSastoji.Enabled;
        }
        #endregion

        #region Dodavanje, Izmena, Brisanje
        // Funkcije za korisnicku obradu podataka u dgw-ovima
        private void btnObrisiRedMeni_Click(object sender, EventArgs e)
        {
            try
            {
                // Potvrda brisanja
                if (MsgBoxQuestion("Da li stvarno zelite da izbrisete taj red?") == DialogResult.Yes)
                {
                    // Brise se izabrani red
                    dgwMeni.Rows.Remove(dgwMeni.SelectedRows[0]);
                    // dgw se osvezava
                    dgwMeni.DataSource = dsPodaci.Tables["meni"];
                    MsgBoxInfo("Red uspesno obrisan!");
                    // DataSet upisuje promene u dgw-u i kontrole se osvezavaju
                    dsPodaci.AcceptChanges();
                    osveziKontroleNaMeniju();
                    osveziKontroleSpecPretraga();
                }
                else
                {
                    // Korisnik otkazao brisanje
                    MsgBoxInfo("Red nije izbrisan");
                }
            }
            catch (Exception exc)
            {
                // U slucaju moguce greske pri brisanju reda, ona se prikazuje
                MsgBoxError("Tekst greske: " + exc.Message);
            }
        }

        private void btnDodajRedMeni_Click(object sender, EventArgs e)
        {
            try
            {
                // Podaci se preuzimaju iz kontrola
                string naziv = txtNazivMenijaMeni.Text;
                bool aktivan = chbMeniAktivanMeni.Checked;
                // Podaci se salju serveru na insertovanje i server vraca ID koji je generisala baza podataka
                int id = proxy.InsertIntoMeni(naziv, aktivan);
                // Pravi se novi red koji se ubacuje u DataSet
                DataRow dr = dsPodaci.Tables["meni"].NewRow();
                dr["id_menija"] = id;
                dr["naziv_menija"] = naziv;
                dr["aktivan"] = aktivan;
                dsPodaci.Tables["meni"].Rows.Add(dr);
                // dgw se osvezava
                dgwMeni.DataSource = dsPodaci.Tables["meni"];
                MsgBoxInfo("Red je uspesno unet!");
                // Kontrole se osvezavaju
                osveziKontroleNaMeniju();
                osveziKontroleSpecPretraga();
            }
            catch (Exception exc)
            {
                // U slucaju moguce greske pri dodavanju reda, ona se prikazuje
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
                    // Preuzimaju se podaci iz kontrola
                    string naziv = txtNazivMenijaMeni.Text;
                    bool aktivan = chbMeniAktivanMeni.Checked;
                    // Trazi se red sa ID-jem podatka koji se menja i u njega se upisuju novi podaci
                    foreach (DataRow dr in dsPodaci.Tables["meni"].Rows)
                    {
                        if (dr["id_menija"].ToString() == dgwMeni.SelectedRows[0].Cells[0].Value.ToString())
                        {
                            dr["naziv_menija"] = naziv;
                            dr["aktivan"] = aktivan;
                        }
                    }
                    // dgw se osvezava
                    dgwMeni.DataSource = dsPodaci.Tables["meni"];
                    // Kontrole se osvezavaju
                    osveziKontroleNaMeniju();
                    osveziKontroleSpecPretraga();
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
            // -||- linija 242
            if (MsgBoxQuestion("Da li stvarno zelite da izbrisete taj red?") == DialogResult.Yes)
            {
                dgwJelo.Rows.Remove(dgwJelo.SelectedRows[0]);
                dgwJelo.DataSource = dsPodaci.Tables["jelo"];
                dsPodaci.AcceptChanges();
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
            // -||- linija 279
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
                // -||- linija 308
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
                    osveziKontroleNaMeniju();
                    osveziKontroleSeSastoji();
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
            // -||- linija 242
            if (MsgBoxQuestion("Da li stvarno zelite da izbrisete taj red?") == DialogResult.Yes)
            {
                dgwSastojak.Rows.Remove(dgwSastojak.SelectedRows[0]);
                dgwSastojak.DataSource = dsPodaci.Tables["sastojci"];
                dsPodaci.AcceptChanges();
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
                // -||- linija 279
                string naziv = txtNazivSastojkaSastojak.Text;
                string opis = txtOpisSastojkaSastojak.Text;
                int id = proxy.InsertIntoSastojak(naziv, opis);
                DataRow dr = dsPodaci.Tables["sastojci"].NewRow();
                dr["id_sastojka"] = id;
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
                // -||- linija 308
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
            // -||- linija 242
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
                // Podaci se uzimaju iz comboBoxova
                int idjela = obradiComboBoxItem(cbIDJelaNaMeniju.SelectedItem);
                int idmenija = obradiComboBoxItem(cbIDMenijaNaMeniju.SelectedItem);
                // Odredjuje da li je korisnik izabrao dva ID-ja koja vec postoje u nekom redu
                bool validno = true;
                foreach (DataRow dr in dsPodaci.Tables["naMeniju"].Rows)
                    // Da li su oba ID-ja ista kao izabrani
                    if ((int)dr["id_jela"] == idjela && (int)dr["id_menija"] == idmenija)
                        validno = false;
                // Ako nije, red se unosi
                if (validno)
                {
                    // Preuzima se i cena za unos, posto je utvrdjeno da je primarni kljuc validan
                    int cena = int.Parse(txtCenaNaMeniju.Text);
                    // Pravi se novi red i unosi se u DataSet
                    DataRow dr = dsPodaci.Tables["naMeniju"].NewRow();
                    dr["id_menija"] = idmenija;
                    dr["id_jela"] = idjela;
                    dr["cena"] = cena;
                    dsPodaci.Tables["naMeniju"].Rows.Add(dr);
                    // Osvezava se dgw
                    dgwNaMeniju.DataSource = dsPodaci.Tables["naMeniju"];
                    MsgBoxInfo("Red uspesno dodat!");
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
                    // Preuzimanje podataka iz comboBoxova i polja za cenu
                    int idjela = obradiComboBoxItem(cbIDJelaNaMeniju.SelectedItem);
                    int idmenija = obradiComboBoxItem(cbIDMenijaNaMeniju.SelectedItem);
                    int cena = int.Parse(txtCenaNaMeniju.Text);
                    // Proverava se da li korisnik nije promenio primarne kljuceve
                    if (idmenija != int.Parse(dgwNaMeniju.SelectedRows[0].Cells[0].Value.ToString()) || idjela != int.Parse(dgwNaMeniju.SelectedRows[0].Cells[1].Value.ToString()))
                    {
                        // Ako jeste, mora se proveriti da li postoji isti primarni kljuc
                        foreach (DataRow dr in dsPodaci.Tables["naMeniju"].Rows)
                            if ((int)dr["id_jela"] == idjela && (int)dr["id_menija"] == idmenija)
                            {
                                MsgBoxError("Vec postoji red sa tim identitetom!");
                                return;
                            }
                    }
                    // Ako nije promenio ili prodje proveru, red se menja
                    foreach (DataRow dr in dsPodaci.Tables["naMeniju"].Rows)
                    {
                        if (dr["id_menija"].ToString() == dgwNaMeniju.SelectedRows[0].Cells[0].Value.ToString() && dr["id_jela"].ToString() == dgwNaMeniju.SelectedRows[0].Cells[1].Value.ToString())
                        {
                            dr["id_menija"] = idmenija;
                            dr["id_jela"] = idjela;
                            dr["cena"] = cena;
                        }
                    }
                    // Osvezava se dgw
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
            // -||- linija 242
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
                // -||- linija 502
                int idsastojka = obradiComboBoxItem(cbIDSastojkaSeSastoji.SelectedItem);
                int idjela = obradiComboBoxItem(cbIDJelaSeSastoji.SelectedItem);
                int kolicina = int.Parse(txtKolicinaSeSastoji.Text);
                bool validno = true;
                foreach (DataRow dr in dsPodaci.Tables["seSastoji"].Rows)
                    if ((int)dr["id_jela"] == idjela && (int)dr["id_sastojka"] == idsastojka)
                        validno = false;
                if (validno)
                {
                    DataRow dr = dsPodaci.Tables["seSastoji"].NewRow();
                    dr["id_sastojka"] = idsastojka;
                    dr["id_jela"] = idjela;
                    dr["kolicina"] = kolicina;
                    dsPodaci.Tables["seSastoji"].Rows.Add(dr);
                    dgwSeSastoji.DataSource = dsPodaci.Tables["seSastoji"];
                    MsgBoxInfo("Red je uspesno dodat!");
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
                // -||- linija 539
                if (MsgBoxQuestion("Da li zelite da izmenite ovaj red?") == DialogResult.Yes)
                {
                    int idsastojka = obradiComboBoxItem(cbIDSastojkaSeSastoji.SelectedItem);
                    int idjela = obradiComboBoxItem(cbIDJelaSeSastoji.SelectedItem);
                    int kolicina = int.Parse(txtKolicinaSeSastoji.Text);
                    if (idjela != (int)dgwSeSastoji.SelectedRows[0].Cells[0].Value || idsastojka != (int)dgwSeSastoji.SelectedRows[0].Cells[1].Value)
                    {
                        foreach (DataRow dr in dsPodaci.Tables["seSastoji"].Rows)
                            if ((int)dr["id_jela"] == idjela && (int)dr["id_sastojka"] == idsastojka)
                            {
                                MsgBoxError("Vec postoji objekat sa tim identitetom! Unesite drugacije ID-jeve!");
                                return;
                            }
                    }
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
                // Serveru se prosledjuje vrednost korisnicke kontrole, a on vraca rezultate upita u listi jela
                List<Jelo> pretraga = proxy.jelaNaMenijimaKojiSuAktivniSaCenom(int.Parse(txtCenaSpecPretraga.Text));
                // Rezultati se prikazuju u dgw-u
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
                // -||-linija 673
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
                // -||-linija 673
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
                // -||-linija 673
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
            // Prema vrednosti selekcije tabele, drugi comboBox se ispunjava poljima te tabele
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
            // Prvo se proverava da li je ijedan element izabran kao parametar, jer je to neophodno za pretragu
            if (cbIzaberiteParametar.SelectedItem != null)
            {
                // Podaci se uzimaju iz kontrola
                string tabela = cbIzaberiteTabelu.SelectedItem.ToString();
                string polje = cbIzaberiteParametar.SelectedItem.ToString();
                // Pravi se klon tabele koju je korisnik izabrao da pretrazuje
                DataTable podaci = dsPodaci.Tables[tabela].Clone();
                // Tabela se prazni radi unosa podataka
                podaci.Clear();
                // Proverava se da li se pretrazuje "aktivan" polje tabele meni
                // Ono je posebno jer ima poseban nacin za unos parametra pretrage (checkBox)
                // Stoga ako je checkBox aktivan pretraga se vrsi na drugaciji nacin
                if (chbAktivanPretraga.Enabled)
                {
                    foreach (DataRow dr in dsPodaci.Tables[tabela].Rows)
                    {
                        if ((bool)dr["aktivan"] == chbAktivanPretraga.Checked)
                            podaci.ImportRow(dr);
                    }
                }
                else
                {
                    // Ako nije pretrazuje se sa vrednoscu iz textBox-a
                    string vrednost = txtVrednostParametra.Text;
                    if (polje == "naziv_menija" || polje == "naziv_jela" || polje == "opis_jela" || polje == "naziv_sastojka" || polje == "opis_sastojka")
                    {
                        // Ako je polje koje je korisnik izabrao jedno od polja sa tekstualnom vrednoscu
                        // Pretraga se vrsi pomocu funkcije Contains() koja proverava da li string sadrzi drugi string
                        foreach (DataRow dr in dsPodaci.Tables[tabela].Rows)
                        {
                            if (dr[polje].ToString().Contains(vrednost.Trim()))
                                podaci.ImportRow(dr);
                        }
                    }
                    else
                    {
                        // Ako nije ni to, onda je polje koje se pretrazuje integer i pretrazuje se preko pretvaranja u string   
                        foreach (DataRow dr in dsPodaci.Tables[tabela].Rows)
                        {
                            if (dr[polje].ToString() == vrednost.Trim())
                                podaci.ImportRow(dr);
                        }
                    }
                }
                // Podaci se prikazuju u dgw-u
                dgwPretragaPoParametrima.DataSource = podaci;
            }
            else
                MsgBoxError("Prvo morate izabrati parametar da biste pretrazivali po njemu!");
        }
        #endregion

        private void cbIzaberiteParametar_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Svaki put kada korisnik izabere novi parametar, proverava se da li je to parametar "atktivan"
            // Ako jeste, checkBox postaje aktivan a textBox neaktivan i obrnuto
            if (cbIzaberiteParametar.SelectedItem.ToString() == "aktivan")
            {
                txtVrednostParametra.Enabled = false;
                chbAktivanPretraga.Enabled = true;
            }
            else
            {
                txtVrednostParametra.Enabled = true;
                chbAktivanPretraga.Enabled = false;
            }
        }
    }
}
