using System.Diagnostics;
using OpenTK.Graphics.ES30;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Orbis.UI;

public class OrbisWindow : GameWindow
{
    private int _vertexBufferObject;
    
    private int _elementBufferObject;
    
    private int _vertexArrayObject;

    private Stopwatch _stopwatch;
    
    private Shader _shader;

    private const float _unitScale = 32.0f;
    
    private float Zoom { get; set; }
    
    private readonly float[] _vertices = {
        -1.0f, -1.0f, 0.0f, 1.0f, 0.0f, 0.0f,
        -1.0f,  1.0f, 0.0f, 0.0f, 1.0f, 0.0f,
         1.0f,  1.0f, 0.0f, 0.0f, 0.0f, 1.0f,
         1.0f,  -1.0f, 0.0f, 1.0f, 1.0f, 1.0f
    };
    
    private readonly uint[] _indices = {  // note that we start from 0!
        0, 1, 2,   // first triangle
        0, 2, 3
    };

    private Vector2 _cameraPosition = (0, 0);
    
    public OrbisWindow(
        GameWindowSettings gameWindowSettings, 
        NativeWindowSettings nativeWindowSettings) : 
        base(gameWindowSettings, nativeWindowSettings)
    {
        Zoom = 1.0f;
    }
    
    protected override void OnLoad()
    {
        base.OnLoad();
        GL.ClearColor(0.23f, 0.16f, 0.23f, 1.0f);
                
        _vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArrayObject);
        
        _vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
        GL.BufferData(
            BufferTarget.ArrayBuffer, 
            _vertices.Length * sizeof(float), 
            _vertices, 
            BufferUsageHint.StaticDraw);
 
        _elementBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
        GL.BufferData(
            BufferTarget.ElementArrayBuffer, 
            _indices.Length * sizeof(uint), 
            _indices, 
            BufferUsageHint.StaticDraw);
        
        GL.VertexAttribPointer(
            0, 
            3, 
            VertexAttribPointerType.Float, 
            false, 
            6 * sizeof(float), 
            0);
        GL.EnableVertexAttribArray(0);
        
        GL.VertexAttribPointer(
            1, 
            3, 
            VertexAttribPointerType.Float, 
            false, 
            6 * sizeof(float), 
            3 * sizeof(float));
        
        GL.EnableVertexAttribArray(1);
        
        _shader = new Shader("Shaders/base.vert", "Shaders/base.frag");

        _stopwatch = new Stopwatch();
        _stopwatch.Start();
        
        GL.Uniform2(_shader.GetLocation("iResolution"), (float)Size.X, Size.Y);
        
        GL.Uniform1(_shader.GetLocation("unit"), _unitScale);
    }
    
    protected override void OnUpdateFrame(FrameEventArgs @event)
    {
        base.OnUpdateFrame(@event);

        const float speed = 4f;
        if (KeyboardState.IsKeyDown(Keys.D))
        {
            _cameraPosition.X += speed * (float) @event.Time;
        }
        if (KeyboardState.IsKeyDown(Keys.A))
        {
            _cameraPosition.X -= speed * (float) @event.Time;
        }
        if (KeyboardState.IsKeyDown(Keys.W))
        {
            _cameraPosition.Y += speed * (float) @event.Time;
        }
        if (KeyboardState.IsKeyDown(Keys.S))
        {
            _cameraPosition.Y -= speed * (float) @event.Time;
        }
    }
    
    protected override void OnRenderFrame(FrameEventArgs @event)
    {
        base.OnRenderFrame(@event);

        GL.Clear(ClearBufferMask.ColorBufferBit);

        _shader.Use();
        GL.BindVertexArray(_vertexArrayObject);
        
        var elapsedTime = _stopwatch.Elapsed.TotalSeconds;
        
        GL.Uniform1(_shader.GetLocation(nameof(elapsedTime)), (float) elapsedTime);
        GL.Uniform1(_shader.GetLocation("zoom"), Zoom);
        GL.Uniform2(_shader.GetLocation("iResolution"), (float)Size.X, Size.Y);
        GL.Uniform2(_shader.GetLocation("iMouse"), MouseState.X, MouseState.Y);

        var translation = Matrix4.CreateTranslation(_cameraPosition.X, _cameraPosition.Y, 0.0f);
        GL.UniformMatrix4(_shader.GetLocation(nameof(translation)), false, ref translation);
        
        
        
        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        
        SwapBuffers();
    }
    
    protected override void OnResize(ResizeEventArgs @event)
    {
        base.OnResize(@event);

        GL.Viewport(0, 0, @event.Width, @event.Height);
    }
}