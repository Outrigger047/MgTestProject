using Microsoft.Xna.Framework;

namespace MgTestProject
{
    public class WorldShapeGeometry
    {
        private Vector2 position;

        private int width;
        private int height;
        private Shapes shapeType;

        private float rotAngle;

        private Color drawColor;

        private bool passable;
        private bool shouldRender;

        public WorldShapeGeometry(
            Vector2 position, 
            int width, 
            int height, 
            Shapes shapeType, 
            float rotAngle, 
            Color drawColor, 
            bool passable = false, 
            bool shouldRender = true)
        {
            Position = position;
            Width = width;
            Height = height;
            ShapeType = shapeType;
            RotAngle = rotAngle;
            DrawColor = drawColor;
            Passable = passable;
            ShouldRender = shouldRender;
        }

        public Vector2 Position { get => position; private set => position = value; }
        public int Width { get => width; private set => width = value; }
        public int Height { get => height; private set => height = value; }
        public Shapes ShapeType { get => shapeType; private set => shapeType = value; }
        public float RotAngle { get => rotAngle; private set => rotAngle = value; }
        public Color DrawColor { get => drawColor; private set => drawColor = value; }
        public bool Passable { get => passable; private set => passable = value; }
        public bool ShouldRender { get => shouldRender; private set => shouldRender = value; }
    }

    public enum Shapes
    {
        Rectangle,
        Ellipse
    }
}
