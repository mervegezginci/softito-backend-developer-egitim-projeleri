using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace efdb_arac_kiralama_proje
{
    public partial class Arac : Form
    {
        public Arac()
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

        // listele metotu
        public void Listele()
        {
            dataGridView1.DataSource = conn.Araclars
                .Select(x => new
                {
                    x.AracNo,
                    x.Model,
                    x.Yil,
                    x.Renk,
                    x.Plaka,
                    x.GunlukUcret,
                    x.Durum,
                    x.MarkaNo,
                    x.ResimYolu
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
            Araclar araclar = new Araclar();

            araclar.Model = textBox1.Text;
            araclar.Yil = Convert.ToInt32(textBox2.Text);
            araclar.Renk = textBox3.Text;
            araclar.Plaka = textBox4.Text;
            araclar.GunlukUcret = Convert.ToDecimal(textBox6.Text);
            araclar.Durum = textBox7.Text;

            // marka
            araclar.MarkaNo = Convert.ToInt32(comboBox1.SelectedValue);

            // resim yolu
            araclar.ResimYolu = pictureBox1.Tag?.ToString();

            conn.Araclars.Add(araclar);
            conn.SaveChanges();
            Listele();
        }

        // yenile butonu
        private void button3_Click(object sender, EventArgs e)
        {
            int aracNo = Convert.ToInt32(textBox1.Tag);

            var guncelle = conn.Araclars.FirstOrDefault(x => x.AracNo == aracNo);

            if (guncelle != null)
            {
                guncelle.Model = textBox1.Text;
                guncelle.Yil = Convert.ToInt32(textBox2.Text);
                guncelle.Renk = textBox3.Text;
                guncelle.Plaka = textBox4.Text;
                guncelle.GunlukUcret = Convert.ToDecimal(textBox6.Text);
                guncelle.Durum = textBox7.Text;
                guncelle.MarkaNo = Convert.ToInt32(comboBox1.SelectedValue);
                guncelle.ResimYolu = pictureBox1.Tag?.ToString();

                conn.SaveChanges();
                Listele();
            }
        }

        // sil butonu
        private void button4_Click(object sender, EventArgs e)
        {
            int AracNo = Convert.ToInt32(textBox1.Tag);

            var delete = conn.Araclars.FirstOrDefault(x => x.AracNo == AracNo);

            if (delete != null)
            {
                conn.Araclars.Remove(delete);
                conn.SaveChanges();
                Listele();
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                textBox1.Tag = row.Cells["AracNo"].Value.ToString();
                textBox1.Text = row.Cells["Model"].Value.ToString();
                textBox2.Text = row.Cells["Yil"].Value.ToString();
                textBox3.Text = row.Cells["Renk"].Value.ToString();
                textBox4.Text = row.Cells["Plaka"].Value.ToString();
                textBox6.Text = row.Cells["GunlukUcret"].Value.ToString();
                textBox7.Text = row.Cells["Durum"].Value.ToString();

                if (row.Cells["MarkaNo"].Value != null)
                {
                    comboBox1.SelectedValue = Convert.ToInt32(row.Cells["MarkaNo"].Value);
                }

                // resim yükleme
                string path = row.Cells["ResimYolu"].Value?.ToString();

                if (!string.IsNullOrEmpty(path) && System.IO.File.Exists(path))
                {
                    pictureBox1.Image = Image.FromFile(path);
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                }
                else
                {
                    pictureBox1.Image = null;
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string kelime = textBox5.Text.ToLower();

            dataGridView1.DataSource = conn.Araclars
                .Where(x => x.Model != null && x.Model.ToLower().Contains(kelime))
                .Select(x => new
                {
                    x.AracNo,
                    x.Model,
                    x.Yil,
                    x.Renk,
                    x.Plaka,
                    x.GunlukUcret,
                    x.Durum,
                    x.MarkaNo,
                    x.ResimYolu
                })
                .ToList();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "Resim Dosyaları|*.jpg;*.jpeg;*.png";

            if (file.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(file.FileName);

                // yolu sakla
                pictureBox1.Tag = file.FileName;
            }
        }

        private void Arac_Load(object sender, EventArgs e)
        {
            comboBox1.DataSource = conn.Markalars.ToList();
            comboBox1.DisplayMember = "MarkaAdi";
            comboBox1.ValueMember = "MarkaNo";

            comboBox1.SelectedIndex = -1;
            comboBox1.Text = "Marka seçiniz";

        }
    }
}
