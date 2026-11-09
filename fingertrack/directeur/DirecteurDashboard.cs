using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using fingertrack;


namespace fingertrack.directeur
{
    public partial class DirecteurDashboard : Form
    {
        private const string SupabaseUrl =
            "https://fndvmiltkpzaazeuqikq.supabase.co";

        private const string SupabasePublishableKey =
            "sb_publishable_E7hpBRmBpjhpuN69YQIFqg_0p2Hyxtc";

        private static readonly HttpClient SupabaseHttp = new HttpClient();

        public DirecteurDashboard(string username = "Administrateur")
        {
            InitializeComponent();

            lblAdmin.Text = "👋 Bonjour\r\n" + username;

            Load += async (sender, e) =>
            {
                try
                {
                    await ChargerStatistiquesAsync();
                    await ChargerPointagesAsync();
                    await ChargerGraphiqueAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Impossible de charger les données depuis Supabase.\r\n\r\n" +
                        ex.Message,
                        "Erreur Supabase",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            };
        }

        private async Task ChargerStatistiquesAsync()
        {
            string aujourdhui = DateTime.Today.ToString("yyyy-MM-dd");

            int totalEtudiants = await CompterLignesAsync(
                "stagiaire",
                "select=id_stagiaire");

            int presents = await CompterLignesAsync(
                "pointage",
                "select=id_pointage" +
                "&date_pointage=eq." + aujourdhui +
                "&statut=eq." + Uri.EscapeDataString("Présent"));

            int absents = await CompterLignesAsync(
                "pointage",
                "select=id_pointage" +
                "&date_pointage=eq." + aujourdhui +
                "&statut=eq." + Uri.EscapeDataString("Absent"));

            AfficherNombre(cardEtudiants, totalEtudiants);
            AfficherNombre(cardPresents, presents);
            AfficherNombre(cardAbsents, absents);
        }

        private async Task ChargerPointagesAsync()
        {
            string url = SupabaseUrl.TrimEnd('/') +
                "/rest/v1/pointage" +
                "?select=id_pointage,date_pointage,heure_arrivee,statut,stagiaire(nom,prenom,CIN)" +
                "&order=date_pointage.desc" +
                "&limit=100";

            using (HttpRequestMessage request =
                   new HttpRequestMessage(HttpMethod.Get, url))
            {
                AjouterCleApi(request);

                using (HttpResponseMessage response =
                       await SupabaseHttp.SendAsync(request))
                {
                    string json = await response.Content.ReadAsStringAsync();
                    VerifierReponse(response, json);

                    using (JsonDocument document = JsonDocument.Parse(json))
                    {
                        dgvActivite.Rows.Clear();

                        int numero = 1;

                        foreach (JsonElement ligne in
                                 document.RootElement.EnumerateArray())
                        {
                            string nomComplet = "";
                            string cin = "";

                            JsonElement stagiaire;

                            if (ligne.TryGetProperty("stagiaire", out stagiaire) &&
                                stagiaire.ValueKind == JsonValueKind.Object)
                            {
                                string nom = LireTexte(stagiaire, "nom");
                                string prenom = LireTexte(stagiaire, "prenom");
                                cin = LireTexte(stagiaire, "CIN");
                                nomComplet = (prenom + " " + nom).Trim();
                            }

                            string date = LireTexte(ligne, "date_pointage");
                            string heure = LireTexte(ligne, "heure_arrivee");
                            string statut = LireTexte(ligne, "statut");

                            dgvActivite.Rows.Add(
                                numero++,
                                nomComplet,
                                cin,
                                date,
                                heure,
                                statut);
                        }
                    }
                }
            }
        }

        private async Task ChargerGraphiqueAsync()
        {
            DateTime fin = DateTime.Today;
            DateTime debut = fin.AddDays(-6);

            string url = SupabaseUrl.TrimEnd('/') +
                "/rest/v1/pointage" +
                "?select=date_pointage,statut" +
                "&date_pointage=gte." + debut.ToString("yyyy-MM-dd") +
                "&date_pointage=lte." + fin.ToString("yyyy-MM-dd") +
                "&limit=1000";

            using (HttpRequestMessage request =
                   new HttpRequestMessage(HttpMethod.Get, url))
            {
                AjouterCleApi(request);

                using (HttpResponseMessage response =
                       await SupabaseHttp.SendAsync(request))
                {
                    string json = await response.Content.ReadAsStringAsync();
                    VerifierReponse(response, json);

                    int[] valeurs = new int[7];

                    using (JsonDocument document = JsonDocument.Parse(json))
                    {
                        foreach (JsonElement ligne in
                                 document.RootElement.EnumerateArray())
                        {
                            string dateTexte = LireTexte(ligne, "date_pointage");
                            string statut = LireTexte(ligne, "statut");

                            if (DateTime.TryParse(dateTexte, out DateTime date) &&
                                statut.Equals(
                                    "Présent",
                                    StringComparison.OrdinalIgnoreCase))
                            {
                                int index = (date.Date - debut.Date).Days;

                                if (index >= 0 && index < valeurs.Length)
                                    valeurs[index]++;
                            }
                        }
                    }

                    AfficherGraphique(debut, valeurs);
                }
            }
        }

        private void AfficherGraphique(DateTime debut, int[] valeurs)
        {
            pnlChart.SuspendLayout();

            for (int i = pnlChart.Controls.Count - 1; i >= 0; i--)
            {
                if (pnlChart.Controls[i] != lblChartTitle)
                    pnlChart.Controls.RemoveAt(i);
            }

            int maximum = 1;

            foreach (int valeur in valeurs)
            {
                if (valeur > maximum)
                    maximum = valeur;
            }

            for (int i = 0; i < valeurs.Length; i++)
            {
                int x = 34 + i * 72;
                int hauteur = valeurs[i] == 0
                    ? 2
                    : Math.Max(8, valeurs[i] * 90 / maximum);

                string jour = debut.AddDays(i).ToString("ddd");

                Label valeurLabel = MakeLabel(
                    valeurs[i].ToString(),
                    x, 42, 48, 18, 8, true, textColor);

                valeurLabel.TextAlign = ContentAlignment.MiddleCenter;

                Panel barre = new Panel
                {
                    Location = new Point(x + 8, 150 - hauteur),
                    Size = new Size(32, hauteur),
                    BackColor = blue
                };

                Label jourLabel = MakeLabel(
                    jour, x, 160, 48, 20, 8, false, textColor);

                jourLabel.TextAlign = ContentAlignment.MiddleCenter;

                pnlChart.Controls.Add(valeurLabel);
                pnlChart.Controls.Add(barre);
                pnlChart.Controls.Add(jourLabel);
            }

            lblChartTitle.BringToFront();
            pnlChart.ResumeLayout();
        }

        private async Task<int> CompterLignesAsync(string table, string query)
        {
            string url = SupabaseUrl.TrimEnd('/') +
                         "/rest/v1/" + table +
                         "?" + query;

            using (HttpRequestMessage request =
                   new HttpRequestMessage(HttpMethod.Get, url))
            {
                AjouterCleApi(request);
                request.Headers.TryAddWithoutValidation("Prefer", "count=exact");
                request.Headers.Range = new RangeHeaderValue(0, 0);

                using (HttpResponseMessage response =
                       await SupabaseHttp.SendAsync(request))
                {
                    string body = await response.Content.ReadAsStringAsync();
                    VerifierReponse(response, body);

                    IEnumerable<string> rangeValues;

                    if (response.Content.Headers.TryGetValues(
                            "Content-Range", out rangeValues))
                    {
                        string contentRange = string.Join("", rangeValues);
                        int slashIndex = contentRange.LastIndexOf('/');

                        if (slashIndex >= 0 &&
                            int.TryParse(
                                contentRange.Substring(slashIndex + 1),
                                out int count))
                        {
                            return count;
                        }
                    }

                    return 0;
                }
            }
        }

        private static void AjouterCleApi(HttpRequestMessage request)
        {
            request.Headers.TryAddWithoutValidation(
                "apikey", SupabasePublishableKey);
        }

        private static void VerifierReponse(
            HttpResponseMessage response, string body)
        {
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(
                    "Supabase a retourné " +
                    (int)response.StatusCode + " " +
                    response.ReasonPhrase + "\r\n" + body);
            }
        }

        private static string LireTexte(JsonElement element, string propriete)
        {
            if (element.TryGetProperty(propriete, out JsonElement valeur) &&
                valeur.ValueKind != JsonValueKind.Null)
            {
                return valeur.ToString();
            }

            return "";
        }

        private void AfficherNombre(Panel carte, int nombre)
        {
            foreach (Control control in carte.Controls)
            {
                if (control is Label label &&
                    label.Name == "numberLabel")
                {
                    label.Text = nombre.ToString();
                    return;
                }
            }
        }
        //navigation
        private void Shortcut_Click(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                MessageBox.Show(
                    "Raccourci sélectionné : " + Convert.ToString(button.Tag),
                    "FingerTrack",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }
        private void Navigation_Click(object sender, EventArgs e)
        {
            if (!(sender is Button button))
                return;

            foreach (Control control in pnlSidebar.Controls)
            {
                if (control is Button navButton)
                    navButton.BackColor = navy;
            }

            button.BackColor = blue;
            if (sender == btnPointages)
            {
                EmpruntStagiere page = new EmpruntStagiere();
                page.Show(this);
                return;
            }
          

               
            }
        }
    }
