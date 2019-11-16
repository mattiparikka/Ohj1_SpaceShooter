using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

public class Pelaaja
{
    private IntMeter alusLaskuri = new IntMeter(10);
    public IntMeter AlusLaskuri { get { return alusLaskuri; } }
    private int tarkistin = 0;

    private Alus alus;
    public Alus Alus { get { return alus; } } //get, jotta voidaan kysyä mikä alus pelaajalla on.

    //private string tunniste;
    //public string Tunniste { get { return tunniste; } }
    private string nimi;
    public string Nimi { get { return nimi; } }

    private Vector aloitus;
    public Vector Aloitus { get { return aloitus; } }

    private Vector naytot;
    public Vector Naytot { get { return naytot; } }


    private Color vari;
    public Color Vari { get { return vari; } }

    private Key[] nappaimet = new Key[6];
    public Key[] Nappaimet { get { return nappaimet; } }

    private int alusKuva;
    public int AlusKuva { get { return alusKuva; } }


    private Label elamaNaytto;
    private Label alusNaytto;
    private Label ammusNaytto;

    private DoubleMeter testi;

    public Pelaaja(int alusKuva, string tunniste, string nimi, Vector sijainti, Vector naytot, Color vari, Key[] nappaimet)
    {
        //this.tunniste = tunniste;
        this.nimi = nimi;
        this.aloitus = sijainti;
        this.vari = vari;
        this.nappaimet = nappaimet;
        this.naytot = naytot;
        this.alusKuva = alusKuva;
    }

    public void LuoAlus(PhysicsGame peli, string tunniste, string nimi, Vector sijainti, Color vari)
    {
        tarkistin++;
        this.alus = new Alus(peli, sijainti, 125, vari, tunniste, nimi, tarkistin);

    }


    public void LuoNaytot(PhysicsGame peli)
    {
        Vector p1 = this.Naytot; //voi yksinkertaistaa

        this.elamaNaytto = new Label();
        elamaNaytto.Font = Font.DefaultSmallBold;
        elamaNaytto.Position = p1;
        elamaNaytto.BindTo(this.alus.ElamaLaskuri);
        elamaNaytto.TextColor = Color.White;
        elamaNaytto.Title = "Aluksen kunto";
        peli.Add(elamaNaytto);
        

        this.alusNaytto = new Label();
        alusNaytto.Font = Font.DefaultSmallBold;

        alusNaytto.Position = new Vector(p1.X, p1.Y - 25);
        alusNaytto.BindTo(this.AlusLaskuri);
        alusNaytto.TextColor = Color.White;
        alusNaytto.Title = "Aluksia jäljellä";
        peli.Add(alusNaytto);

        this.ammusNaytto = new Label();
        ammusNaytto.Font = Font.DefaultSmallBold;

        ammusNaytto.Position = new Vector(p1.X, p1.Y - 50);
        ammusNaytto.BindTo(this.Alus.AmmusLaskuri);
        ammusNaytto.TextColor = Color.White;
        ammusNaytto.Title = "Ase1 ammuksia jäljellä";
        peli.Add(ammusNaytto);
    }

    public void PoistaNaytot()
    {
        if (elamaNaytto == null || alusNaytto == null) return;
        this.elamaNaytto.Destroy();
        this.alusNaytto.Destroy();
        this.ammusNaytto.Destroy();
    }


}
