using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

public class Avaruustaistelu3 : PhysicsGame
{
    private Pelaaja pelaaja1;
    private Vector p1aloitus = new Vector(150, 0);
    private Pelaaja pelaaja2;
    private Vector p2aloitus = new Vector(-150, 0);
    private bool loppunut = false;
    private int asteroideja = 0;
    private int minAsteroideja = 15;
    private static Image[] taustaKuvat = Game.LoadImages("Tausta4");
    private static Image[] alukset = Game.LoadImages("alus2", "vihuAlus");

    public override void Begin()
    {
        Alkuvalikko("Tervetuloa peliin");
    }


    /// <summary>
    /// Luodaan pelin alkuvalikko. Voi lisätä parhaiden pisteiden taulukon myöhemmin jos haluaa.
    /// </summary>
    /// <param name="otsikkoteksti">Teksti, joka näytetään valikon yläreunassa</param>
    private void Alkuvalikko(string otsikkoteksti)
    {
        MultiSelectWindow valikko = new MultiSelectWindow(otsikkoteksti,
"Aloita peli", /*"Parhaat pisteet", */"Lopeta");
        valikko.ItemSelected += PainettiinValikonNappia;
        Add(valikko);
    }


    /// <summary>
    /// Käsitellään valikon nappien toiminta
    /// </summary>
    /// <param name="valinta">Valikon näppäimet ja mihin niistä mennään</param>
    private void PainettiinValikonNappia(int valinta)
    {
        switch (valinta)
        {
            case 0:
                LuoKentta();// AloitaPeli();
                break;
            /*case 1:
                // ParhaatPisteet();
                break;*/
            case 1:
                Exit();
                break;
        }
    }


    /// <summary>
    /// Luodaan pelialue ja sinne asteroidit
    /// </summary>
    private void LuoKentta()
    {
        ClearAll();
        
        loppunut = false;
        //IsFullScreen = true; //jos haluaa koko näyttöön.
        SetWindowSize(1080, 1080, false);
        Level.Background.Color = Color.Black;
        Level.Height = 6000;
        Level.Width = 6000;
        //Level.Background.Image = taustaKuvat[RandomGen.NextInt(0, taustaKuvat.Length)];
        Level.Background.Image = taustaKuvat[0];
        Level.Background.FitToLevel();

        Level.CreateBorders(5, true);
        //Camera.ZoomToAllObjects();
        
        Camera.ZoomToLevel();
        Camera.StayInLevel = true;
        Gravity = new Vector(0, 0);

        pelaaja1 = new Pelaaja(0, "Pelaaja", "Pelaaja 1", p1aloitus, new Vector(400, 500), Color.Yellow, new Key[] { Key.Up, Key.Down, Key.Left, Key.Right, Key.RightControl, Key.RightShift });

        pelaaja2 = new Pelaaja(1, "Pelaaja", "Pelaaja 2", p2aloitus, new Vector(-400, 500), Color.Red, new Key[] { Key.W, Key.S, Key.A, Key.D, Key.LeftControl, Key.LeftShift });

        PaivitaPelaaja(pelaaja1); //päivitetään vasta, kun molemmat on luo
        PaivitaPelaaja(pelaaja2);

        LuoAsteroidit(20);
    }


    /// <summary>
    /// Luodaan asteroidioliot
    /// </summary>
    /// <param name="maara">Kuinka monta luodaan</param>
    private void LuoAsteroidit(int maara)
    {
        double koko = 550;
        for (int i = 0; i < maara; i++)
        {
            /*
            Vector p1paikka = pelaaja1.Alus.Position;
            Vector p2paikka = pelaaja2.Alus.Position;

            bool nok = true;
            Vector paikka = uusiPaikka(koko);
            double px1 = paikka.X - koko;
            double px2 = paikka.X + koko;
            double py1 = paikka.Y - koko;
            double py2 = paikka.Y + koko;

            while (nok)
            {
                if (px1 <= p1paikka.X && p1paikka.X <= px2) { paikka = uusiPaikka(koko); continue; }
                if (py1 <= p1paikka.Y && p1paikka.Y <= py2) { paikka = uusiPaikka(koko); continue; }
                if (px1 <= p2paikka.X && p1paikka.X <= px2) { paikka = uusiPaikka(koko); continue; }
                if (py1 <= p2paikka.Y && p1paikka.Y <= py2) { paikka = uusiPaikka(koko); continue; }
                nok = false;
            }
            */
            Vector paikka = uusiPaikka(koko);
            var asteroidi = new Asteroidi(this, paikka, RandomGen.NextDouble(80, koko), Color.Gray, "Asteroidi");
            asteroidi.Image = LoadImage("Asteroidi4");
            this.asteroideja += 1;
        }
    }


    private Vector uusiPaikka(double koko)
    {
        return new Vector(RandomGen.NextDouble(Level.Left + koko, Level.Right - koko),
                RandomGen.NextDouble(Level.Bottom + koko, Level.Top - koko));
    }


    /// <summary>
    /// Päivitetään pelaajan tiedot: luodaan alus, kontrollerit, collisionhandlerit ja näytöt
    /// </summary>
    /// <param name="pelaaja">Pelaaja, jota päivitetään</param>
    private void PaivitaPelaaja(Pelaaja pelaaja)
    {
        pelaaja.LuoAlus(this, "Pelaaja", pelaaja.Nimi, pelaaja.Aloitus, pelaaja.Vari);
        pelaaja.Alus.Image = alukset[pelaaja.AlusKuva];
        //AsetaNappaimet(pelaaja, pelaaja.Nappaimet);
        PaivitaNappaimet();
        pelaaja.PoistaNaytot();       
        pelaaja.LuoNaytot(this);
        this.AddCollisionHandler(pelaaja.Alus, "Asteroidi", Tormays);
        this.AddCollisionHandler(pelaaja.Alus, "Pelaaja", Tormays);
        pelaaja.Alus.Ase1.ProjectileCollision = AmmusOsui1;
        pelaaja.Alus.Ase2.ProjectileCollision = AmmusOsui2;

    }


    /// <summary>
    /// Poistetaan vanhat kontrollit, jotka voivat viitata vielä mahdollisesti olemassa oleviin alus-olioihin.
    /// Null-tarkastus aivan ensimmäistä luontikertaa varten.
    /// </summary>
    private void PaivitaNappaimet()
    {
        ClearControls();
        AsetaNappaimet(pelaaja1, pelaaja1.Nappaimet);
        if (pelaaja2.Alus == null) return;
        AsetaNappaimet(pelaaja2, pelaaja2.Nappaimet);
    }


    /// <summary>
    /// Asetetaan näppäimet pelaajalle.
    /// </summary>
    /// <param name="pelaaja">Pelaaja, jolle asetetaan</param>
    /// <param name="nappaimet">Taulukko näppäimistä</param>
    private void AsetaNappaimet(Pelaaja pelaaja, Key[] nappaimet)
    {
        Keyboard.Listen(nappaimet[0], ButtonState.Down, pelaaja.Alus.LyoAlusta, "Lyö alusta ylöspäin", 200.0);
        Keyboard.Listen(nappaimet[1], ButtonState.Down, pelaaja.Alus.LyoAlusta, "Lyö alusta alaspäin", -100.0);
        Keyboard.Listen(nappaimet[2], ButtonState.Down, delegate { pelaaja.Alus.Angle += Angle.FromDegrees(7.0); }, "Käännä alusta vasemmalle");
        Keyboard.Listen(nappaimet[3], ButtonState.Down, delegate { pelaaja.Alus.Angle += Angle.FromDegrees(-7.0); }, "Käännä alusta oikealle");
        //Keyboard.Listen(nappaimet[4], ButtonState.Down, delegate { PhysicsObject ammus = pelaaja.Alus.Ase1.Shoot(); }, "Ammu aseella 1");
        Keyboard.Listen(nappaimet[4], ButtonState.Down, pelaaja.Alus.AmmuAseella1, "Ammu aseella 1");
        Keyboard.Listen(nappaimet[5], ButtonState.Down, pelaaja.Alus.AmmuAseella2, "Ammu aseella 2");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }


    /// <summary>
    /// Aliohjelmalla käsitellään kappaleiden törmäykset
    /// </summary>
    /// <param name="osuja">Määritellään kuka on osuja</param>
    /// <param name="kohde">Mihin törmätään</param>
    public void Tormays(PhysicsObject osuja, PhysicsObject kohde)
    {
        // Kaksi eri tapaa kirjoittaa sama.
        Explosion rajahdys = new Explosion(100)
        {
            Position = kohde.Position,
            UseShockWave = false
        };
        Add(rajahdys);

        Explosion rajahdys2 = new Explosion(100);
        rajahdys2.Position = osuja.Position;
        rajahdys2.UseShockWave = false;
        Add(rajahdys2);

        TarkistaKohde(osuja, 1);
        TarkistaKohde(kohde, 1);
    }


    /// <summary>
    /// Ammuksen osumisen käsittelevä aliohjelma
    /// </summary>
    /// <param name="ammus">Ammus</param>
    /// <param name="kohde">Kohde, johon ammus osuu</param>
    private void AmmusOsui1(PhysicsObject ammus, PhysicsObject kohde)
    {
        Explosion rajahdys = new Explosion(100);
        rajahdys.Position = kohde.Position;
        rajahdys.UseShockWave = false;
        Add(rajahdys);
        ammus.Destroy();

        TarkistaKohde(kohde, 13);
    }


    /// <summary>
    /// Ammuksen osumisen käsittelevä aliohjelma
    /// </summary>
    /// <param name="ammus">Ammus</param>
    /// <param name="kohde">Kohde, johon ammus osuu</param>
    private void AmmusOsui2(PhysicsObject ammus, PhysicsObject kohde)
    {
        Explosion rajahdys = new Explosion(40);
        rajahdys.Position = kohde.Position;
        rajahdys.UseShockWave = false;
        Add(rajahdys);
        ammus.Destroy();

        TarkistaKohde(kohde, 2);
    }


    /// <summary>
    /// Tarkistetaan mihin ollaan osuttu (pelaaja/asteroidi)
    /// </summary>
    /// <param name="kohde">Mihin ollaan osuttu</param>
    /// <param name="maara">Kuinka paljon tulee osumasta kohteelle vahinkoa</param>
    private void TarkistaKohde(PhysicsObject kohde, int maara)
    {
        //voisi olla myös taulukkototeutus jos on paljon kohteita. Nyt vain kaksi joilla on merkitystä
        if ((string)kohde.Tag == "Pelaaja") MuutaLaskuria((Alus)kohde, maara);
        else if ((string)kohde.Tag == "Asteroidi") MuutaLaskuria((Asteroidi)kohde, maara);
    }


    /// <summary>
    /// Muutetaan pelaajan aluksen elämälaskurin arvoa
    /// </summary>
    /// <param name="alus">Pelaajan alus</param>
    /// <param name="maara">Elämälaskurin muutoksen määrä</param> 
    private void MuutaLaskuria(Alus alus, int maara)
    {
        alus.ElamaLaskuri.Value -= maara;
        if (alus.Pelaaja.Equals(pelaaja1.Nimi)) //voisi ehkä suoraan verrata objekteja, mutta ainakin nimellä onnistuu. Aluksen ja pelaajan nimi ovat samat tässä vaiheessa.
        {
            TarkistaPelaaja(pelaaja1);
            return;
        }
        TarkistaPelaaja(pelaaja2);
    }


    /// <summary>
    /// Muutetaan pelaajan aluksen elämälaskurin arvoa
    /// </summary>
    /// <param name="asteroidi">Asteroidi, johon on osuttu</param>
    /// <param name="maara">Elämälaskurin muutoksen määrä</param> 
    private void MuutaLaskuria(Asteroidi asteroidi, int maara)
    {
        asteroidi.ElamaLaskuri.Value -= maara;
        if (asteroidi.ElamaLaskuri == 0)
        {
            asteroidi.Destroy();

            KasitteleRajahdys(asteroidi);
            asteroideja--;
            if (asteroideja < minAsteroideja)
            {
                Timer respawnLaskuri = new Timer();
                respawnLaskuri.Interval = 3;
                respawnLaskuri.Timeout += delegate {
                    LuoAsteroidit(minAsteroideja - asteroideja);
                };
                respawnLaskuri.Start(1);
            }
        }
    }


    private void KasitteleRajahdys(PhysicsObject kohde)
    {
        Explosion rajahdys = new Explosion(kohde.Width)
        {
            Position = kohde.Position,
            UseShockWave = false
        };
        Add(rajahdys);
    }


    /// <summary>
    /// Tarkistetaan kumman pelaajan alus on kyseessä. Vain kaksi alusta voi olla olemassa tässä vaiheessa, joten if/else toimii
    /// </summary>
    /// <param name="pelaaja">Pelaaja, jonka aluksen elämälaskuri tarkastetaan</param>
    public void TarkistaPelaaja(Pelaaja pelaaja)
    {
        if (pelaaja.Alus.ElamaLaskuri == 0)
        {
            pelaaja.Alus.Ase1.Destroy();
            pelaaja.Alus.Destroy();
            KasitteleRajahdys(pelaaja.Alus);

            pelaaja.AlusLaskuri.Value -= 1;

            TarkistaLoppu(pelaaja);
        }
    }


    /// <summary>
    /// Tarkistetaan onko pelaajalla vielä aluksia jäljellä edellisen tuhoutumisen jälkeen
    /// </summary>
    /// <param name="pelaaja">Tarkastettava pelaaja</param>
    public void TarkistaLoppu(Pelaaja pelaaja)
    {
        if (pelaaja.AlusLaskuri.Value == 0)
        {
            string voittaja = " voitti!";
            string nimi;
            if (pelaaja.Nimi.Equals(pelaaja1.Nimi)) nimi = pelaaja2.Nimi;
            else nimi = pelaaja1.Nimi;
            PeliLoppui(nimi + voittaja);
        }

        Timer respawnLaskuri = new Timer();
        respawnLaskuri.Interval = 3;
        respawnLaskuri.Timeout += delegate {
            PaivitaPelaaja(pelaaja);
        };
        respawnLaskuri.Start(1);

    }


    /// <summary>
    /// Pelin lopetuksen käsittely
    /// </summary>
    /// <param name="viesti">Pelin lopetuksessa näytettävä viesti</param>
    private void PeliLoppui(string viesti)
    {
        if (loppunut) return;
        loppunut = true;
        NaytaViesti(viesti);
        Alkuvalikko("Aloitetaanko uusi peli?");
    }


    /// <summary>
    /// Luodaan lopetusviestin tekstikenttä ja paikka.
    /// </summary>
    /// <param name="viesti">Viesti, joka näytetään tekstikentässä</param>
    public void NaytaViesti(string viesti)
    {
        Label tekstikentta = new Label(500.0, 20.0, viesti);
        tekstikentta.Color = Color.White;
        tekstikentta.Position = new Vector(0, 200);
        Add(tekstikentta);
    }


}
