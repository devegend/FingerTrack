using System;
using System.Drawing;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace fingertrack
{
    public partial class Form1 : Form
    {
        private const string SupabaseUrl =
            "https://fndvmiltkpzaazeuqikq.supabase.co";

        private const string SupabasePublishableKey =
            "sb_publishable_E7hpBRmBpjhpuN69YQIFqg_0p2Hyxtc";

        private static readonly HttpClient Http = new();

        private static Supabase.Client? _supabaseClient;

        private int _failedLoginAttempts;
        private const int MaxFailedLoginAttempts = 3;
        private Image? _eyeImage;
        private Image? _eyeOffImage;
        private bool _passwordVisible = false;
        public Form1()
        {
            InitializeComponent();

            textBox1.Multiline = false;
            textBox1.UseSystemPasswordChar = true;

            string imgFolder = Path.Combine(AppContext.BaseDirectory, "img");

            _eyeImage = Image.FromFile(Path.Combine(imgFolder, "eye.png"));
            _eyeOffImage = Image.FromFile(Path.Combine(imgFolder, "eye_off.png"));

            pictureBox5.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox5.Image = _eyeOffImage;
            pictureBox5.Cursor = Cursors.Hand;
            pictureBox5.Click += pictureBox5_Click;
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

            return client;
        }

        private static async Task<LoginResponse?> LoginWithPseudoAsync(
            string pseudo,
            string password)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"{SupabaseUrl}/functions/v1/login-by-pseudo");

            request.Headers.Add("apikey", SupabasePublishableKey);

            request.Content = JsonContent.Create(new
            {
                pseudo,
                password
            });

            using HttpResponseMessage response = await Http.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return null;

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    $"Le service de connexion a répondu avec le code " +
                    $"{(int)response.StatusCode}.");
            }

            string responseBody = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<LoginResponse>(
                responseBody,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            // textBox3 = pseudo ; textBox1 = mot de passe
            string pseudo = textBox3.Text.Trim();
            string password = textBox1.Text;

            if (string.IsNullOrWhiteSpace(pseudo) ||
                string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show(
                    "Veuillez saisir votre pseudo et votre mot de passe.",
                    "Connexion",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            button2.Enabled = false;

            try
            {
                LoginResponse? login =
                    await LoginWithPseudoAsync(pseudo, password);

                if (login == null ||
                    string.IsNullOrWhiteSpace(login.AccessToken) ||
                    string.IsNullOrWhiteSpace(login.RefreshToken))
                {
                    _failedLoginAttempts++;

                    if (_failedLoginAttempts >= MaxFailedLoginAttempts)
                    {
                        MessageBox.Show(
                            "Le pseudo ou le mot de passe est incorrect.\n" +
                            "Vous avez dépassé 3 tentatives. L'application va se fermer.",
                            "Échec de connexion",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

                        Application.Exit();
                        return;
                    }

                    int remaining =
                        MaxFailedLoginAttempts - _failedLoginAttempts;

                    MessageBox.Show(
                        "Le pseudo ou le mot de passe est incorrect.\n" +
                        $"Il vous reste {remaining} tentative(s).",
                        "Échec de connexion",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                var supabase = await GetSupabaseClientAsync();

                await supabase.Auth.SetSession(
                    login.AccessToken,
                    login.RefreshToken);

                string? userId = supabase.Auth.CurrentSession?.User?.Id;

                if (string.IsNullOrWhiteSpace(userId))
                {
                    MessageBox.Show(
                        "La connexion a échoué. Veuillez réessayer.",
                        "Erreur de connexion",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                ConnexionProfile profile;

                try
                {
                    profile = await supabase
                        .From<ConnexionProfile>()
                        .Where(user => user.AuthUserId == userId)
                        .Single();
                }
                catch
                {
                    MessageBox.Show(
                        "Votre compte est connecté, mais son profil est introuvable " +
                        "dans la table « connexion ».",
                        "Profil introuvable",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                _failedLoginAttempts = 0;

                string poste = profile.Poste.Trim().ToLowerInvariant();

                Form? page = poste switch
                {
                    "directeur" => new directeur.DirecteurDashboard("Loudi Issam"),
                    "secretaire" or "secrétaire" => new secretaire.SecretaireDshboard(),
                    _ => null
                };

                if (page == null)
                {
                    MessageBox.Show(
                        "Le poste associé à votre compte n'est pas reconnu.",
                        "Accès impossible",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                Hide();
                page.FormClosed += (_, _) => Close();
                page.Show();
            }
            catch (HttpRequestException)
            {
                MessageBox.Show(
                    "Impossible de contacter le serveur. Vérifiez votre connexion Internet.",
                    "Erreur réseau",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Une erreur est survenue pendant la connexion :\n{ex.Message}",
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                if (!IsDisposed)
                    button2.Enabled = true;
            }
        }
        private void pictureBox5_Click(object? sender, EventArgs e)
        {
            _passwordVisible = !_passwordVisible;

            textBox1.UseSystemPasswordChar = !_passwordVisible;

            pictureBox5.Image = _passwordVisible
                ? _eyeImage
                : _eyeOffImage;
        }
        private void label1_Click(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void panel2_Paint(object sender, PaintEventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void pictureBox2_Click(object sender, EventArgs e) { }
        private void panel3_Paint(object sender, PaintEventArgs e) { }

        private void linkLabel2_LinkClicked(
            object sender,
            LinkLabelLinkClickedEventArgs e)
        {
        }

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

    public class LoginResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; } = string.Empty;
    }

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