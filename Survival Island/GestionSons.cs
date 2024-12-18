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

namespace Survival_Island
{
    public class GestionSons
    {
        public MediaPlayer MediaPlayerMusique { get; }
        public SoundPlayer SoundPlayerTire { get; }

        public GestionSons()
        {
            MediaPlayerMusique = new MediaPlayer();

            MediaPlayerMusique.Open(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Chemin.MUSIQUE_FOND)));
            MediaPlayerMusique.Volume = Constante.MUSIQUE_VOLUME;
            MediaPlayerMusique.MediaEnded += (sender, e) => MediaPlayerMusique.Position = TimeSpan.Zero;
            MediaPlayerMusique.Play();

            SoundPlayerTire = new SoundPlayer(Application.GetResourceStream(new Uri(Chemin.SON_TIRE)).Stream);
        }
    }
}
