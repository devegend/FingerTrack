using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace fingertrack.directeur
{
    public partial class EmpruntStagiere : DirecteurDashboard
    {
        private readonly System.Windows.Forms.Timer timerPointage =
            new System.Windows.Forms.Timer();

        private Label lblHeure;
        private Label lblDate;
        private ComboBox cmbFiliere;
        private ComboBox cmbBranche;
        private ComboBox cmbNiveau;

        private readonly Color couleurTexte = Color.FromArgb(10, 28, 76);
        private readonly Color bleu = Color.FromArgb(0, 105, 245);

        public EmpruntStagiere() : base("Issam Loudi")
        {
            ConstruirePage();

            Text = "Gestion et pointage des stagiaires";

            timerPointage.Interval = 1000;
            timerPointage.Tick += TimerPointage_Tick;
            timerPointage.Start();
        }

        private void ConstruirePage()
        {
            pnlMain.Controls.Clear();
            pnlMain.BackColor = Color.FromArgb(241, 245, 255);
            pnlMain.AutoScroll = true;

            Label titrePage = new Label
            {
                Text = "Gestion et pointage des stagiaires",
                AutoSize = true,
                Location = new Point(20, 16),
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = couleurTexte
            };

            Panel formulaire = new Panel
            {
                Location = new Point(20, 65),
                Size = new Size(570, 500),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label titreFormulaire = new Label
            {
                Text = "Informations du stagiaire",
                AutoSize = true,
                Location = new Point(22, 18),
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = couleurTexte
            };

            TextBox txtNom = AjouterChamp(formulaire, "Nom", 22, 65);
            TextBox txtPrenom = AjouterChamp(formulaire, "Prénom", 300, 65);
            TextBox txtCIN = AjouterChamp(formulaire, "CIN", 22, 155);

            cmbFiliere = AjouterListe(
                formulaire, "Filière", 300, 155,
                "Technicien Spécialisé",
                "Technicien");

            cmbBranche = AjouterListe(
                formulaire, "Branche", 22, 245);

            cmbNiveau = AjouterListe(
                formulaire, "Niveau", 300, 245);

            cmbFiliere.SelectedIndexChanged += cmbFiliere_SelectedIndexChanged;
            cmbBranche.SelectedIndexChanged += cmbBranche_SelectedIndexChanged;

            Button btnEnregistrer = new Button
            {
                Text = "Enregistrer",
                Location = new Point(22, 345),
                Size = new Size(170, 42),
                BackColor = bleu,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnEnregistrer.FlatAppearance.BorderSize = 0;

            btnEnregistrer.Click += (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtNom.Text) ||
                    string.IsNullOrWhiteSpace(txtPrenom.Text) ||
                    string.IsNullOrWhiteSpace(txtCIN.Text) ||
                    cmbFiliere.SelectedItem == null ||
                    cmbBranche.SelectedItem == null ||
                    cmbNiveau.SelectedItem == null)
                {
                    MessageBox.Show(
                        "Complétez les informations et choisissez filière, branche et niveau.",
                        "Formulaire incomplet",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                MessageBox.Show(
                    "Les informations sont prêtes. Le bouton devra être relié à Supabase pour enregistrer le stagiaire.",
                    "Stagiaire",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            };

            formulaire.Controls.Add(titreFormulaire);
            formulaire.Controls.Add(btnEnregistrer);

            Panel carteScan = new Panel
            {
                Location = new Point(610, 65),
                Size = new Size(340, 430),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label titreScan = new Label
            {
                Text = "Pointage biométrique",
                AutoSize = true,
                Location = new Point(20, 18),
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = couleurTexte
            };

            Label poste = new Label
            {
                Text = "Poste d’accueil · Siège MAROC",
                Location = new Point(15, 58),
                Size = new Size(310, 25),
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(70, 90, 120),
                TextAlign = ContentAlignment.MiddleCenter
            };

            lblHeure = new Label
            {
                Text = HeureMaroc().ToString("HH:mm:ss"),
                Location = new Point(15, 92),
                Size = new Size(310, 60),
                Font = new Font("Segoe UI", 31F, FontStyle.Bold),
                ForeColor = couleurTexte,
                TextAlign = ContentAlignment.MiddleCenter
            };

            lblDate = new Label
            {
                Text = HeureMaroc().ToString(
                    "dd MMMM yyyy",
                    new CultureInfo("fr-FR")),
                Location = new Point(15, 150),
                Size = new Size(310, 26),
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(120, 140, 165),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Panel empreinte = new Panel
            {
                Location = new Point(100, 190),
                Size = new Size(140, 140),
                BackColor = Color.FromArgb(240, 246, 255)
            };

            empreinte.Paint += (sender, e) =>
            {
                e.Graphics.SmoothingMode =
                    System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                using (Pen stylo = new Pen(Color.FromArgb(50, 115, 255), 3))
                {
                    e.Graphics.DrawEllipse(stylo, 18, 18, 104, 104);
                    e.Graphics.DrawEllipse(stylo, 32, 32, 76, 76);
                    e.Graphics.DrawArc(stylo, 43, 43, 54, 54, 200, 280);
                }
            };

            Button btnScan = new Button
            {
                Text = "SCAN",
                Location = new Point(95, 345),
                Size = new Size(150, 42),
                BackColor = bleu,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnScan.FlatAppearance.BorderSize = 0;

            btnScan.Click += (sender, e) =>
            {
                MessageBox.Show(
                    "Mode démonstration : aucun lecteur physique connecté.",
                    "Pointage biométrique",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            };

            carteScan.Controls.Add(titreScan);
            carteScan.Controls.Add(poste);
            carteScan.Controls.Add(lblHeure);
            carteScan.Controls.Add(lblDate);
            carteScan.Controls.Add(empreinte);
            carteScan.Controls.Add(btnScan);

            pnlMain.Controls.Add(titrePage);
            pnlMain.Controls.Add(formulaire);
            pnlMain.Controls.Add(carteScan);
        }

        private TextBox AjouterChamp(Panel parent, string titre, int x, int y)
        {
            Label label = new Label
            {
                Text = titre,
                Location = new Point(x, y),
                Size = new Size(245, 22),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = couleurTexte
            };

            TextBox champ = new TextBox
            {
                Location = new Point(x, y + 27),
                Size = new Size(245, 32),
                Font = new Font("Segoe UI", 10F)
            };

            parent.Controls.Add(label);
            parent.Controls.Add(champ);
            return champ;
        }

        private ComboBox AjouterListe(
            Panel parent, string titre, int x, int y, params string[] choix)
        {
            Label label = new Label
            {
                Text = titre,
                Location = new Point(x, y),
                Size = new Size(245, 22),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = couleurTexte
            };

            ComboBox liste = new ComboBox
            {
                Location = new Point(x, y + 27),
                Size = new Size(245, 32),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F)
            };

            if (choix.Length > 0)
                liste.Items.AddRange(choix);

            parent.Controls.Add(label);
            parent.Controls.Add(liste);
            return liste;
        }

        private void cmbFiliere_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbBranche.Items.Clear();
            cmbNiveau.Items.Clear();

            string filiere = Convert.ToString(cmbFiliere.SelectedItem);

            if (filiere == "Technicien Spécialisé")
            {
                cmbBranche.Items.AddRange(new object[]
                {
                    "Développement Informatique",
                    "Réseaux Informatiques",
                    "Gestion informatisée"
                });
            }
            else if (filiere == "Technicien")
            {
                cmbBranche.Items.Add("Gestion informatisée");
            }
        }

        private void cmbBranche_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbNiveau.Items.Clear();

            string branche = Convert.ToString(cmbBranche.SelectedItem);

            if (branche == "Développement Informatique" ||
                branche == "Réseaux Informatiques")
            {
                cmbNiveau.Items.AddRange(new object[]
                {
                    "1ère année",
                    "2ème année",
                    "Licence"
                });
            }
            else if (branche == "Gestion informatisée")
            {
                cmbNiveau.Items.AddRange(new object[]
                {
                    "1ère année",
                    "2ème année"
                });
            }
        }

        private DateTime HeureMaroc()
        {
            try
            {
                TimeZoneInfo maroc =
                    TimeZoneInfo.FindSystemTimeZoneById("Morocco Standard Time");

                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, maroc);
            }
            catch (TimeZoneNotFoundException)
            {
                return DateTime.Now;
            }
        }

        private void TimerPointage_Tick(object sender, EventArgs e)
        {
            DateTime maintenant = HeureMaroc();

            lblHeure.Text = maintenant.ToString("HH:mm:ss");
            lblDate.Text = maintenant.ToString(
                "dd MMMM yyyy",
                new CultureInfo("fr-FR"));
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            timerPointage.Stop();
            timerPointage.Dispose();
            base.OnFormClosed(e);
        }
    }
}