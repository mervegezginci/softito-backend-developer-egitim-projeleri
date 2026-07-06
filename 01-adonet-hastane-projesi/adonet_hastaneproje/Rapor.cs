using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace adonet_hastaneproje
{
    public partial class Rapor : Form
    {
        public Rapor()
        {
            InitializeComponent();
        }

        SqlConnection conn = new SqlConnection("Data Source=Merve;Database=softHastane;Integrated Security=true");


        public void Calistir(string procName)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = procName;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            dataGridView1.DataSource = dt;
            dataGridView1.Visible = true;
        }

        // doktorlar menü butonu
        private void doktorlarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Doktorlar frm = new Doktorlar();
            frm.Show();
            this.Hide();
        }

        // hastalar menü butonu
        private void hastalarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Hastalar frm = new Hastalar();
            frm.Show();
            this.Close();
        }

        // poliklinik menü butonu
        private void polikliniklerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Poliklinikler frm = new Poliklinikler();
            frm.Show();
            this.Close();
        }

        // kayıtlar menü butonu
        private void kayıtlarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Kayitlar frm = new Kayitlar();
            frm.Show();
            this.Close();
        }

        // raporlar menü butonu
        private void raporlarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Rapor frm = new Rapor();
            frm.Show();
            this.Close();
        }

        private void Rapor_Load(object sender, EventArgs e)
        {
            dataGridView1.Visible = false;
        }

        // 1 - Kilosu yüksek hastalar
        private void button1_Click(object sender, EventArgs e)
        {
            Calistir("sp_KilosuYuksekHastalar");
        }

        // 2 - 20+ yaş ortalama boy
        private void button2_Click(object sender, EventArgs e)
        {
            Calistir("sp_20UstuBoyOrtalamasi");
        }

        // 3 - Kilo sırala
        private void button3_Click(object sender, EventArgs e)
        {
            Calistir("sp_KiloSirala");
        }

        // 4 - Doktor maaş sıralama
        private void button4_Click(object sender, EventArgs e)
        {
            Calistir("sp_DoktorMaasSirala");
        }

        // 5 - Ortalama kilo
        private void button5_Click(object sender, EventArgs e)
        {
            Calistir("sp_OrtalamaKilo");
        }

        // 6 - En kilolu hasta
        private void button6_Click(object sender, EventArgs e)
        {
            Calistir("sp_EnKiloluHasta");
        }

        // 7 - 18 altı hastalar
        private void button7_Click(object sender, EventArgs e)
        {
            Calistir("sp_18AltiHastalar");
        }

        // 8 - 70+ kilo hastalar
        private void button8_Click(object sender, EventArgs e)
        {
            Calistir("sp_70UstuHastalar");
        }

        // 9 - Ortalama maaş
        private void button9_Click(object sender, EventArgs e)
        {
            Calistir("sp_OrtalamaMaas");
        }

        // 10 - İsmi A ile Başlayan Hastalar
        private void button10_Click(object sender, EventArgs e)
        {
            Calistir("sp_A_ileBaslayanHastalar");
        }

    }
}
