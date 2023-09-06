#version 330 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aColor;

uniform float zoom;
uniform mat4 translation;
uniform vec2 iResolution;

// Size of unit square in pixels
uniform float unit;

void main()
{
    vec2 scale = 16.0f * zoom / iResolution;
    mat4 scaleTransform = mat4(
        scale.x, 0.0f, 0.0f, 0.0f,
        0.0f, scale.y, 0.0f, 0.0f,
        0.0f, 0.0f, 1.0f, 0.0f,
        0.0f, 0.0f, 0.0f, 1.0f
    );
    
    mat4 view = scaleTransform * translation;
    gl_Position = view * vec4(aPosition, 1.0);
}