using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Process myProcess;
        DirectoryInfo winTemp;
        DirectoryInfo appTemp;
        FileInfo fdate;

        public MainWindow()
        {
            InitializeComponent();
            myProcess = new Process();
            winTemp = new DirectoryInfo(@"C:\Windows\Temp");
            appTemp = new DirectoryInfo(System.IO.Path.GetTempPath());
            fdate = new FileInfo("date.txt");
            GetDate();
        }

        /// <summary>
        /// Calcule la taille du dossier en paramètre.
        /// </summary>
        /// <param name="dir"></param>
        /// <returns>size</returns>
        public long DirSize (DirectoryInfo dir)
        {
            long size = 0;
            //Gestion de la partie fichier
            FileInfo[] files = dir.GetFiles();
            foreach(FileInfo fi in files)
            {
                size += fi.Length;
            }
            //Gestion de la partie dossier
            DirectoryInfo[] dirs = dir.GetDirectories();
            foreach(DirectoryInfo di in dirs)
            {
                size += DirSize(di);
            }

            return size;
        }

        /// <summary>
        /// Supprime les fichiers et répertoires du dossier entrée en paramètre.
        /// </summary>
        /// <param name="dir"></param>
        public void ClearTempData(DirectoryInfo dir)
        {
            foreach(FileInfo fi in dir.GetFiles())
            {
                try
                {
                    fi.Delete();
                    Console.WriteLine(fi.FullName);
                }
                catch(Exception e)
                {
                    continue;
                }
            }

            foreach(DirectoryInfo di in dir.GetDirectories())
            {
                try
                {
                    di.Delete(true);
                    Console.WriteLine(di.FullName);
                }
                catch(Exception e)
                {
                    continue;
                }
            }
        }

        /// <summary>
        /// Action de clique sur le bouton MISE A JOUR.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void maj_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Votre logiciel est à jour.", "Mise à jour", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Action de clique sur le bouton WEB.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void web_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                myProcess.StartInfo.UseShellExecute = true;
                myProcess.StartInfo.FileName = "https://www.google.fr/";
                myProcess.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Vous n'avez pas de navigateur.", "Erreur de navigation", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Action de clique sur le bouton Analyser.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            analyser.Content = "En cours...";
            AnalysableFolder();
            analyser.Content = "Analyser";
        }

        /// <summary>
        /// Analyse les dossiers winTemp et appTemp, puis affiche sur l'interface.
        /// </summary>
        public void AnalysableFolder()
        {
            long totalSize = 0;

            try
            {
                totalSize += DirSize(winTemp) / 1000000;
                totalSize += DirSize(appTemp) / 1000000;
            }
            catch(Exception e)
            {
                MessageBox.Show("Impossible d'analyser les dossiers temporaires : " + e.Message, "Erreur d'analyse!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            espace.Content = totalSize + " Mo";
            date.Content = DateTime.Today;
            SaveDate();
        }

        /// <summary>
        /// Action de clique du bouton NETTOYER.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nettoyer_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Voulez vous démarrer le nettoyage? Tout les fichiers temporaires seront perdus.", "Nettoyage", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if(result == MessageBoxResult.Yes)
            {
                nettoyer.Content = "EN COURS...";

                try
                {
                    ClearTempData(winTemp);
                    ClearTempData(appTemp);
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Echec de nettoyage : " + ex.Message, "Erreur de nettoyage", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                nettoyer.Content = "TERMINÉ!";
                espace.Content = "0 Mo";
            }
        }

        /// <summary>
        /// Action de clique du bouton HISTORIQUE.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void historique_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Indisponible pour cette version.", "Historique", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Enregistre la date dans le fichier date.txt.
        /// </summary>
        public void SaveDate()
        {
            string date = DateTime.Today.ToString();
            File.WriteAllText("date.txt", date);
        }

        /// <summary>
        /// Récupère la date dans le fichier date.txt.
        /// </summary>
        public void GetDate()
        {
            if(fdate.Exists)
            {
                string fiDate = File.ReadAllText("date.txt");
                if (fiDate != String.Empty)
                {
                    date.Content = fiDate;
                }
            }
        }
    }
}
