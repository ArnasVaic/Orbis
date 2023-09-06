#version 330 core
out vec4 FragColor;

in vec4 vertexColor;

uniform float elapsedTime;
uniform vec2 iResolution;
uniform vec2 iMouse;

void main()
{
    vec2 rg = iMouse / iResolution;
    
    rg.y = 1 - rg.y;
    
    FragColor = vec4(rg, 0.0f, 1.0f);
}