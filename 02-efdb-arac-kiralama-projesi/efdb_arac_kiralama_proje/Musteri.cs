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
    public partial class Musteri : Form
    {
        public Musteri()
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

        public void Listele()
        {
            dataGridView1.DataSource = conn.Musterilers
                .Select(x => new
                {
                    x.MusteriNo,
                    x.AdSoyad,
                    x.Telefon,
                    x.Tc,
                    x.Email
                })
                .ToList();
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dataGridView1.CurrentRow;

            textBox1.Tag = row.Cells["MusteriNo"].Value.ToString();

            textBox1.Text = row.Cells["AdSoyad"].Value.ToString();
            textBox2.Text = row.Cells["Telefon"].Value.ToString();
            textBox3.Text = row.Cells["TC"].Value.ToString();
            textBox4.Text = row.Cells["Email"].Value.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Listele();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Musteriler musteri = new Musteriler();

            musteri.AdSoyad = textBox1.Text;
            musteri.Telefon = textBox2.Text;
            musteri.Tc = textBox3.Text;
            musteri.Email = textBox4.Text;

            conn.Musterilers.Add(musteri);
            conn.SaveChanges();

            Listele();

            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int musteriNo = Convert.ToInt32(textBox1.Tag);

            var guncelle = conn.Musterilers
                .FirstOrDefault(x => x.MusteriNo == musteriNo);

            if (guncelle != null)
            {
                guncelle.AdSoyad = textBox1.Text;
                guncelle.Telefon = textBox2.Text;
                guncelle.Tc = textBox3.Text;
                guncelle.Email = textBox4.Text;

                conn.SaveChanges();
                Listele();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int musteriNo = Convert.ToInt32(textBox1.Tag);

            var sil = conn.Musterilers
                .FirstOrDefault(x => x.MusteriNo == musteriNo);

            if (sil != null)
            {
                conn.Musterilers.Remove(sil);
                conn.SaveChanges();

                Listele();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = conn.Musterilers
                .Where(x => x.AdSoyad.Contains(textBox5.Text))
                .Select(x => new
                {
                    x.MusteriNo,
                    x.AdSoyad,
                    x.Telefon,
                    x.Tc,
                    x.Email
                })
                .ToList();
        }

        private void Musteri_Load(object sender, EventArgs e)
        {

        }
    }
}
