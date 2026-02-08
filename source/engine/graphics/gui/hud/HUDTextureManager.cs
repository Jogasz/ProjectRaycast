using OpenTK.Graphics.OpenGL4;

namespace Engine;

// Simple HUD texture loader/binder. Not part of global `Texture` lists.
internal static class HUDTextureManager
{
 static Texture? _sword;
 static Texture? _vignette;
 static Texture? _container;

 public static void Load()
 {
 // Order requirement:
 //0: sword
 //1: vignette
 //2: container
 _sword?.Dispose();
 _vignette?.Dispose();
 _container?.Dispose();

 _sword = new Texture("assets/textures/gui/hudTex/sword.png");
 _vignette = new Texture("assets/textures/gui/hudTex/vignette.png");
 _container = new Texture("assets/textures/gui/hudTex/container.png");
 }

 public static void BindAll()
 {
 _sword?.Use(TextureUnit.Texture0);
 _vignette?.Use(TextureUnit.Texture1);
 _container?.Use(TextureUnit.Texture2);
 }

 public static void Dispose()
 {
 _sword?.Dispose();
 _vignette?.Dispose();
 _container?.Dispose();
 _sword = null;
 _vignette = null;
 _container = null;
 }
}
