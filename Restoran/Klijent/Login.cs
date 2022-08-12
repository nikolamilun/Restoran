using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Klijent
{
    // Forma za login
    public partial class Login : Form
    {
        // Korisnicko ime i lozinka cisto za primer
        string korIme = "admin";
        string lozinka = "12345";
        // Promenljiva pokazuje da li se korisnik ulogovao
        public bool logged = false;
        public Login()
        {
            InitializeComponent();
        }

        private void btnPrijaviSe_Click(object sender, EventArgs e)
        {
            // Provera da li su uneseni tacni podaci
            if (txtKorIme.Text.Trim() == korIme)
            {
                if (txtLozinka.Text.Trim() == lozinka)
                {
                    logged = true;
                    Close();  
                }
                else
                    GlavnaForma.MsgBoxError("Netacna lozinka!");
                return;

            }
            GlavnaForma.MsgBoxError("Netacni ulazni podaci!");
        }
    }
}
