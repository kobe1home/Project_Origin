using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Project_Origin
{
    class DefaultEffect : Effect, IEffectMatrices
    {
        EffectParameter world;
        EffectParameter projection;

        public DefaultEffect(Effect effect)
            : base(effect)
        {
            world = Parameters["World"];
            projection = Parameters["Projection"];
        }

        public Matrix World
        {
            get { return world.GetValueMatrix(); }
            set { world.SetValue(value); }
        }
         
        public Matrix View
        {
            get { return Matrix.Identity; }
            set { }
        }

        public Matrix Projection
        {
            get { return projection.GetValueMatrix(); }
            set { projection.SetValue(value); }
        }
    }
}
