using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

public class Asteroidi : PhysicsObject
{
    private IntMeter elamaLaskuri = new IntMeter(100);
    public IntMeter ElamaLaskuri { get { return elamaLaskuri; } set { elamaLaskuri = value; } }

    public Asteroidi(PhysicsGame peli, Vector p, double r, Color vari, String tunniste) : base(r,r)
    {
        this.Shape = Shape.Circle;
        this.Position = p;
        this.Color = vari;
        this.Mass = 10 * r;
        this.Angle = Angle.FromDegrees(RandomGen.NextDouble(0, 360));
        this.Tag = tunniste;
        this.Restitution = 0.1;
        this.CanRotate = false;
        peli.Add(this);
    }


}
