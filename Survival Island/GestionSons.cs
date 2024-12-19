using Survival_Island.Outils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Survival_Island
{
    public class GestionSons
    {
        public MediaPlayer MediaPlayerMusique { get; }

        public MediaPlayer[] Musiques { get; set; }
        //public SoundPlayer SoundPlayerTire { get; }
        public SoundPlayer[] Sons { get; set; }
        public int IndiceMusiqueJoue { get; set; }
        public int IndiceSonJoue { get; set; }

        public GestionSons()
        {

            InitMusique();
            InitSons();

            IndiceSonJoue = Constante.SON_DE_BASE;
            IndiceMusiqueJoue = Constante.MUSIQUE_DE_BASE;

            Musiques[IndiceMusiqueJoue].Play();

            // Les sounds players n'étant pas idéal, nous utilisons les media players
            //SoundPlayerTire = new SoundPlayer(Application.GetResourceStream(new Uri(Chemin.SON_TIRE)).Stream);
        }

        public void JoueSon()
        {
            Sons[IndiceSonJoue].Play();
        }

        public void SonSuivant()
        {
            
            IndiceSonJoue += 1;
            IndiceSonJoue = IndiceSonJoue % Sons.Length;
            Sons[IndiceSonJoue].Play();

        }
        private void InitSons()
        {
           Sons = ChargerSons(Chemin.SON, Constante.NB_SON, Constante.SON_EXT);

        }

        private SoundPlayer ChargerSon(string chemin)
        {
            return new SoundPlayer(Application.GetResourceStream(new Uri(chemin)).Stream);
        }
        private SoundPlayer[] ChargerSons(string chemin, int nombreSons, string extension)
        {
            SoundPlayer[] sons = new SoundPlayer[nombreSons];
            for (int i = 1; i <= nombreSons; i++)
            {
                sons[i - 1] = ChargerSon(chemin + i + "." + extension);
            }

            return sons;
        }


        private Uri ChargerMusique(string chemin)
        {
            return new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, chemin));
        }

        private Uri[] ChargerMusiques(string chemin, int nombreSons, string extension)
        {
            Uri[] sons = new Uri[nombreSons];
            for (int i = 1; i <= nombreSons; i++)
            {
                sons[i - 1] = ChargerMusique(chemin + i + "." + extension);
            }

            return sons;
        }
        private void InitMusique()
        {
            Uri[] tab = ChargerMusiques(Chemin.MUSIQUE_FOND, Constante.NB_MUSIQUE, Constante.MUSIQUE_EXT);

            Musiques = new MediaPlayer[tab.Length];

            for (int i = 0; i < tab.Length; i++) {
                Musiques[i] = new MediaPlayer();
                Musiques[i].Open(tab[i]);
                Musiques[i].Volume = Constante.MUSIQUE_VOLUME;
                Musiques[i].MediaEnded += Relance;
            }
        }

        public void Relance(object? sender, EventArgs e)
        {
            Musiques[IndiceMusiqueJoue].Position = TimeSpan.Zero;
        }

        public void MusiqueSuivante()
        {
            Musiques[IndiceMusiqueJoue].Stop();

            double volumeMusiqueSuivante = Musiques[IndiceMusiqueJoue].Volume;

            IndiceMusiqueJoue += 1;
            IndiceMusiqueJoue= IndiceMusiqueJoue % Musiques.Length ;
            Musiques[IndiceMusiqueJoue].Volume=volumeMusiqueSuivante;
            Musiques[IndiceMusiqueJoue].Play();

        }
    }
}
