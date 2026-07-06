using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace efdb_arac_kiralama_proje
{
    public partial class Menu : Form
    {
        public Menu()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Arac arac = new Arac();
            arac.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Marka marka = new Marka();
            marka.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Musteri musteri= new Musteri();
            musteri.Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            KiralamaIslemleri kiralamaIslemleri = new KiralamaIslemleri();
            kiralamaIslemleri.Show();
            this.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Rapor rapor = new Rapor();
            rapor.Show();
            this.Hide();
        }
    }
}
