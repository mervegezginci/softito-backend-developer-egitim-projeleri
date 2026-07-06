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
    public partial class Marka : Form
    {
        public Marka()
        {
            InitializeComponent();
        }

        softAracKiralamaEntities1 conn = new softAracKiralamaEntities1();

        // geri butonu
        private void button6_Click(object sender, EventArgs e)
        {
            Menu ana = new Menu();
            ana.Show();
            this.Hide();
        }

        // listele
        public void Listele()
        {
            dataGridView1.DataSource = conn.Markalars
                .Select(x => new
                {
                    x.MarkaNo,
                    x.MarkaAdi
                })
                .ToList();
        }

        // listele butonu
        private void button1_Click(object sender, EventArgs e)
        {
            Listele();
        }

        // kaydet butonu
        private void button2_Click(object sender, EventArgs e)
        {
            Markalar marka = new Markalar();

            marka.MarkaAdi = textBox1.Text;

            conn.Markalars.Add(marka);
            conn.SaveChanges();

            Listele();
            textBox1.Clear();
        }

        // yenile butonu
        private void button3_Click(object sender, EventArgs e)
        {
            int markaNo = Convert.ToInt32(textBox1.Tag);

            var guncelle = conn.Markalars.FirstOrDefault(x => x.MarkaNo == markaNo);

            if (guncelle != null)
            {
                guncelle.MarkaAdi = textBox1.Text;

                conn.SaveChanges();
                Listele();
            }
        }

        // sil butonu
        private void button4_Click(object sender, EventArgs e)
        {
            int markaNo = Convert.ToInt32(textBox1.Tag);

            var sil = conn.Markalars.FirstOrDefault(x => x.MarkaNo == markaNo);

            if (sil != null)
            {
                conn.Markalars.Remove(sil);
                conn.SaveChanges();
                Listele();
            }
        }

        // arama butonu
        private void button5_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = conn.Markalars
                .Where(x => x.MarkaAdi.Contains(textBox5.Text))
                .Select(x => new
                {
                    x.MarkaNo,
                    x.MarkaAdi
                })
                .ToList();
        }

        private void Marka_Load(object sender, EventArgs e)
        {
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Listele();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow row = dataGridView1.CurrentRow;

            textBox1.Tag = row.Cells["MarkaNo"].Value.ToString();
            textBox1.Text = row.Cells["MarkaAdi"].Value.ToString();

        }
    }
}
