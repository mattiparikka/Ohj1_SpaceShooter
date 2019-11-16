using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

public class Alus : PhysicsObject

{
    private string pelaaja; // Luodaan nimi myöhemmin, jotta voidaan erottaa eri alukse toisistaan. Tunnisteella erotetaan objektit ts. alus, asteroidi jne.
    public string Pelaaja { get { return pelaaja; } }

    private Cannon ase1 = new Cannon(100, 100);
    public Cannon Ase1 { get { return ase1; } }

    private AssaultRifle ase2 = new AssaultRifle(100, 100);
    public AssaultRifle Ase2 { get { return ase2; } }


    private IntMeter elamaLaskuri = new IntMeter(20);
    public IntMeter ElamaLaskuri { get { return elamaLaskuri; } set { elamaLaskuri = value; } }
    private int tarkistin = 0;

    private IntMeter ammusLaskuri = new IntMeter(20);
    public IntMeter AmmusLaskuri { get { return ammusLaskuri; } set { ammusLaskuri = value; } }

    private double koko = 0;

    public double Koko { get { return koko; } }

    public Alus(PhysicsGame peli, Vector p, double r, Color vari, string tunniste, string pelaaja, int tarkistin) : base(r, r)
    {
        this.koko = r;
        this.Shape = Shape.Circle;
        this.Position = p;
        this.Color = vari;
        this.Mass = 5;
        this.Restitution = 0.1;
        this.CanRotate = false;
        this.Tag = tunniste;
        this.pelaaja = pelaaja;
        this.tarkistin = tarkistin;

        this.ase1.Power.Value = 100000;
        this.ase1.Power.DefaultValue = 100000;
        this.ase1.FireRate = 1;
        this.ase1.CanHitOwner = false;
        this.ase1.IsVisible = false;
        this.Add(ase1);


        this.ase2.Power.Value = 500;
        this.ase2.Power.DefaultValue = 500;
        this.ase2.FireRate = 10;
        this.ase2.CanHitOwner = false;
        this.ase2.IsVisible = false;
        this.ase2.Ammo.Value = 20;
        this.Add(ase2);

        peli.Add(this);
    }

    
    public void PoistaAse()
    {
        this.ase1 = null;
    }


    public void AmmuAseella1()
    {
        PhysicsObject ammus = this.ase1.Shoot();
        if (ammus != null)
        {
            ammus.Size *= 5;
        }
    }


    public void AmmuAseella2()
    {
        PhysicsObject ammus = this.ase2.Shoot();
        if (this.ase2.Ammo.Value == 0) Lataa();
        if (ammus != null)
        {
            this.ammusLaskuri.Value = this.ase2.Ammo.Value;
            ammus.Size *= 3;
            //ammus.MaximumLifetime = TimeSpan.FromSeconds(10.0);
        }
    }


    private void Lataa()
    {
        Timer laskuri = new Timer();
        laskuri.Interval = 4;
        laskuri.Timeout += delegate {
            this.ase2.Ammo.Value = this.ammusLaskuri.Value = 20;
        };
        laskuri.Start(1);
    }

    /// <summary>
    /// Aliohjelma, jolla lyödään alusta sen keulan suuntaisesti suuntaan
    /// </summary>
    /// <param name="alus">Kappale, jota lyödään</param>
    /// <param name="suunta">Suunta/voimavektori</param>
    public virtual void LyoAlusta(double suunta)
    {
        Vector pelaajanSuunta = Vector.FromLengthAndAngle(suunta, this.Angle);
        this.Hit(pelaajanSuunta);
        this.MaxVelocity = 1000;
    }


}
