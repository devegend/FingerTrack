using System;
using System.Drawing;
using System.Windows.Forms;

namespace fingertrack.directeur
{
    partial class DirecteurDashboard
    {
       private Panel pnlSidebar, pnlTopbar, pnlStats;
protected Panel pnlMain;
        private Panel pnlChart, pnlScanner, pnlActivity, pnlShortcuts;
        private Panel cardEtudiants, cardPresents, cardAbsents, cardCours;

        private Label lblLogo, lblLogoSub, lblFooter;
        private Label lblWelcome, lblDescription, lblDate, lblAdmin;
        private Label lblChartTitle, lblScannerIcon, lblScannerTitle, lblScannerSub;
        private Label lblActivityTitle, lblShortcutsTitle;

        private TextBox txtSearch;
        private DataGridView dgvActivite;

        private Button btnDashboard, btnEtudiants, btnEmployes, btnPointages;
        private Button btnRapports, btnAPropos, btnDeconnexion;
        private Button btnAjouterEtudiant, btnEmploiTemps;
        private Button btnVoirRapports, btnParametresRapide, btnVoirTout;

        private readonly Color navy = Color.FromArgb(4, 23, 65);
        private readonly Color blue = Color.FromArgb(0, 105, 245);
        private readonly Color pageColor = Color.FromArgb(241, 245, 255);
        private readonly Color textColor = Color.FromArgb(10, 28, 76);
      
        private void InitializeComponent()
        {
            pnlSidebar = new Panel();
            pnlTopbar = new Panel();
            pnlMain = new Panel();
            pnlStats = new Panel();
            pnlChart = new Panel();
            pnlScanner = new Panel();
            pnlActivity = new Panel();
            pnlShortcuts = new Panel();

            cardEtudiants = new Panel();
            cardPresents = new Panel();
            cardAbsents = new Panel();
            cardCours = new Panel();

            lblLogo = MakeLabel("FingerTrack", 18, 45, 184, 36, 20, true, Color.White);
            lblLogoSub = MakeLabel("Système de Pointage", 18, 81, 184, 24, 9, false, Color.LightSteelBlue);
            lblFooter = MakeLabel("FingerTrack\r\nScanner • Suivez • Gérez", 18, 690, 190, 45, 9, false, Color.White);

            lblWelcome = MakeLabel("Bienvenue dans l'espace Miage Sidi Slimane!", 20, 10, 460, 36, 19, true, textColor);
            lblDescription = MakeLabel("Système de pointage des étudiants par empreinte digitale", 20, 46, 540, 24, 10, false, textColor);

            lblDate = MakeLabel("Aujourd'hui : " + DateTime.Now.ToString("dd/MM/yyyy"),
                0, 15, 165, 55, 9, true, textColor);

            lblAdmin = MakeLabel("Bonjour\r\nIssam Loudi",
                0, 10, 190, 58, 10, true, textColor);

            lblChartTitle = MakeLabel("Aperçu des présences", 15, 8, 350, 30, 13, true, textColor);
            lblScannerIcon = MakeLabel("O", 0, 20, 360, 90, 52, true, Color.DeepSkyBlue);
            lblScannerTitle = MakeLabel("Système actif", 20, 125, 330, 30, 15, true, Color.White);
            lblScannerSub = MakeLabel("Prêt pour le pointage...", 20, 158, 330, 25, 10, false, Color.LightBlue);
            lblActivityTitle = MakeLabel("Activité récente", 14, 8, 260, 30, 13, true, textColor);
            lblShortcutsTitle = MakeLabel("Raccourcis", 14, 8, 260, 30, 13, true, textColor);

            txtSearch = new TextBox();
            dgvActivite = new DataGridView();

            btnDashboard = MakeNavButton("⌂   Tableau de bord", true);
            btnEtudiants = MakeNavButton("♙   Stagière", false);
            btnEmployes = MakeNavButton("▦   Employés", false);
            btnPointages = MakeNavButton("✓   Pointages", false);
            btnRapports = MakeNavButton("▥   Rapports", false);
            btnAPropos = MakeNavButton("ⓘ   À propos", false);
            btnDeconnexion = MakeNavButton("⇥   Déconnexion", false);
            
            btnAjouterEtudiant = MakeShortcut("Ajouter étudiant");
            btnEmploiTemps = MakeShortcut("Emploi du temps");
            btnVoirRapports = MakeShortcut("Voir rapports");
            btnParametresRapide = MakeShortcut("Paramètres");
            btnVoirTout = new Button();

            SuspendLayout();

            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1216, 788);
            MinimumSize = new Size(1216, 788);
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Normal;
            Text = "FingerTrack - Tableau de bord";
            BackColor = pageColor;
            Font = new Font("Segoe UI", 9F);

            // Barre latérale
            pnlSidebar.Dock = DockStyle.Left;
            pnlSidebar.Width = 220;
            pnlSidebar.BackColor = navy;

            Button[] navButtons =
            {
                btnDashboard, btnEtudiants, btnEmployes, btnPointages,
                btnRapports, btnAPropos, btnDeconnexion
            };

            for (int i = 0; i < navButtons.Length; i++)
            {
                navButtons[i].Location = new Point(12, 145 + i * 53);
                navButtons[i].Click += Navigation_Click;
                pnlSidebar.Controls.Add(navButtons[i]);
            }

            btnDeconnexion.Location = new Point(12, 620);
            btnDeconnexion.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            lblFooter.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;

            pnlSidebar.Controls.Add(lblLogo);
            pnlSidebar.Controls.Add(lblLogoSub);
            pnlSidebar.Controls.Add(lblFooter);

            // Barre du haut : recherche, date et administrateur
            pnlTopbar.Dock = DockStyle.Top;
            pnlTopbar.Height = 82;
            pnlTopbar.BackColor = Color.White;

            txtSearch.Location = new Point(20, 24);
            txtSearch.Size = new Size(330, 30);
            txtSearch.Font = new Font("Segoe UI", 10F);
            txtSearch.Text = "Rechercher un étudiant...";

            lblDate.Location = new Point(600, 15);
            lblDate.Size = new Size(165, 55);

            lblAdmin.Location = new Point(780, 10);
            lblAdmin.Size = new Size(190, 58);

            pnlTopbar.Controls.Add(txtSearch);
            pnlTopbar.Controls.Add(lblDate);
            pnlTopbar.Controls.Add(lblAdmin);

            // Zone principale
            pnlMain.Dock = DockStyle.Fill;
            pnlMain.BackColor = pageColor;
            pnlMain.AutoScroll = false;

            pnlStats.Location = new Point(20, 82);
            pnlStats.Size = new Size(960, 112);
            pnlStats.BackColor = Color.Transparent;

            SetupStatCard(cardEtudiants, "128", "Total étudiants", "ET",
                Color.FromArgb(59, 151, 255), 0);
            SetupStatCard(cardPresents, "112", "Présents aujourd'hui", "PR",
                Color.FromArgb(43, 195, 155), 240);
            SetupStatCard(cardAbsents, "16", "Absents aujourd'hui", "AB",
                Color.FromArgb(255, 105, 120), 480);
            SetupStatCard(cardCours, "7", "Cours aujourd'hui", "CR",
                Color.FromArgb(142, 112, 255), 720);
           
            pnlStats.Controls.Add(cardEtudiants);
            pnlStats.Controls.Add(cardPresents);
            pnlStats.Controls.Add(cardAbsents);
            pnlStats.Controls.Add(cardCours);

            // Graphique
            pnlChart.Location = new Point(20, 210);
            pnlChart.Size = new Size(560, 205);
            pnlChart.BackColor = Color.White;
            pnlChart.BorderStyle = BorderStyle.FixedSingle;
            pnlChart.Controls.Add(lblChartTitle);
            BuildChart();

            // Scanner
            pnlScanner.Location = new Point(600, 210);
            pnlScanner.Size = new Size(380, 205);
            pnlScanner.BackColor = navy;
            pnlScanner.Controls.Add(lblScannerIcon);
            pnlScanner.Controls.Add(lblScannerTitle);
            pnlScanner.Controls.Add(lblScannerSub);

            // Activité
            pnlActivity.Location = new Point(20, 430);
            pnlActivity.Size = new Size(620, 250);
            pnlActivity.BackColor = Color.White;
            pnlActivity.BorderStyle = BorderStyle.FixedSingle;

            btnVoirTout.Text = "Voir tout";
            btnVoirTout.Location = new Point(495, 8);
            btnVoirTout.Size = new Size(105, 29);
            btnVoirTout.FlatStyle = FlatStyle.Flat;
            btnVoirTout.FlatAppearance.BorderSize = 0;
            btnVoirTout.BackColor = Color.FromArgb(239, 245, 255);
            btnVoirTout.ForeColor = blue;

            dgvActivite.Location = new Point(12, 48);
            dgvActivite.Size = new Size(594, 185);
            dgvActivite.AllowUserToAddRows = false;
            dgvActivite.AllowUserToDeleteRows = false;
            dgvActivite.ReadOnly = true;
            dgvActivite.RowHeadersVisible = false;
            dgvActivite.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvActivite.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvActivite.BackgroundColor = Color.White;
            dgvActivite.BorderStyle = BorderStyle.None;
            dgvActivite.EnableHeadersVisualStyles = false;
            dgvActivite.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(231, 238, 252);
            dgvActivite.ColumnHeadersDefaultCellStyle.ForeColor = textColor;
            dgvActivite.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            dgvActivite.RowTemplate.Height = 29;

            dgvActivite.Columns.Add("Numero", "#");
            dgvActivite.Columns.Add("Etudiant", "Étudiant");
            dgvActivite.Columns.Add("Matricule", "Matricule");
            dgvActivite.Columns.Add("Date", "Date");
            dgvActivite.Columns.Add("Heure", "Heure");
            dgvActivite.Columns.Add("Statut", "Statut");


            pnlActivity.Controls.Add(lblActivityTitle);
            pnlActivity.Controls.Add(btnVoirTout);
            pnlActivity.Controls.Add(dgvActivite);

            // Raccourcis
            pnlShortcuts.Location = new Point(660, 430);
            pnlShortcuts.Size = new Size(320, 250);
            pnlShortcuts.BackColor = Color.White;
            pnlShortcuts.BorderStyle = BorderStyle.FixedSingle;

            btnAjouterEtudiant.Location = new Point(12, 48);
            btnEmploiTemps.Location = new Point(165, 48);
            btnVoirRapports.Location = new Point(12, 145);
            btnParametresRapide.Location = new Point(165, 145);

            btnAjouterEtudiant.Tag = "Ajouter étudiant";
            btnEmploiTemps.Tag = "Emploi du temps";
            btnVoirRapports.Tag = "Voir rapports";
            btnParametresRapide.Tag = "Paramètres";

            btnAjouterEtudiant.Click += Shortcut_Click;
            btnEmploiTemps.Click += Shortcut_Click;
            btnVoirRapports.Click += Shortcut_Click;
            btnParametresRapide.Click += Shortcut_Click;

            pnlShortcuts.Controls.Add(lblShortcutsTitle);
            pnlShortcuts.Controls.Add(btnAjouterEtudiant);
            pnlShortcuts.Controls.Add(btnEmploiTemps);
            pnlShortcuts.Controls.Add(btnVoirRapports);
            pnlShortcuts.Controls.Add(btnParametresRapide);

            // Assemblage
            pnlMain.Controls.Add(lblWelcome);
            pnlMain.Controls.Add(lblDescription);
            pnlMain.Controls.Add(pnlStats);
            pnlMain.Controls.Add(pnlChart);
            pnlMain.Controls.Add(pnlScanner);
            pnlMain.Controls.Add(pnlActivity);
            pnlMain.Controls.Add(pnlShortcuts);

            Controls.Add(pnlMain);
            Controls.Add(pnlTopbar);
            Controls.Add(pnlSidebar);

            ResumeLayout(false);
        }

        private Label MakeLabel(
            string text, int x, int y, int width, int height,
            float fontSize, bool bold, Color color)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, height),
                Font = new Font("Segoe UI", fontSize,
                    bold ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = color,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft
            };
        }

        private Button MakeNavButton(string text, bool active)
        {
            Button button = new Button
            {
                Text = text,
                Size = new Size(196, 44),
                FlatStyle = FlatStyle.Flat,
                BackColor = active ? blue : navy,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false
            };

            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(20, 55, 110);
            return button;
        }

        private Button MakeShortcut(string text)
        {
            Button button = new Button
            {
                Text = text,
                Size = new Size(140, 82),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(239, 245, 255),
                ForeColor = blue,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false
            };

            button.FlatAppearance.BorderColor = Color.FromArgb(190, 210, 245);
            button.FlatAppearance.BorderSize = 1;
            return button;
        }

        private void SetupStatCard(
            Panel panel, string number, string title,
            string symbol, Color accent, int x)
        {
            panel.Location = new Point(x, 0);
            panel.Size = new Size(230, 105);
            panel.BackColor = Color.White;
            panel.BorderStyle = BorderStyle.FixedSingle;

            Label icon = new Label
            {
                Text = symbol,
                Location = new Point(12, 22),
                Size = new Size(62, 58),
                BackColor = accent,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label numberLabel = new Label
            {
                Name = "numberLabel",
                Text = number,
                Location = new Point(84, 14),
                Size = new Size(130, 38),
                ForeColor = textColor,
                Font = new Font("Segoe UI", 20F, FontStyle.Bold)
            };

            Label titleLabel = new Label
            {
                Text = title,
                Location = new Point(84, 56),
                Size = new Size(138, 37),
                ForeColor = textColor,
                Font = new Font("Segoe UI", 8F)
            };

            panel.Controls.Add(icon);
            panel.Controls.Add(numberLabel);
            panel.Controls.Add(titleLabel);
        }

        private void BuildChart()
        {
            string[] days = { "Lun", "Mar", "Mer", "Jeu", "Ven", "Sam", "Dim" };
            int[] values = { 120, 118, 122, 115, 112, 110, 108 };
            int[] heights = { 82, 78, 86, 70, 65, 61, 56 };

            for (int i = 0; i < days.Length; i++)
            {
                int x = 34 + i * 72;

                Label valueLabel = MakeLabel(
                    values[i].ToString(), x, 42, 48, 18, 8, true, textColor);
                valueLabel.TextAlign = ContentAlignment.MiddleCenter;

                Panel bar = new Panel
                {
                    Location = new Point(x + 8, 155 - heights[i]),
                    Size = new Size(32, heights[i]),
                    BackColor = blue
                };

                Label dayLabel = MakeLabel(
                    days[i], x, 160, 48, 20, 8, false, textColor);
                dayLabel.TextAlign = ContentAlignment.MiddleCenter;

                pnlChart.Controls.Add(valueLabel);
                pnlChart.Controls.Add(bar);
                pnlChart.Controls.Add(dayLabel);
            }
        }
    }
}