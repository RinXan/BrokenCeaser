using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Numerics;

namespace BrokenCeaser
{
    public partial class Form1 : Form
    {
        private string text;

        static readonly string cyrillic = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя0123456789";
        static readonly string latin = "abcdefghijklmnopqrstuvwxyz0123456789";

        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string name = openFileDialog1.FileName;
                textBox1.Clear();
                textBox1.Text = File.ReadAllText(name);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            text = textBox1.Text;
            textBox1.Clear();
            if (checkBox1.Checked)
            {
                textBox1.Text = Broke_Ceaser(text, cyrillic, true);

            }
            else
            {
                textBox1.Text = Broke_Ceaser(text, latin);
            }
        }

        // Взлом шифра Цезаря при помощи частотного анализа
        private string Broke_Ceaser(string text, string alphabet, bool russian = false)
        {
            try
            {
                var letter_appears_in_text = new Dictionary<char, int>();
                var letter_frequency_in_text = new Dictionary<char, double>();

                int key;
                int total_letters_count_in_text = 0;
                int key_letter_index;
                int max_frequency_letter_index;
                double max_frequency = 0;
                char max_frequency_letter = 'a';

                foreach (char s in text)
                {
                    if (!cyrillic.Contains(s) && !latin.Contains(s))
                    {
                        continue;
                    }
                    if (letter_appears_in_text.ContainsKey(s))
                    {
                        letter_appears_in_text[s]++;
                    }
                    else
                    {
                        letter_appears_in_text.Add(s, 1);
                    }
                    total_letters_count_in_text++;
                }

                foreach (var letter in letter_appears_in_text)
                {
                    double frequency = Math.Round((double)letter.Value / (double)total_letters_count_in_text, 2);
                    letter_frequency_in_text.Add(letter.Key, frequency);
                    if (frequency > max_frequency)
                    {
                        max_frequency = frequency;
                        max_frequency_letter = letter.Key;
                    }
                }

                if (russian)
                {
                    key_letter_index = alphabet.IndexOf('о');
                }
                else
                {
                    key_letter_index = alphabet.IndexOf('e');
                }

                max_frequency_letter_index = alphabet.IndexOf(max_frequency_letter);
                key = Math.Abs(key_letter_index - max_frequency_letter_index);
                text = Decrypt(alphabet, text, $"{key}");
                textBox2.Text = key.ToString();

                return text;
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
                textBox2.Clear();
                return text;
            }
        }

        // Функция дешифратор из шифра Цезаря
        public string Decrypt(string al, string text, string step)
        {
            try
            {
                Is_text_correct(text, al);
                StringBuilder code = new StringBuilder();
                BigInteger key = BigInteger.Parse(step);

                for (int i = 0; i < text.Length; i++)
                {
                    for (int j = 0; j < al.Length; j++)
                    {
                        if (Char.ToLower(text[i]) == al[j])
                        {
                            int ind = (int)((j - key + al.Length) % al.Length);
                            if (ind < 0)
                            {
                                code.Append(al[al.Length + ind]);
                            }
                            else
                            {
                                code.Append(al[ind]);
                            }
                            break;
                        }
                        else if (!cyrillic.Contains(text[i]) && !latin.Contains(text[i]))
                        {
                            code.Append(text[i]);
                            break;
                        }
                    }
                }
                return code.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Проверка введенного текста 
        public void Is_text_correct(string text, string al)
        {
            if (text.Length == 0)
            {
                throw new Exception("Введите текст для шифрования.");
            }
            // Пропускаем все символы которые отсутствуют в алфавитах
            for (int i = 0; i < text.Length; i++)
            {
                if (!cyrillic.Contains(text[i]) && !latin.Contains(text[i]))
                {
                    continue;
                }
                if (!al.Contains(text[i]))
                {
                    throw new Exception($"Текст содержит не соответсвтующие алфавиту символы.");
                }
            }
        }
    }
}
