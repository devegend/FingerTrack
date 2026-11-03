using Supabase.Gotrue.Exceptions;
using Supabase.Postgrest.Models;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace fingertrack
{
    public partial class Form1 : Form
    {
        private const string SupabaseUrl =
            "https://fndvmiltkpzaazeuqikq.supabase.co";

        private const string SupabasePublishableKey =
            "sb_publishable_E7hpBRmBpjhpuN69YQIFqg_0p2Hyxtc";

        private static Supabase.Client? _supabaseClient;
        private int _failedLoginAttempts = 0;
        private const int MaxFailedLoginAttempts = 3;

        public Form1()
        {
            InitializeComponent();

            // إظهار كلمة السر كنقاط
            textBox4.UseSystemPasswordChar = true;
        }

        private static async Task<Supabase.Client> GetSupabaseClientAsync()
        {
            if (_supabaseClient != null)
                return _supabaseClient;

            var client = new Supabase.Client(
                SupabaseUrl,
                SupabasePublishableKey);

            await client.InitializeAsync();
            _supabaseClient = client;

            return _supabaseClient;
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            string email = textBox2.Text.Trim();
            string password = textBox4.Text;

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("دخل اسم المستخدم وكلمة السر.");
                return;
            }

            button2.Enabled = false;

            try
            {
                var supabase = await GetSupabaseClientAsync();

                Supabase.Gotrue.Session? session;

                try
                {
                    session = await supabase.Auth.SignIn(email, password);
                }
                catch (GotrueException)
                {
                    _failedLoginAttempts++;

                    if (_failedLoginAttempts >= MaxFailedLoginAttempts)
                    {
                        MessageBox.Show(
                            "اسم المستخدم أو كلمة السر غير صحيحين.\n" +
                            "فشلت 3 محاولات، غادي يتسد التطبيق.");

                        Application.Exit();
                    }
                    else
                    {
                        int remaining = MaxFailedLoginAttempts - _failedLoginAttempts;

                        MessageBox.Show(
                            $"اسم المستخدم أو كلمة السر غير صحيحين.\n" +
                            $"باقي ليك {remaining} محاولات.");
                    }

                    return;
                }

                string? userId = session?.User?.Id;

                if (string.IsNullOrWhiteSpace(userId))
                {
                    MessageBox.Show("تعذر التحقق من الحساب.");
                    return;
                }

                _failedLoginAttempts = 0;

                var profile = await supabase
                    .From<UserProfile>()
                    .Where(user => user.Id == userId)
                    .Single();

                if (profile == null)
                {
                    MessageBox.Show("ما لقيناش الدور ديال هاد الحساب.");
                    return;
                }

                Form? page = profile.Role.Trim().ToLowerInvariant() switch
                {
                    "directeur" => new DirecteurForm(),
                    "secretaire" => new SecretaireForm(),
                    _ => null
                };

                if (page == null)
                {
                    MessageBox.Show("الدور ديال الحساب غير معروف.");
                    return;
                }

                Hide();
                page.FormClosed += (_, _) => Close();
                page.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"تعذر الاتصال بـ Supabase:\n{ex.Message}");
            }
            finally
            {
                button2.Enabled = true;
            }
        }

        private void label1_Click(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void panel2_Paint(object sender, PaintEventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void pictureBox2_Click(object sender, EventArgs e) { }
        private void panel3_Paint(object sender, PaintEventArgs e) { }
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) { }
        private void Form1_Load(object sender, EventArgs e) { }
        private void button1_Click(object sender, EventArgs e) { }
        private void pictureBox1_Click(object sender, EventArgs e) { }
        private void textBox1_TextChanged_1(object sender, EventArgs e) { }

        private void button3_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }

    [Table("profiles")]
    public class UserProfile : BaseModel
    {
        [PrimaryKey("id")]
        public string Id { get; set; } = string.Empty;

        [Column("role")]
        public string Role { get; set; } = string.Empty;
    }

    // صفحات مؤقتة؛ تقدر تعوض محتواها بواجهات المدير والسكرتيرة ديالك
    public class DirecteurForm : Form
    {
        public DirecteurForm()
        {
            Text = "Espace directeur";
            StartPosition = FormStartPosition.CenterScreen;
            Width = 900;
            Height = 600;

            Controls.Add(new Label
            {
                Text = "Bienvenue dans l'espace directeur",
                AutoSize = true,
                Location = new Point(30, 30)
            });
        }
    }

    public class SecretaireForm : Form
    {
        public SecretaireForm()
        {
            Text = "Espace secrétaire";
            StartPosition = FormStartPosition.CenterScreen;
            Width = 900;
            Height = 600;

            Controls.Add(new Label
            {
                Text = "Bienvenue dans l'espace secrétaire",
                AutoSize = true,
                Location = new Point(30, 30)
            });
        }
    }
}